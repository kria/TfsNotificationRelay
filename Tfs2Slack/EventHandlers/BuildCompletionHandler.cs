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
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DevCore.Tfs2Slack.EventHandlers
{
    class BuildCompletionHandler : BaseHandler
    {
        protected override IList<string> _ProcessEvent(TeamFoundationRequestContext requestContext, object notificationEventArgs, Configuration.BotElement bot)
        {
            var buildNotification = (BuildCompletionNotificationEvent)notificationEventArgs;
            var build = buildNotification.Build;
            
            var lines = new List<string>();

            if (IsNotificationMatch(build, bot))
            {
                var locationService = requestContext.GetService<TeamFoundationLocationService>();

                string buildUrl = String.Format("{0}/{1}/{2}/_build#buildUri={3}&_a=summary",
                    locationService.GetAccessMapping(requestContext, "PublicAccessMapping").AccessPoint,
                    requestContext.ServiceHost.Name,
                    build.TeamProject,
                    build.Uri);
                
                lines.Add(text.BuildFormat.FormatWith(new
                {
                    BuildUrl = buildUrl,
                    ProjectName = build.TeamProject,
                    BuildNumber = build.BuildNumber,
                    BuildStatuses = build.Status
                }));
            }

            return lines;
        }

        private bool IsNotificationMatch(BuildDetail build, Configuration.BotElement bot)
        {
            foreach (var rule in bot.EventRules)
            {
                if (build.Status.HasFlag(BuildStatus.Succeeded) && rule.Events.HasFlag(TfsEvents.BuildSucceeded)
                    || build.Status.HasFlag(BuildStatus.Failed) && rule.Events.HasFlag(TfsEvents.BuildFailed))
                {
                    if ((String.IsNullOrEmpty(rule.TeamProject) || Regex.IsMatch(build.TeamProject, rule.TeamProject))
                        && (String.IsNullOrEmpty(rule.BuildDefinition) || Regex.IsMatch(build.BuildNumber, rule.BuildDefinition)))
                    {
                        return rule.Notify;
                    }
                }
            }
            return false;
        }

    }
}
