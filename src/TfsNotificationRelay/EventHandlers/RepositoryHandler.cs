/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2016 Kristian Adrup
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
using System;
using System.Linq;
using Microsoft.VisualStudio.Services.Location.Server;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    class RepositoryHandler : BaseHandler
    {
        public override Type[] SubscribedTypes()
        {
            return new Type[] {
                typeof(RepositoryCreatedNotification),
                typeof(RepositoryDeletedNotification),
                typeof(RepositoryRenamedNotification)
            };
        }

        protected override IEnumerable<Notifications.INotification> CreateNotifications(IVssRequestContext requestContext, object notificationEventArgs, int maxLines)
        {
            var repositoryService = requestContext.GetService<ITeamFoundationGitRepositoryService>();
            var identityService = requestContext.GetService<ITeamFoundationIdentityService>();
            var commonService = requestContext.GetService<CommonStructureService>();
            var locationService = requestContext.GetService<ILocationService>();

            string baseUrl = String.Format("{0}/{1}/",
                        locationService.GetAccessMapping(requestContext, "PublicAccessMapping").AccessPoint,
                        requestContext.ServiceHost.Name);

            var gitNotification = notificationEventArgs as GitNotification;

            Notifications.RepositoryNotification notification = null;

            if (gitNotification is RepositoryCreatedNotification)
            {
                var ev = notificationEventArgs as RepositoryCreatedNotification;
                var identity = identityService.ReadIdentity(requestContext, IdentitySearchFactor.Identifier, ev.Creator.Identifier);

                notification = new Notifications.RepositoryCreatedNotification();
                notification.UniqueName = identity.UniqueName;
                notification.DisplayName = identity.DisplayName;
                notification.TeamNames = GetUserTeamsByProjectUri(requestContext, gitNotification.TeamProjectUri, identity.Descriptor);
                using (ITfsGitRepository repository = repositoryService.FindRepositoryById(requestContext, gitNotification.RepositoryId))
                {
                    notification.RepoUri = repository.GetRepositoryUri();
                    notification.RepoName = repository.Name;
                }
            }
            else if (gitNotification is RepositoryRenamedNotification)
            {
                notification = new Notifications.RepositoryRenamedNotification();
                notification.RepoName = gitNotification.RepositoryName;
                using (ITfsGitRepository repository = repositoryService.FindRepositoryById(requestContext, gitNotification.RepositoryId))
                {
                    notification.RepoUri = repository.GetRepositoryUri();
                    notification.RepoName = repository.Name;
                }
                notification.TeamNames = Enumerable.Empty<string>();
            }
            else if (gitNotification is RepositoryDeletedNotification)
            {
                var repoInfo = repositoryService
                    .QueryDeletedRepositories(requestContext, gitNotification.RepositoryId)
                    .Single(r => r.Key.RepoId == gitNotification.RepositoryId);
                var identity = identityService.ReadIdentities(requestContext, new[] { repoInfo.DeletedBy }).First();
                notification = new Notifications.RepositoryDeletedNotification();
                notification.UniqueName = identity.UniqueName;
                notification.DisplayName = identity.DisplayName;
                notification.TeamNames = GetUserTeamsByProjectUri(requestContext, gitNotification.TeamProjectUri, identity.Descriptor);
                notification.RepoName = repoInfo.Name;
            }

            notification.TeamProjectCollection = requestContext.ServiceHost.Name;
            notification.ProjectName = commonService.GetProject(requestContext, gitNotification.TeamProjectUri).Name;
            notification.ProjectUrl = baseUrl + notification.ProjectName;

            yield return notification;
        }
    }
}
