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
    public abstract class RepositoryNotification : BaseNotification
    {
        protected static SettingsElement Settings = TfsNotificationRelaySection.Instance.Settings;

        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string ProjectName { get; set; }
        public string ProjectUrl { get; set; }
        public string RepoUri { get; set; }
        public string RepoName { get; set; }

        public string UserName => Settings.StripUserDomain ? TextHelper.StripDomain(UniqueName) : UniqueName;

        protected abstract string GetFormat(BotElement bot);

        public override IList<string> ToMessage(BotElement bot, Func<string, string> transform)
        {
            var formatter = new
            {
                TeamProjectCollection = transform(TeamProjectCollection),
                DisplayName = transform(DisplayName),
                UserName = transform(UserName),
                ProjectName = transform(ProjectName),
                ProjectUrl,
                RepoUri,
                RepoName = transform(RepoName),
                MappedUser = bot.GetMappedUser(UniqueName)
            };

            return new[] { GetFormat(bot).FormatWith(formatter) };
        }

        public IEnumerable<EventRuleElement> GetRulesMatch(string collection, IEnumerable<EventRuleElement> eventRules)
        {
            var rules = eventRules.Where(r => collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject)
                && TeamNames.IsMatchOrNoPattern(r.TeamName)
                && RepoName.IsMatchOrNoPattern(r.GitRepository));

            return rules;
        }

    }
}
