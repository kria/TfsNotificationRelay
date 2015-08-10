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

using DevCore.TfsNotificationRelay.Notifications;
using DevCore.TfsNotificationRelay.Notifications.GitPush;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Common;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core;
using WebApi = Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;
using System.Linq;

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
                var notification = new GitPushNotification(requestContext.ServiceHost.Name, pushRow.ProjectName, 
                    pushNotification.AuthenticatedUserName, pushRow.RepoName, teamNames,
                    pushNotification.RefUpdateResults.Where(r => r.Succeeded).Select(r => new GitRef(r)));
                notification.Add(pushRow);
                notification.TotalLineCount++;

                var refLookup = new Dictionary<Sha1Id, List<GitRef>>();
                var deletedRefs = new List<GitRef>();
                var oldCommits = new HashSet<TfsGitCommit>(new TfsGitObjectEqualityComparer());
                var unknowns = new List<TfsGitRefUpdateResult>();

                // Associate refs (branch, lightweight and annotated tag) with corresponding commit
                foreach (var refUpdateResult in pushNotification.RefUpdateResults.Where(r => r.Succeeded))
                {
                    var newObjectId = refUpdateResult.NewObjectId;
                    TfsGitCommit commit = null;

                    if (newObjectId.IsEmpty)
                    {
                        deletedRefs.Add(new GitRef(refUpdateResult));
                        continue;
                    }

                    TfsGitObject gitObject = repository.LookupObject(requestContext, newObjectId);

                    if (gitObject.ObjectType == WebApi.GitObjectType.Commit)
                    {
                        commit = gitObject as TfsGitCommit;
                    }
                    else if (gitObject.ObjectType == WebApi.GitObjectType.Tag)
                    {
                        var tag = (TfsGitTag)gitObject;
                        commit = tag.TryResolveToCommit(requestContext);
                    }

                    if (commit != null)
                    {
                        List<GitRef> refs;
                        if (!refLookup.TryGetValue(commit.ObjectId, out refs))
                        {
                            refs = new List<GitRef>();
                            refLookup.Add(commit.ObjectId, refs);
                        }
                        refs.Add(new GitRef(refUpdateResult));

                        if (!pushNotification.IncludedCommits.Contains(commit.ObjectId))
                        {
                            oldCommits.Add(commit);
                        }
                    }
                    else
                    {
                        unknowns.Add(refUpdateResult);
                    }

                }

                notification.TotalLineCount += pushNotification.IncludedCommits.Count() + oldCommits.Count + unknowns.Count;

                // Add new commits with refs
                foreach (var commitId in pushNotification.IncludedCommits.TakeWhile(c => notification.Count < maxLines))
                {
                    TfsGitCommit gitCommit = (TfsGitCommit)repository.LookupObject(requestContext, commitId);
                    notification.Add(CreateCommitRow(requestContext, commitService, gitCommit, CommitRowType.Commit, pushNotification, refLookup));
                }

                // Add updated refs to old commits
                foreach (TfsGitCommit gitCommit in oldCommits.OrderByDescending(c => c.GetCommitTime(requestContext)).TakeWhile(c => notification.Count < maxLines))
                {
                    notification.Add(CreateCommitRow(requestContext, commitService, gitCommit, CommitRowType.RefUpdate, pushNotification, refLookup));
                }

                // Add deleted refs if any
                if (deletedRefs.Any() && notification.Count < maxLines)
                {
                    notification.Add(new DeleteRow() { Refs = deletedRefs });
                }

                // Add "unknown" refs
                foreach (var refUpdateResult in unknowns.TakeWhile(c => notification.Count < maxLines))
                {
                    var newObjectId = refUpdateResult.NewObjectId;
                    TfsGitObject gitObject = repository.LookupObject(requestContext, newObjectId);
                    notification.Add(new RefUpdateRow()
                    {
                        NewObjectId = newObjectId,
                        ObjectType = gitObject.ObjectType,
                        Refs = new[] { new GitRef(refUpdateResult) }
                    });
                }

                yield return notification;
            }
        }

        private static CommitRow CreateCommitRow(TeamFoundationRequestContext requestContext, TeamFoundationGitCommitService commitService,  
            TfsGitCommit gitCommit, CommitRowType rowType, PushNotification pushNotification, Dictionary<Sha1Id, List<GitRef>> refLookup)
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
            List<GitRef> refs = null;
            refLookup.TryGetValue(gitCommit.ObjectId, out refs);
            commitRow.Refs = refs;

            return commitRow;
        }
    }

}
