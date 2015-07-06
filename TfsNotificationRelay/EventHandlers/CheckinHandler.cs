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
    using System.Diagnostics;

    using Microsoft.TeamFoundation.Framework.Common;
    using Microsoft.TeamFoundation.Integration.Server;
    using Microsoft.TeamFoundation.Server.Core;

    class CheckinHandler : BaseHandler<TFVC.CheckinNotification>
    {
        protected override IEnumerable<INotification> CreateNotifications(TeamFoundationRequestContext requestContext, TFVC.CheckinNotification checkin, int maxLines)
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
                Comment = checkin.Comment,
                Teams = new Dictionary<string, string>()
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

                    // Add the teams to the notification
                    foreach (var team in this.GetTeamsByUser(requestContext, projectName, notification.DisplayName))
                    {
                        if (notification.Teams.ContainsKey(team.Key))
                        {
                            continue;
                        }    
                        notification.Teams.Add(team.Key, team.Value);
                    }
                    
                }
            }

            yield return notification;   
        }

        /// <summary>
        /// Gets the team names by project name and user display name
        /// Could be moved into base class?
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="projectName">Name of the team project</param>
        /// <param name="displayName">Name of the user</param>
        /// <returns>A dictionary of teams the user is a member of</returns>
        private Dictionary<string, string> GetTeamsByUser(TeamFoundationRequestContext requestContext, string projectName, string displayName)
        {
            var teamList = new Dictionary<string, string>();

            var teamService = requestContext.GetService<TeamFoundationTeamService>();
            this.Trace(requestContext, "Getting Project by name");

            // Get the project URI
            var project = this.ProjectsNames.Where(p => p.Value.Equals(projectName, StringComparison.InvariantCultureIgnoreCase))
                              .Select(p => new { Uri = p.Key, Name = p.Value })
                              .FirstOrDefault();

            if (project == null)
            {
                this.Trace(requestContext, "Failed to locate project");
                return teamList;
            }

            this.Trace(requestContext, "Looking for teams in {0}", projectName);

            // Get the teams in the project for this checkin
            // Todo: Cache these?
            var allTeams = teamService.QueryTeams(requestContext, project.Uri);

            if (allTeams != null)
            {
                var allTeamsList = allTeams as IList<TeamFoundationTeam> ?? allTeams.ToList();

                this.Trace(requestContext, "Found teams [{0}]", allTeamsList.Count());

                foreach (var team in allTeamsList)
                {
                    this.Trace(requestContext, "Getting Team Members in team [{0}]", team.Name);
                    var members = teamService.ReadTeamMembers(requestContext, team, MembershipQuery.Direct, ReadIdentityOptions.None, null);

                    this.Trace(requestContext, "Found Team Members [{0}]", members.Count());

                    foreach (var member in members)
                    {
                        this.Trace(requestContext, "Comparing [{0}] to [{1}]", member.DisplayName, displayName);
                        if (member.DisplayName.Equals(displayName))
                        {
                            this.Trace(requestContext, "Match");
                            teamList.Add(team.Name, team.Identity.TeamFoundationId.ToString());
                        }
                    }
                }
            }
            else
            {
                this.Trace(requestContext, "No Teams found");
            }

            return teamList;
        }

        /// <summary>
        /// Shortcut for tracing - could be refactored further
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        private void Trace(TeamFoundationRequestContext requestContext, string message, params object[] args)
        {
            requestContext.Trace(0, TraceLevel.Verbose, Constants.TraceArea, this.GetType().Name, message, args);
        }
    }
}
