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
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace DevCore.Tfs2Slack.EventHandlers
{
    class CheckinHandler : BaseHandler
    {
        protected override IList<string> _ProcessEvent(TeamFoundationRequestContext requestContext, object notificationEventArgs, Configuration.BotElement bot)
        {
            var checkin = (CheckinNotification)notificationEventArgs;

            string userName = checkin.ChangesetOwner.UniqueName;
            if (settings.StripUserDomain) userName = Utils.StripDomain(userName);

            var locationService = requestContext.GetService<TeamFoundationLocationService>();

            string baseUrl = String.Format("{0}/{1}/",
                    locationService.GetAccessMapping(requestContext, "PublicAccessMapping").AccessPoint,
                    requestContext.ServiceHost.Name);

            string changesetUrl = String.Format("{0}_versionControl/changeset/{1}",
                    baseUrl, checkin.Changeset);
            
            string pattern = @"^\$\/([^\/]*)\/";
            var projectLinks = new Dictionary<string, string>();
            foreach (string item in checkin.GetSubmittedItems(requestContext))
            {
                Match match = Regex.Match(item, pattern);
                if (match.Success)
                {
                    string projectName = match.Groups[1].Value;
                    if (projectLinks.ContainsKey(projectName)) continue;
                    string projectUrl = baseUrl + projectName;
                    projectLinks.Add(projectName, String.Format("<{0}|{1}>", projectUrl, projectName));
                }
            }

            if (!IsNotificationMatch(checkin, bot, projectLinks.Keys.ToList())) return null;

            string message = text.CheckinFormat.FormatWith(new { 
                UserName = userName, 
                ChangesetUrl = changesetUrl, 
                ChangesetId = checkin.Changeset, 
                ProjectLinks = String.Join(", ", projectLinks.Values.Select(x => x)),
                Comment = checkin.Comment
            });

            return new string[] { message };   
        }

        public bool IsNotificationMatch(CheckinNotification checkin, Configuration.BotElement bot, IEnumerable<string> projectNames)
        {
            var rule = bot.EventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.Checkin) 
                && (String.IsNullOrEmpty(r.TeamProject) || projectNames.Any(n => Regex.IsMatch(n, r.TeamProject))));

            if (rule != null) return rule.Notify;

            return false;
        }
    }
}
