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

using Microsoft.TeamFoundation.Framework.Server;
using TFVC = Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using DevCore.Tfs2Slack.Notifications;

namespace DevCore.Tfs2Slack.EventHandlers
{
    class CheckinHandler : BaseHandler
    {
        public override Type[] SubscribedTypes()
        {
            return new Type[] { typeof(TFVC.CheckinNotification) };
        }

        protected override INotification CreateNotification(TeamFoundationRequestContext requestContext, object notificationEventArgs, int maxLines)
        {
            var checkin = (TFVC.CheckinNotification)notificationEventArgs;

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
