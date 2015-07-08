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

namespace DevCore.TfsNotificationRelay.Notifications
{
    public class BuildQualityChangedNotification : BuildNotification
    {
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public override IList<string> ToMessage(Configuration.BotElement bot, Func<string, string> transform)
        {
            var formatter = new
            {
                TeamProjectCollection = transform(this.TeamProjectCollection),
                ProjectName = transform(this.ProjectName),
                BuildDefinition = transform(this.BuildDefinition),
                BuildStatus = transform(this.BuildStatus.ToString()),
                BuildUrl = this.BuildUrl,
                BuildNumber = transform(this.BuildNumber),
                BuildReason = transform(this.BuildReason.ToString()),
                RequestedFor = transform(this.RequestedFor),
                RequestedForDisplayName = transform(this.RequestedForDisplayName),
                DisplayName = transform(this.RequestedForDisplayName),
                StartTime = this.StartTime,
                FinishTime = this.FinishTime,
                UserName = transform(this.UserName),
                BuildDuration = FormatBuildDuration(bot),
                DropLocation = this.DropLocation,
                NewValue = this.NewValue == null ? bot.Text.BuildQualityNotSet : transform(this.NewValue),
                OldValue = this.OldValue == null ? bot.Text.BuildQualityNotSet : transform(this.OldValue)
            };
            return new[] { bot.Text.BuildQualityChangedFormat.FormatWith(formatter) };
        }

        public override EventRuleElement GetRuleMatch(string collection, Configuration.EventRuleCollection eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.BuildQualityChanged)
                && (r.BuildStatuses & BuildStatus) != 0
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject)
                && TeamNames.IsMatchOrNoPattern(r.TeamName)
                && BuildDefinition.IsMatchOrNoPattern(r.BuildDefinition));

            return rule;
        }
    }
}
