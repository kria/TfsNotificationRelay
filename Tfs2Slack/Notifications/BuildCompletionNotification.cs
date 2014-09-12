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

using Microsoft.TeamFoundation.Build.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.Notifications
{
    class BuildCompletionNotification : BaseNotification
    {
        protected static Configuration.TextElement text = Configuration.Tfs2SlackSection.Instance.Text;

        public string ProjectName { get; set; }
        public string BuildDefinition { get; set; }
        public BuildStatus BuildStatuses { get; set; }
        public string BuildUrl { get; set; }
        public string BuildNumber { get; set; }

        public override IList<string> ToMessage(Configuration.BotElement bot)
        {
            return new[] { text.BuildFormat.FormatWith(this) };
        }

        public override bool IsMatch(Configuration.EventRuleCollection eventRules)
        {
            foreach (var rule in eventRules)
            {
                if (BuildStatuses.HasFlag(BuildStatus.Succeeded) && rule.Events.HasFlag(TfsEvents.BuildSucceeded)
                    || BuildStatuses.HasFlag(BuildStatus.Failed) && rule.Events.HasFlag(TfsEvents.BuildFailed))
                {
                    if ((String.IsNullOrEmpty(rule.TeamProject) || Regex.IsMatch(ProjectName, rule.TeamProject))
                        && (String.IsNullOrEmpty(rule.BuildDefinition) || Regex.IsMatch(BuildNumber, rule.BuildDefinition)))
                    {
                        return rule.Notify;
                    }
                }
            }
            return false;
        }
    }
}
