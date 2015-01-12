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

using Microsoft.TeamFoundation.Framework.Server;
using TFVC = Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DevCore.TfsNotificationRelay.Notifications;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    class CheckinHandler : BaseHandler<TFVC.CheckinNotification>
    {
        protected override INotification CreateNotification(TeamFoundationRequestContext requestContext, TFVC.CheckinNotification checkin, int maxLines)
        {
            var locationService = requestContext.GetService<TeamFoundationLocationService>();
            string baseUrl = String.Format("{0}/{1}/",
                    locationService.GetAccessMapping(requestContext, "PublicAccessMapping").AccessPoint,
                    requestContext.ServiceHost.Name);

            var notification = new CheckinNotification()
            {
                TeamProjectCollection = requestContext.ServiceHost.Name,
                UniqueName = checkin.ChangesetOwner.UniqueName,
                DisplayName = checkin.ChangesetOwner.DisplayName,
                ChangesetUrl = String.Format("{0}_versionControl/changeset/{1}", baseUrl, checkin.Changeset),
                ChangesetId = checkin.Changeset,
                Projects = new Dictionary<string, string>(),
                Comment = checkin.Comment
            };

            string pattern = @"^\$\/([^\/]*)\/";
            foreach (string item in checkin.GetSubmittedItems(requestContext))
            {
                Match match = Regex.Match(item, pattern);
                if (match.Success)
                {
                    string projectName = match.Groups[1].Value;
                    if (notification.Projects.ContainsKey(projectName)) continue;
                    string projectUrl = baseUrl + projectName;
                    notification.Projects.Add(projectName, projectUrl);
                }
            }

            return notification;   
        }
    }
}
