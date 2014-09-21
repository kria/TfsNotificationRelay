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

using DevCore.Tfs2Slack.Slack;
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
        protected static Configuration.SettingsElement settings = Configuration.Tfs2SlackSection.Instance.Settings;

        public string ProjectName { get; set; }
        public string BuildDefinition { get; set; }
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
            get { return settings.StripUserDomain ? Utils.StripDomain(RequestedFor) : RequestedFor; }
        }
        public string DisplayName
        {
            get { return RequestedForDisplayName; }
        }
        public string BuildDuration
        {
            get
            {
                var duration = FinishTime - StartTime;
                return String.IsNullOrEmpty(text.TimeSpanFormat) ? duration.ToString(@"hh\:mm\:ss") : duration.ToString(text.TimeSpanFormat);
            }
        }

        public override IList<string> ToMessage(Configuration.BotElement bot)
        {
            return new[] { text.BuildFormat.FormatWith(this), BuildStatus.ToString() };
        }

        public override Slack.Message ToSlackMessage(Configuration.BotElement bot, string channel)
        {
            var lines = ToMessage(bot);
            var color = BuildStatus.HasFlag(BuildStatus.Succeeded) ? bot.SuccessColor : bot.ErrorColor;
            return SlackHelper.CreateSlackMessage(lines, bot, channel, color);
        }

        public override bool IsMatch(string collection, Configuration.EventRuleCollection eventRules)
        {
            foreach (var rule in eventRules)
            {
                if (BuildStatus.HasFlag(BuildStatus.Succeeded) && rule.Events.HasFlag(TfsEvents.BuildSucceeded)
                    || BuildStatus.HasFlag(BuildStatus.Failed) && rule.Events.HasFlag(TfsEvents.BuildFailed))
                {
                    if (collection.IsMatchOrNoPattern(rule.TeamProjectCollection)
                        && ProjectName.IsMatchOrNoPattern(rule.TeamProject) 
                        && BuildNumber.IsMatchOrNoPattern(rule.BuildDefinition))
                    {
                        return rule.Notify;
                    }
                }
            }
            return false;
        }
    }
}
