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
        protected static Configuration.SettingsElement settings = Configuration.TfsNotificationRelaySection.Instance.Settings;

        public string ProjectName { get; set; }
        public string BuildDefinition { get; set; }
        public string DropLocation { get; set; }
        public BuildStatus BuildStatus { get; set; }
        public string BuildUrl { get; set; }
        public string BuildNumber { get; set; }
        public BuildReason BuildReason { get; set; }
        public string RequestedFor { get; set; }
        public string RequestedForDisplayName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public string UserName 
        {
            get { return settings.StripUserDomain ? TextHelper.StripDomain(RequestedFor) : RequestedFor; }
        }
        public override IEnumerable<string> TargetUserNames
        {
            get { return new[] { RequestedFor }; }
        }
        public string DisplayName
        {
            get { return RequestedForDisplayName; }
        }
        protected string FormatBuildDuration(Configuration.BotElement bot)
        {
            var duration = FinishTime - StartTime;
            return String.IsNullOrEmpty(bot.Text.TimeSpanFormat) ? duration.ToString(@"hh\:mm\:ss") : duration.ToString(bot.Text.TimeSpanFormat);
        }

        public bool IsSuccessful
        {
            get { return BuildStatus.HasFlag(BuildStatus.Succeeded); }
        }

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
                DropLocation = this.DropLocation
            };
            return new[] { bot.Text.BuildFormat.FormatWith(formatter), transform(BuildStatus.ToString()) };
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
