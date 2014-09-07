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

using DevCore.Tfs2Slack.Configuration;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.EventHandlers
{
    class ProjectCreatedHandler : BaseHandler
    {
        protected override IList<string> _ProcessEvent(TeamFoundationRequestContext requestContext, object notificationEventArgs, Configuration.BotElement bot)
        {
            var ev = (ProjectCreatedEvent)notificationEventArgs;
            if (!IsNotificationMatch(bot, ev.Name)) return null;
            var locationService = requestContext.GetService<TeamFoundationLocationService>();

            string projectUrl = String.Format("{0}/{1}/{2}",
                locationService.GetAccessMapping(requestContext, "PublicAccessMapping").AccessPoint,
                requestContext.ServiceHost.Name,
                ev.Name);

            return new [] { text.ProjectCreatedFormat.FormatWith(new { ProjectUrl = projectUrl, ProjectName = ev.Name }) };
        }

        public bool IsNotificationMatch(Configuration.BotElement bot, string projectName)
        {
            var rule = bot.EventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.ProjectCreated)
                && (String.IsNullOrEmpty(r.TeamProject) || Regex.IsMatch(projectName, r.TeamProject)));

            if (rule != null) return rule.Notify;

            return false;
        }
    }
}
