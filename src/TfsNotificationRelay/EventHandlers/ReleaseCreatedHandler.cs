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

using DevCore.TfsNotificationRelay.Notifications;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.ServerEvents;
using System.Collections.Generic;
using System.Linq;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    class ReleaseCreatedHandler : BaseHandler<ReleaseCreatedServerEvent>
    {
        protected override IEnumerable<INotification> CreateNotifications(IVssRequestContext requestContext, ReleaseCreatedServerEvent ev, int maxLines)
        {
            var projectService = requestContext.GetService<IProjectService>();
            var identityService = requestContext.GetService<ITeamFoundationIdentityService>();

            var release = ev.Release;
            var creator = identityService.ReadIdentities(requestContext, new[] { release.CreatedBy }).First();

            string projectName = projectService.GetProjectName(requestContext, ev.ProjectId);
            var releaseUrl = WebAccessUrlBuilder.GetReleaseWebAccessUri(requestContext, projectName, release.Id);

            var notification = new ReleaseCreatedNotification
            {
                TeamProjectCollection = requestContext.ServiceHost.Name,
                ProjectName = projectName,
                ReleaseDefinition = release.ReleaseDefinitionName,
                ReleaseName = release.Name,
                ReleaseReason = release.Reason,
                ReleaseStatus = release.Status,
                ReleaseUrl = releaseUrl,
                CreatedOn = release.CreatedOn,
                CreatedByUniqueName = creator.UniqueName,
                CreatedByDisplayName = creator.DisplayName,
                TeamNames = GetUserTeamsByProjectName(requestContext, projectName, creator.Descriptor)
            };

            yield return notification;
        }
    }
}
