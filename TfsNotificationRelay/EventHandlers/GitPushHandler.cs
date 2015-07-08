/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2014 Kristian Adrup
 * 
 * This file is part of TfsNotificationRelay.
 * 
 * TfsNotificationRelay is free software: you can redistribute it and/or 
 * modify it under the terms of the GNU General Public License as published 
 * by the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. See included file COPYING for details.
 */

using DevCore.TfsNotificationRelay.Configuration;
using DevCore.TfsNotificationRelay.Notifications;
using DevCore.TfsNotificationRelay.Notifications.GitPush;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    class GitPushHandler : BaseHandler<PushNotification>
    {
        protected override IEnumerable<INotification> CreateNotifications(TeamFoundationRequestContext requestContext, PushNotification pushNotification, int maxLines)
        {
            var repositoryService = requestContext.GetService<TeamFoundationGitRepositoryService>();
            var commonService = requestContext.GetService<CommonStructureService>();
            var commitService = requestContext.GetService<TeamFoundationGitCommitService>();
            var identityService = requestContext.GetService<TeamFoundationIdentityService>();
            
            var identity = identityService.ReadIdentity(requestContext, IdentitySearchFactor.Identifier, pushNotification.Pusher.Identifier);
            var teamNames = GetUserTeamsByProjectUri(requestContext, pushNotification.TeamProjectUri, pushNotification.Pusher);

            using (TfsGitRepository repository = repositoryService.FindRepositoryById(requestContext, pushNotification.RepositoryId))
            {
                var pushRow = new PushRow()
                {
                    UniqueName = pushNotification.AuthenticatedUserName,
                    DisplayName = identity.DisplayName,
                    RepoName = pushNotification.RepositoryName,
                    RepoUri = repository.GetRepositoryUri(requestContext),
                    ProjectName = commonService.GetProject(requestContext, pushNotification.TeamProjectUri).Name,
                    IsForcePush = settings.IdentifyForcePush ? pushNotification.IsForceRequired(requestContext, repository) : false
                };
                var notification = new GitPushNotification(requestContext.ServiceHost.Name, pushRow.ProjectName, pushRow.RepoName, teamNames);
                notification.Add(pushRow);
                notification.TotalLineCount++;

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

                notification.TotalLineCount += pushNotification.IncludedCommits.Count() + oldCommits.Count + unknowns.Count;

                // Add new commits with refs
                foreach (byte[] commitId in pushNotification.IncludedCommits.TakeWhile(c => notification.Count < maxLines))
                {
                    TfsGitCommit gitCommit = (TfsGitCommit)repository.LookupObject(requestContext, commitId);
                    notification.Add(CreateCommitRow(requestContext, commitService, gitCommit, CommitRowType.Commit, pushNotification, refNames));
                }

                // Add updated refs to old commits
                foreach (byte[] commitId in oldCommits.TakeWhile(c => notification.Count < maxLines))
                {
                    if (commitId.IsZero())
                    {
                        notification.Add(new DeleteRow() { RefNames = refNames[commitId] });
                    }
                    else
                    {
                        TfsGitCommit gitCommit = (TfsGitCommit)repository.LookupObject(requestContext, commitId);
                        notification.Add(CreateCommitRow(requestContext, commitService, gitCommit, CommitRowType.RefUpdate, pushNotification, refNames));
                    }
                }

                // Add "unknown" refs
                foreach (var refUpdateResultGroup in unknowns.TakeWhile(c => notification.Count < maxLines))
                {
                    byte[] newObjectId = refUpdateResultGroup.NewObjectId;
                    TfsGitObject gitObject = repository.LookupObject(requestContext, newObjectId);
                    notification.Add(new RefUpdateRow()
                    {
                        NewObjectId = newObjectId,
                        ObjectType = gitObject.ObjectType,
                        RefNames = RefsToStrings(refUpdateResultGroup.RefUpdateResults)
                    });
                }

                yield return notification;
            }
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

        private static CommitRow CreateCommitRow(TeamFoundationRequestContext requestContext, TeamFoundationGitCommitService commitService,  
            TfsGitCommit gitCommit, CommitRowType rowType, PushNotification pushNotification, Dictionary<byte[], List<string>> refNames)
        {
            var commitManifest = commitService.GetCommitManifest(requestContext, gitCommit.Repository, gitCommit.ObjectId);
            string repoUri = gitCommit.Repository.GetRepositoryUri(requestContext);

            var commitRow = new CommitRow()
            {
                CommitId = gitCommit.ObjectId,
                Type = rowType,
                CommitUri = repoUri + "/commit/" + gitCommit.ObjectId.ToHexString(),
                AuthorTime = gitCommit.GetLocalAuthorTime(requestContext),
                Author = gitCommit.GetAuthor(requestContext),
                AuthorName = gitCommit.GetAuthorName(requestContext),
                AuthorEmail = gitCommit.GetAuthorEmail(requestContext),
                Comment = gitCommit.GetComment(requestContext),
                ChangeCounts = commitManifest.ChangeCounts
            };
            List<string> refs = null;
            refNames.TryGetValue(gitCommit.ObjectId, out refs);
            commitRow.RefNames = refs;

            return commitRow;
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
