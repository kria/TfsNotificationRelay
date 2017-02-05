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
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TFVC = Microsoft.TeamFoundation.VersionControl.Server;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    class CheckinHandler : BaseHandler<TFVC.CheckinNotification>
    {
        protected override IEnumerable<INotification> CreateNotifications(IVssRequestContext requestContext, TFVC.CheckinNotification checkin, int maxLines)
        {
            var locationService = requestContext.GetService<ILocationService>();

            string baseUrl = String.Format("{0}/{1}/",
                    locationService.GetAccessMapping(requestContext, "PublicAccessMapping").AccessPoint,
                    requestContext.ServiceHost.Name);

            var teamNames = new HashSet<string>();
            var projects = new Dictionary<string, string>();

            var submittedItems = checkin.GetSubmittedItems(requestContext).ToList();

            const string pattern = @"^\$\/([^\/]*)\/";
            foreach (string item in submittedItems)
            {
                Match match = Regex.Match(item, pattern);
                if (match.Success)
                {
                    string projectName = match.Groups[1].Value;
                    if (projects.ContainsKey(projectName)) continue;

                    string projectUrl = baseUrl + projectName;
                    projects.Add(projectName, projectUrl);
                    
                    foreach (var team in GetUserTeamsByProjectName(requestContext, projectName, checkin.ChangesetOwner.Descriptor))
                    {
                        teamNames.Add(team);
                    }
                }
            }

            var notification = new CheckinNotification()
            {
                TeamProjectCollection = requestContext.ServiceHost.Name,
                UniqueName = checkin.ChangesetOwner.UniqueName,
                DisplayName = checkin.ChangesetOwner.DisplayName,
                ChangesetUrl = $"{baseUrl}_versionControl/changeset/{checkin.Changeset}",
                ChangesetId = checkin.Changeset,
                Projects = projects,
                Comment = TextHelper.Truncate(checkin.Comment, Settings.CommentMaxLength, true),
                TeamNames = teamNames,
                SubmittedItems = submittedItems
            };

            yield return notification;
        }

    }
}
