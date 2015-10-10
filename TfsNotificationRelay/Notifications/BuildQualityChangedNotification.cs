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

        public override IList<string> ToMessage(BotElement bot, Func<string, string> transform)
        {
            var formatter = new
            {
                TeamProjectCollection = transform(TeamProjectCollection),
                ProjectName = transform(ProjectName),
                BuildDefinition = transform(BuildDefinition),
                BuildStatus = transform(BuildStatus.ToString()), BuildUrl,
                BuildNumber = transform(BuildNumber),
                BuildReason = transform(BuildReason.ToString()),
                RequestedFor = transform(RequestedFor),
                RequestedForDisplayName = transform(RequestedForDisplayName),
                DisplayName = transform(RequestedForDisplayName),
                StartTime,
                FinishTime,
                UserName = transform(UserName),
                BuildDuration = FormatBuildDuration(bot),
                DropLocation,
                NewValue = NewValue == null ? bot.Text.BuildQualityNotSet : transform(NewValue),
                OldValue = OldValue == null ? bot.Text.BuildQualityNotSet : transform(OldValue)
            };
            return new[] { bot.Text.BuildQualityChangedFormat.FormatWith(formatter) };
        }

        public override EventRuleElement GetRuleMatch(string collection, IEnumerable<EventRuleElement> eventRules)
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
