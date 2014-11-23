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

using DevCore.Tfs2Slack.Notifications;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.EventHandlers
{
    class PullRequestStatusUpdateHandler : BaseHandler<StatusUpdateNotification>
    {
        protected override Notifications.INotification CreateNotification(TeamFoundationRequestContext requestContext, StatusUpdateNotification ev, int maxLines)
        {
            var repositoryService = requestContext.GetService<TeamFoundationGitRepositoryService>();
            var identityService = requestContext.GetService<ITeamFoundationIdentityService>();
            var commonService = requestContext.GetService<ICommonStructureService>();

            var identity = identityService.ReadIdentity(requestContext, IdentitySearchFactor.Identifier, ev.Updater.Identifier);

            using (TfsGitRepository repository = repositoryService.FindRepositoryById(requestContext, ev.RepositoryId))
            {
                var pullRequestService = requestContext.GetService<ITeamFoundationGitPullRequestService>();
                TfsGitPullRequest pullRequest;
                if (pullRequestService.TryGetPullRequestDetails(requestContext, repository, ev.PullRequestId, out pullRequest))
                {
                    string repoUri = repository.GetRepositoryUri(requestContext);
                    var notification = new Notifications.PullRequestStatusUpdateNotification()
                    {
                        TeamProjectCollection = requestContext.ServiceHost.Name,
                        Status = ev.Status,
                        UniqueName = identity.UniqueName,
                        DisplayName = identity.DisplayName,
                        ProjectName = commonService.GetProject(requestContext, ev.TeamProjectUri).Name,
                        RepoUri = repoUri,
                        RepoName = ev.RepositoryName,
                        PrId = pullRequest.PullRequestId,
                        PrUrl = string.Format("{0}/pullrequest/{1}#view=discussion", repoUri, ev.PullRequestId),
                        PrTitle = pullRequest.Title
                    };
                    return notification;
                }
                else
                {
                    throw new Tfs2SlackException("Unable to get pull request " + ev.PullRequestId);
                }

            }
        }
    }
}
