/*
 * Tfs2Slack - http://github.com/kria/Tfs2Slack
 * 
 * Copyright (C) 2014 Kristian Adrup
 * 
 * This file is part of Tfs2Slack.
 * 
 * Tfs2Slack is free software: you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version. See included file COPYING for details.
 */

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Integration.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace DevCore.Tfs2Slack
{
    class NotificationSubscriber : ISubscriber
    {
        private Properties.Settings settings = Properties.Settings.Default;
        private Properties.Text text = Properties.Text.Default;

        public string Name
        {
            get { return "Tfs2Slack handler"; }
        }

        public SubscriberPriority Priority
        {
            get { return SubscriberPriority.Normal; }
        }

        public Type[] SubscribedTypes()
        {
            return new Type[1] { typeof(PushNotification) };
        }

        public EventNotificationStatus ProcessEvent(TeamFoundationRequestContext requestContext, NotificationType notificationType,
            object notificationEventArgs, out int statusCode, out string statusMessage, out Microsoft.TeamFoundation.Common.ExceptionPropertyCollection properties)
        {
            statusCode = 0;
            statusMessage = string.Empty;
            properties = null;

            try
            {
                if (notificationType == NotificationType.Notification && notificationEventArgs is PushNotification)
                {
                    Stopwatch timer = new Stopwatch();
                    timer.Start();

                    PushNotification pushNotification = notificationEventArgs as PushNotification;
                    var repositoryService = requestContext.GetService<TeamFoundationGitRepositoryService>();
                    var commonService = requestContext.GetService<CommonStructureService>();
                    
                    using (TfsGitRepository repository = repositoryService.FindRepositoryById(requestContext, pushNotification.RepositoryId))
                    {
                        string repoName = pushNotification.RepositoryName;
                        string repoUri = repository.GetRepositoryUri(requestContext);
                        string projectName = commonService.GetProject(requestContext, pushNotification.TeamProjectUri).Name;
                        string userName = pushNotification.AuthenticatedUserName;
                        if (settings.StripUserDomain) userName = Utils.StripDomain(userName);

                        var lines = new List<string>();
                        
                        lines.Add(text.FormatPushText(userName, repoUri, projectName, repoName, pushNotification.IsForceRequired(requestContext, repository)));

                        var refNames = new Dictionary<byte[], List<string>>(new ByteArrayComparer());
                        var oldCommits = new HashSet<byte[]>(new ByteArrayComparer());
                        var unknowns = new List<RefUpdateResultGroup>();

                        // Associate refs (branch, lightweight and annotated tag) with corresponding commit
                        var refUpdateResultGroups = pushNotification.RefUpdateResults
                            .Where(r => r.Succeeded)
                            .GroupBy(r => r.NewObjectId, (key, items) => new RefUpdateResultGroup(key, items), new ByteArrayComparer());

                        foreach (var refUpdateResultGroup in refUpdateResultGroups)
                        {
                            byte[] newObjectId = refUpdateResultGroup.NewObjectId;
                            byte[] commitId = null;
 
                            if (newObjectId.IsZero())
                            {
                                commitId = newObjectId;
                            }
                            else
                            {
                                TfsGitObject gitObject = repository.LookupObject(requestContext, newObjectId);

                                if (gitObject.ObjectType == TfsGitObjectType.Commit)
                                {
                                    commitId = newObjectId;
                                }
                                else if (gitObject.ObjectType == TfsGitObjectType.Tag)
                                {
                                    var tag = (TfsGitTag)gitObject;
                                    var commit = tag.TryResolveToCommit(requestContext);
                                    if (commit != null)
                                    {
                                        commitId = commit.ObjectId;
                                    }
                                }
                            }

                            if (commitId != null)
                            {
                                List<string> names;
                                if (!refNames.TryGetValue(commitId, out names))
                                {
                                    names = new List<string>();
                                    refNames.Add(commitId, names);
                                }
                                names.AddRange(RefsToStrings(refUpdateResultGroup.RefUpdateResults));

                                if (commitId.IsZero() || !pushNotification.IncludedCommits.Any(r => r.SequenceEqual(commitId)))
                                {
                                    oldCommits.Add(commitId);
                                }
                            }
                            else 
                            {
                                unknowns.Add(refUpdateResultGroup);
                            }
                            
                        }

                        // Display new commits with refs
                        foreach (byte[] commitId in pushNotification.IncludedCommits)
                        {
                            TfsGitCommit gitCommit = (TfsGitCommit)repository.LookupObject(requestContext, commitId);
                            string line = CommitToString(requestContext, gitCommit, text.Commit, pushNotification, refNames);
                            lines.Add(line);
                        }

                        // Display updated refs to old commits
                        foreach (byte[] commitId in oldCommits)
                        {
                            string line = null;

                            if (commitId.IsZero())
                            {
                                line = String.Format("{0} {1}", String.Concat(refNames[commitId]), text.Deleted);
                            }
                            else
                            {
                                TfsGitCommit gitCommit = (TfsGitCommit)repository.LookupObject(requestContext, commitId);
                                line = CommitToString(requestContext, gitCommit, text.RefPointer, pushNotification, refNames);
                            }
                            lines.Add(line);
                        }

                        // Display "unknown" refs
                        foreach (var refUpdateResultGroup in unknowns)
                        {
                            byte[] newObjectId = refUpdateResultGroup.NewObjectId;
                            TfsGitObject gitObject = repository.LookupObject(requestContext, newObjectId);
                            string line = String.Format("{0} {1} {2} {3}", 
                                RefsToString(refUpdateResultGroup.RefUpdateResults), text.RefPointer, gitObject.ObjectType, newObjectId.ToHexString());

                            lines.Add(line);
                        }

                        //Log(lines);

                        List<string> sendLines = lines;
                        if (lines.Count > settings.MaxLines)
                        {
                            sendLines = lines.Take(settings.MaxLines).ToList();
                            sendLines.Add(text.FormatLinesSupressedText(lines.Count - settings.MaxLines));
                        }

                        Task.Run(() => SendToSlack(sendLines));
                    }

                    timer.Stop();
                    //Log("Time spent in ProcessEvent: " + timer.Elapsed);
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }

            return EventNotificationStatus.ActionPermitted;
        }

        private string RefsToString(IEnumerable<TfsGitRefUpdateResult> refUpdateResults)
        {
            return String.Concat(RefsToStrings(refUpdateResults));
        }

        private string[] RefsToStrings(IEnumerable<TfsGitRefUpdateResult> refUpdateResults)
        {
            if (refUpdateResults.Count() == 0) return null;
            var refStrings = new List<string>();
            foreach (var gitRef in refUpdateResults)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('[');
                if (gitRef.Name.StartsWith("refs/heads/") && gitRef.OldObjectId.IsZero())
                    sb.Append('+');
                sb.AppendFormat("{0}]", gitRef.Name.Replace("refs/heads/", "").Replace("refs/tags/", ""));
                refStrings.Add(sb.ToString());
            }
            return refStrings.ToArray();
        }

        private string CommitToString(TeamFoundationRequestContext requestContext, TfsGitCommit gitCommit, string action, PushNotification pushNotification, 
            Dictionary<byte[], List<string>> refNames)
        {
            string repoUri = gitCommit.Repository.GetRepositoryUri(requestContext);
            string commitUri = repoUri + "/commit/" + gitCommit.ObjectId.ToHexString();
            DateTime authorTime = gitCommit.GetLocalAuthorTime(requestContext);
            string authorName = gitCommit.GetAuthor(requestContext);
            string comment = gitCommit.GetComment(requestContext);

            StringBuilder sb = new StringBuilder();
            List<string> names = null;
            if (refNames.TryGetValue(gitCommit.ObjectId, out names)) sb.AppendFormat("{0} ", String.Concat(names));
            sb.Append(text.FormatCommitText(action, commitUri, gitCommit.ObjectId.ToShortHexString(), authorTime, authorName, comment.Truncate(settings.CommentMaxLength)));

            return sb.ToString();
        }

        private void SendToSlack(IEnumerable<string> lines)
        {
            try
            {
                dynamic json = JObject.FromObject(new
                {
                    channel = settings.SlackChannel,
                    username = settings.SlackUsername,
                    attachments = new[] {
                        new {
                            fallback = lines.First(),
                            pretext = lines.First(),
                            color = settings.SlackColor,
                            mrkdwn_in = new [] { "pretext", "text", "title", "fields", "fallback" },
                            fields = from line in lines.Skip(1) select new { value = line, @short = false }
                        }
                    }
                });
                if (!String.IsNullOrEmpty(settings.SlackIconUrl))
                    json.icon_url = settings.SlackIconUrl;
                else if (!String.IsNullOrEmpty(settings.SlackIconEmoji))
                    json.icon_emoji = settings.SlackIconEmoji;

                Log("json= " + json.ToString());

                using (var client = new HttpClient())
                {
                    var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                    var result = client.PostAsync(settings.SlackWebhookUrl, content).Result;
                    result.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void Log(IEnumerable<string> lines)
        {
            if (String.IsNullOrEmpty(settings.Logfile)) return;

            using (StreamWriter sw = File.AppendText(settings.Logfile))
            {
                foreach (string line in lines)
                {
                    sw.WriteLine("[{0}] {1}", DateTime.Now, line);
                }
            }
        }
        private void Log(string line)
        {
            Log(new[] { line });
        }
        private void Log(Exception ex)
        {
            Log(ex.ToString());
        }

        
    }

    class RefUpdateResultGroup
    {
        public RefUpdateResultGroup(byte[] newObjectId, IEnumerable<TfsGitRefUpdateResult> refUpdateResults)
        {
            this.NewObjectId = newObjectId;
            this.RefUpdateResults = refUpdateResults;
        }
        public byte[] NewObjectId { get; set; }
        public IEnumerable<TfsGitRefUpdateResult> RefUpdateResults { get; set; }

    }

}
