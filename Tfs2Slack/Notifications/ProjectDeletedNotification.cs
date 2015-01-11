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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.Notifications
{
    class ProjectDeletedNotification : BaseNotification
    {
        public string ProjectUri { get; set; }

        private string projectName;
        public string ProjectName {
            get { return projectName ?? ProjectUri; }
            set { projectName = value; }
        }

        public override IList<string> ToMessage(Configuration.BotElement bot)
        {
            return new[] { bot.Text.ProjectDeletedFormat.FormatWith(this) };
        }

        public override bool IsMatch(string collection, Configuration.EventRuleCollection eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.ProjectDeleted)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection));

            if (rule != null) return rule.Notify;

            return false;
        }
    }
}
