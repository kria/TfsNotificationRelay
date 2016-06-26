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
using Microsoft.TeamFoundation.Build.Server;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DevCore.TfsNotificationRelay.Notifications
{
    public abstract class BuildNotification : BaseNotification
    {
        protected static SettingsElement Settings = TfsNotificationRelaySection.Instance.Settings;

        public string ProjectName { get; set; }
        public string BuildDefinition { get; set; }
        public string DropLocation { get; set; }
        public BuildStatus BuildStatus { get; set; }
        public string BuildUrl { get; set; }
        public string BuildNumber { get; set; }
        public BuildReason BuildReason { get; set; }
        public string RequestedForUniqueName { get; set; }
        public string RequestedForDisplayName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public string UserName => Settings.StripUserDomain ? TextHelper.StripDomain(RequestedForUniqueName) : RequestedForUniqueName;

        public override IEnumerable<string> TargetUserNames => new[] { RequestedForUniqueName };
        public string DisplayName => RequestedForDisplayName;

        protected string FormatBuildDuration(TextElement text)
        {
            var duration = FinishTime - StartTime;
            return string.IsNullOrEmpty(text.TimeSpanFormat) ? duration.ToString(@"hh\:mm\:ss") : duration.ToString(text.TimeSpanFormat);
        }

        protected virtual string GetBuildFormat(TextElement text)
        {
            return text.BuildFormat;
        }

        public bool IsSuccessful => BuildStatus.HasFlag(BuildStatus.Succeeded);

        public override IList<string> ToMessage(BotElement bot, TextElement text, Func<string, string> transform)
        {
            var formatter = new
            {
                TeamProjectCollection = transform(TeamProjectCollection),
                ProjectName = transform(ProjectName),
                BuildDefinition = transform(BuildDefinition),
                BuildStatus = transform(BuildStatus.ToString()),
                BuildUrl,
                BuildNumber = transform(BuildNumber),
                BuildReason = transform(BuildReason.ToString()),
                RequestedFor = transform(RequestedForUniqueName),
                RequestedForDisplayName = transform(RequestedForDisplayName),
                DisplayName = transform(RequestedForDisplayName),
                StartTime,
                FinishTime,
                UserName = transform(UserName),
                BuildDuration = FormatBuildDuration(text),
                DropLocation,
                MappedUser = bot.GetMappedUser(RequestedForUniqueName)
            };
            return new[] { GetBuildFormat(text).FormatWith(formatter), transform(BuildStatus.ToString()) };
        }

        public override EventRuleElement GetRuleMatch(string collection, IEnumerable<EventRuleElement> eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.BuildCompleted)
                && (r.BuildStatuses & BuildStatus) != 0
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject)
                && TeamNames.IsMatchOrNoPattern(r.TeamName)
                && BuildDefinition.IsMatchOrNoPattern(r.BuildDefinition));

            return rule;
        }
    }
}
