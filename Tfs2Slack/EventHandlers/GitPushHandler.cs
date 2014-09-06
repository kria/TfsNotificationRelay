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

using DevCore.Tfs2Slack.Configuration;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Integration.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.EventHandlers
{
    class GitPushHandler : IEventHandler
    {
        private static Configuration.SettingsElement settings = Configuration.Tfs2SlackSection.Instance.Settings;
        private static Configuration.TextElement text = Configuration.Tfs2SlackSection.Instance.Text;

        public IList<string> ProcessEvent(TeamFoundationRequestContext requestContext, object notificationEventArgs, Configuration.BotElement bot)
        {
            var pushNotification = (PushNotification)notificationEventArgs;
            if (!bot.NotifyOn.HasFlag(TfsEvents.GitPush)) return null;
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

                lines.Add(text.PushFormat.FormatWith(new
                {
                    UserName = userName,
                    Pushed = pushNotification.IsForceRequired(requestContext, repository) ? text.ForcePushed : text.Pushed,
                    RepoUri = repoUri,
                    ProjectName = projectName,
                    RepoName = repoName
                }));

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

                return lines;
            }
        }

        private static string RefsToString(IEnumerable<TfsGitRefUpdateResult> refUpdateResults)
        {
            return String.Concat(RefsToStrings(refUpdateResults));
        }

        private static string[] RefsToStrings(IEnumerable<TfsGitRefUpdateResult> refUpdateResults)
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

        private static string CommitToString(TeamFoundationRequestContext requestContext, TfsGitCommit gitCommit, string action, PushNotification pushNotification,
            Dictionary<byte[], List<string>> refNames)
        {
            var commitService = requestContext.GetService<TeamFoundationGitCommitService>();
            var commitManifest = commitService.GetCommitManifest(requestContext, gitCommit.Repository, gitCommit.ObjectId);            
            string repoUri = gitCommit.Repository.GetRepositoryUri(requestContext);
            string commitUri = repoUri + "/commit/" + gitCommit.ObjectId.ToHexString();
            DateTime authorTime = gitCommit.GetLocalAuthorTime(requestContext);
            string author = gitCommit.GetAuthor(requestContext);
            string authorName = gitCommit.GetAuthorName(requestContext);
            string authorEmail = gitCommit.GetAuthorEmail(requestContext);
            string comment = gitCommit.GetComment(requestContext);
            var changeCounts = String.Join(", ", commitManifest.ChangeCounts.Select(c => ChangeCountToString(c)));

            StringBuilder sb = new StringBuilder();
            List<string> names = null;
            if (refNames.TryGetValue(gitCommit.ObjectId, out names)) sb.AppendFormat("{0} ", String.Concat(names));
            string formattedTime = String.IsNullOrEmpty(text.DateTimeFormat) ? authorTime.ToString() : authorTime.ToString(text.DateTimeFormat);
            sb.Append(text.CommitFormat.FormatWith(new
            {
                Action = action,
                CommitUri = commitUri,
                CommitId = gitCommit.ObjectId.ToHexString(settings.HashLength),
                ChangeCounts = changeCounts,
                AuthorTime = formattedTime,
                Author = author,
                AuthorName = authorName,
                AuthorEmail = authorEmail,
                Comment = comment.Truncate(settings.CommentMaxLength)
            }));

            return sb.ToString();
        }

        private static string ChangeCountToString(KeyValuePair<TfsGitChangeType, int> changeCount) {
            string format = null;
            switch (changeCount.Key)
            {
                case TfsGitChangeType.Add: format = text.CountAddFormat; break;
                case TfsGitChangeType.Delete: format = text.CountDeleteFormat; break;
                case TfsGitChangeType.Edit: format = text.CountEditFormat; break;
                case TfsGitChangeType.Rename: format = text.CountRenameFormat; break;
                case TfsGitChangeType.SourceRename: format = text.CountSourceRenameFormat; break;
            }
            return format.FormatWith(new { Count = changeCount.Value });
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
