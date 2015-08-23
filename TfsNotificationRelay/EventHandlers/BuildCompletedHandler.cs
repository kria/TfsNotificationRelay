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

using DevCore.TfsNotificationRelay.Notifications;
using Microsoft.TeamFoundation.Build.WebApi.Events;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    public class BuildCompletedHandler : BaseHandler<BuildCompletedEvent>
    {
        protected override IEnumerable<INotification> CreateNotifications(TeamFoundationRequestContext requestContext, BuildCompletedEvent notificationEventArgs, int maxLines)
        {
            var build = notificationEventArgs.Build;

            var converter = new Build2Converter();

            var notification = new BuildCompletionNotification()
            {
                TeamProjectCollection = requestContext.ServiceHost.Name,
                BuildUrl = ((ReferenceLink)build.Links.Links["web"]).Href,
                ProjectName = build.Project.Name,
                BuildNumber = build.BuildNumber,
                BuildStatus = converter.ConvertBuildStatus(build.Status, build.Result),
                BuildReason = converter.ConvertReason(build.Reason),
                StartTime = build.StartTime.Value,
                FinishTime = build.FinishTime.Value,
                RequestedFor = build.RequestedFor.UniqueName,
                RequestedForDisplayName = build.RequestedFor.DisplayName,
                BuildDefinition = build.Definition.Name,
                DropLocation = "",
                TeamNames = GetUserTeamsByProjectName(requestContext, build.Project.Name, build.RequestedFor.UniqueName)
            };

            yield return notification;
        }

        
    }
    
}
