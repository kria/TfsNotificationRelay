/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2015 Kristian Adrup
 * 
 * This file is part of TfsNotificationRelay.
 * 
 * TfsNotificationRelay is free software: you can redistribute it and/or 
 * modify it under the terms of the GNU General Public License as published 
 * by the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. See included file COPYING for details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevCore.TfsNotificationRelay.Notifications;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Core;
using Newtonsoft.Json.Linq;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Discussion.Server;
using Microsoft.TeamFoundation.Discussion.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System.Text.RegularExpressions;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Location.Server;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    class DiscussionsHandler : BaseHandler<DiscussionsNotification>
    {
        protected override IEnumerable<INotification> CreateNotifications(TeamFoundationRequestContext requestContext,
            DiscussionsNotification args, int maxLines)
        {
            var locationService = requestContext.GetService<ILocationService>();
            var linkingService = requestContext.GetService<TeamFoundationLinkingService>();
            var hyperlinkService = requestContext.GetService<TswaServerHyperlinkService>();
            var identityService = requestContext.GetService<ITeamFoundationIdentityService>();
            var commonService = requestContext.GetService<ICommonStructureService>();
            var discussionService = requestContext.GetService<ITeamFoundationDiscussionService>();

            var notifications = new List<INotification>();
            
            foreach (var thread in args.Threads)
            {
                var artifactId = LinkingUtilities.DecodeUri(thread.ArtifactUri);

                int discussionId = thread.DiscussionId <= 0 ? thread.Comments[0].DiscussionId : thread.DiscussionId;
                List<DiscussionComment> discussionComments;
                discussionService.QueryDiscussionsById(requestContext, discussionId, out discussionComments);

                if (artifactId.ArtifactType.Equals("Commit", StringComparison.OrdinalIgnoreCase))
                {
                    var repositoryService = requestContext.GetService<TeamFoundationGitRepositoryService>();
                    var commitService = requestContext.GetService<TeamFoundationGitCommitService>();
                    
                    Guid projectGuid;
                    Guid repositoryId;
                    Sha1Id commitId;
                    GitCommitArtifactId.Decode(artifactId, out projectGuid, out repositoryId, out commitId);

                    using (TfsGitRepository repository = repositoryService.FindRepositoryById(requestContext, repositoryId))
                    {
                        var project = commonService.GetProject(requestContext, projectGuid);
                        var repoUri = repository.GetRepositoryUri(requestContext);
                        var commitUri = repoUri + "/commit/" + commitId.ToHexString();
                        string itemPath;
                        if (thread.Properties.TryGetValue<string>("Microsoft.TeamFoundation.Discussion.ItemPath", out itemPath))
                        {
                            commitUri += string.Format("#path={0}&discussionId={1}&_a=contents", Uri.EscapeDataString(itemPath), discussionId);
                        }

                        var commitManifest = commitService.GetCommitManifest(requestContext, repository, commitId);

                        var pusher = identityService.ReadIdentities(requestContext, new[] { commitManifest.PusherId }).FirstOrDefault();

                        foreach (var comment in thread.Comments)
                        {
                            var commenter = identityService.ReadIdentities(requestContext, new[] { comment.Author }).First();

                            var notification = new Notifications.CommitCommentNotification()
                            {
                                TeamProjectCollection = requestContext.ServiceHost.Name,
                                PusherUserName = pusher?.UniqueName,
                                UniqueName = commenter.UniqueName,
                                DisplayName = comment.AuthorDisplayName,
                                ProjectName = project.Name,
                                RepoUri = repoUri,
                                RepoName = repository.Name,
                                CommitId = commitId,
                                CommitUri = commitUri,
                                Comment = TextHelper.Truncate(comment.Content, Settings.DiscussionCommentMaxLength),
                                TeamNames = GetUserTeamsByProjectUri(requestContext, project.Uri, commenter.Descriptor),
                            };

                            notifications.Add(notification);
                        }
                    }

                }
                else if (artifactId.ArtifactType.Equals("CodeReviewId", StringComparison.OrdinalIgnoreCase))
                {
                    Guid projectGuid;
                    int pullRequestId;
                    LegacyCodeReviewArtifactId.Decode(artifactId, out projectGuid, out pullRequestId);

                    var pullRequestService = requestContext.GetService<ITeamFoundationGitPullRequestService>();
                    var pullRequest = pullRequestService.GetPullRequestDetails(requestContext, pullRequestId);

                    var repositoryService = requestContext.GetService<TeamFoundationGitRepositoryService>();

                    using (TfsGitRepository repository = repositoryService.FindRepositoryById(requestContext, pullRequest.RepositoryId))
                    {
                        var project = commonService.GetProject(requestContext, projectGuid);
                        string repoUri = repository.GetRepositoryUri(requestContext);
                        var creator = identityService.ReadIdentities(requestContext, new[] { pullRequest.Creator }).FirstOrDefault();

                        foreach (var comment in thread.Comments)
                        {
                            var commenter = identityService.ReadIdentities(requestContext, new[] { comment.Author }).First();
                            
                            var notification = new Notifications.PullRequestCommentNotification()
                            {
                                TeamProjectCollection = requestContext.ServiceHost.Name,
                                CreatorUserName = creator?.UniqueName,
                                UniqueName = commenter.UniqueName,
                                DisplayName = commenter.DisplayName,
                                ProjectName = project.Name,
                                RepoUri = repoUri,
                                RepoName = repository.Name,
                                PrId = pullRequest.PullRequestId,
                                PrUrl = $"{repoUri}/pullrequest/{pullRequest.PullRequestId}#view=discussion",
                                PrTitle = pullRequest.Title,
                                TeamNames = GetUserTeamsByProjectUri(requestContext, project.Uri, commenter.Descriptor),
                                SourceBranch = new GitRef(pullRequest.SourceBranchName),
                                TargetBranch = new GitRef(pullRequest.TargetBranchName),
                                Comment = TextHelper.Truncate(comment.Content, Settings.DiscussionCommentMaxLength)
                            };

                            notifications.Add(notification);
                        }
                    }

                }
                else if (artifactId.ArtifactType.Equals("Changeset", StringComparison.OrdinalIgnoreCase))
                {
                    var versionControlService = requestContext.GetService<TeamFoundationVersionControlService>();
                    
                    var uri = new VersionControlIntegrationUri(thread.ArtifactUri);
                    var changesetId = int.Parse(uri.ArtifactName);
                    var reader = versionControlService.QueryChangeset(requestContext, changesetId, true, false, false);
                    var changeset = reader.Current<Changeset>();

                    var changesetUrl = hyperlinkService.GetChangesetDetailsUrl(changesetId).AbsoluteUri;
                    string baseUrl = String.Format("{0}/{1}/",
                        locationService.GetAccessMapping(requestContext, "PublicAccessMapping").AccessPoint,
                        requestContext.ServiceHost.Name);

                    string itemPath = string.Empty;
                    string projectName = string.Empty;
                    const string projectNamePattern = @"^\$\/([^\/]*)\/";

                    if (thread.Properties != null && thread.Properties.TryGetValue<string>("Microsoft.TeamFoundation.Discussion.ItemPath", out itemPath))
                    {
                        changesetUrl += string.Format("#path={0}&discussionId={1}&_a=contents", Uri.EscapeDataString(itemPath), discussionId);
                        Match match = Regex.Match(itemPath, projectNamePattern);
                        if (match.Success) projectName = match.Groups[1].Value;
                    } else
                    {
                        // This assumes changeset doesn't span multiple projects.
                        var serverItem = changeset.Changes.FirstOrDefault()?.Item.ServerItem;
                        if (serverItem != null)
                        {
                            Match match = Regex.Match(serverItem, projectNamePattern);
                            if (match.Success) projectName = match.Groups[1].Value;
                        }
                    }

                    var commiter = identityService.ReadIdentity(requestContext, IdentitySearchFactor.AccountName, changeset.Committer);

                    foreach (var comment in thread.Comments)
                    {
                        var commenter = identityService.ReadIdentities(requestContext, new[] { comment.Author }).First();

                        var notification = new Notifications.ChangesetCommentNotification()
                        {
                            TeamProjectCollection = requestContext.ServiceHost.Name,
                            CreatorUserName = commiter?.UniqueName,
                            UniqueName = commenter.UniqueName,
                            DisplayName = commenter.DisplayName,
                            ProjectUrl = baseUrl + projectName,
                            ProjectName = projectName,
                            ChangesetId = changesetId,
                            ChangesetUrl = changesetUrl,
                            TeamNames = GetUserTeamsByProjectName(requestContext, projectName, commenter.Descriptor),
                            SourcePath = itemPath,
                            Comment = TextHelper.Truncate(comment.Content, Settings.DiscussionCommentMaxLength)
                        };

                        notifications.Add(notification);
                    }
                }

            }

            return notifications;
        }
    }
}
