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

using DevCore.TfsNotificationRelay.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.Notifications
{
    public class ProjectCreatedNotification : BaseNotification
    {
        protected static Configuration.SettingsElement settings = Configuration.TfsNotificationRelaySection.Instance.Settings;

        public string ProjectUrl { get; set; }
        public string ProjectName { get; set; }

        public override IList<string> ToMessage(Configuration.BotElement bot, Func<string, string> transform)
        {
            var formatter = new
            {
                TeamProjectCollection = transform(this.TeamProjectCollection),
                ProjectUrl = this.ProjectUrl,
                ProjectName = transform(this.ProjectName),
            };

            return new[] { bot.Text.ProjectCreatedFormat.FormatWith(formatter) };
        }

        public override EventRuleElement GetRuleMatch(string collection, Configuration.EventRuleCollection eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.ProjectCreated)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject));

            return rule;
        }
    }
}
