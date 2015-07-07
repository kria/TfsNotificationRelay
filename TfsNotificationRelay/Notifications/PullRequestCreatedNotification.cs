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
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.Notifications
{
    public class PullRequestCreatedNotification : BaseNotification
    {
        protected readonly static Configuration.SettingsElement settings = Configuration.TfsNotificationRelaySection.Instance.Settings;

        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string ProjectName { get; set; }
        public string RepoUri { get; set; }
        public string RepoName { get; set; }
        public int PrId { get; set; }
        public string PrUrl { get; set; }
        public string PrTitle { get; set; }

        public string UserName
        {
            get { return settings.StripUserDomain ? TextHelper.StripDomain(UniqueName) : UniqueName; }
        }

        public override IList<string> ToMessage(Configuration.BotElement bot, Func<string, string> transform)
        {
            var formatter = new
            {
                TeamProjectCollection = transform(this.TeamProjectCollection),
                DisplayName = transform(this.DisplayName),
                ProjectName = transform(this.ProjectName),
                RepoUri = this.RepoUri,
                RepoName = transform(this.RepoName),
                PrId = this.PrId,
                PrUrl = this.PrUrl,
                PrTitle = transform(this.PrTitle),
                UserName = transform(this.UserName),
            };

            return new[] { bot.Text.PullRequestCreatedFormat.FormatWith(formatter) };
        }

        public override EventRuleElement GetRuleMatch(string collection, Configuration.EventRuleCollection eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.PullRequestCreated)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject)
                && TeamNames.IsMatchOrNoPattern(r.TeamName)
                && RepoName.IsMatchOrNoPattern(r.GitRepository));

            return rule;
        }
    }
}
