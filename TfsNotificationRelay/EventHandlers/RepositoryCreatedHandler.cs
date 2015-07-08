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

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Git.Server;
using System.Collections.Generic;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    class RepositoryCreatedHandler : BaseHandler<RepositoryCreatedNotification>
    {
        protected override IEnumerable<Notifications.INotification> CreateNotifications(TeamFoundationRequestContext requestContext, RepositoryCreatedNotification ev, int maxLines)
        {
            var repositoryService = requestContext.GetService<TeamFoundationGitRepositoryService>();
            var identityService = requestContext.GetService<ITeamFoundationIdentityService>();
            var commonService = requestContext.GetService<CommonStructureService>();

            var identity = identityService.ReadIdentity(requestContext, IdentitySearchFactor.Identifier, ev.Creator.Identifier);

            using (TfsGitRepository repository = repositoryService.FindRepositoryById(requestContext, ev.RepositoryId))
            {
                string repoUri = repository.GetRepositoryUri(requestContext);

                var notification = new Notifications.RepositoryCreatedNotification()
                {
                    TeamProjectCollection = requestContext.ServiceHost.Name,
                    UniqueName = identity.UniqueName,
                    DisplayName = identity.DisplayName,
                    ProjectName = commonService.GetProject(requestContext, ev.TeamProjectUri).Name,
                    RepoUri = repoUri,
                    RepoName = ev.RepositoryName,
                    TeamNames = GetUserTeamsByProjectUri(requestContext, ev.TeamProjectUri, identity.Descriptor)
                };
                yield return notification;

            }
        }
    }
}
