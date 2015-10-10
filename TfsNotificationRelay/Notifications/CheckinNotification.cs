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
    public class CheckinNotification : BaseNotification
    {
        protected static SettingsElement Settings = TfsNotificationRelaySection.Instance.Settings;

        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string ChangesetUrl { get; set; }
        public int ChangesetId { get; set; }
        public Dictionary<string, string> Projects { get; set; }
        public string Comment { get; set; }
        public IEnumerable<string> SubmittedItems { get; set; }

        private string FormatProjectLinks(BotElement bot, Func<string, string> transform)
        {
            return string.Join(", ", Projects.Select(x => bot.Text.ProjectLinkFormat
                .FormatWith(new { ProjectName = transform(x.Key), ProjectUrl = x.Value })));
        }
        public string UserName => Settings.StripUserDomain ? TextHelper.StripDomain(UniqueName) : UniqueName;

        public override IList<string> ToMessage(BotElement bot, Func<string, string> transform)
        {
            var formatter = new
            {
                TeamProjectCollection = transform(TeamProjectCollection),
                DisplayName = transform(DisplayName),
                ChangesetUrl,
                ChangesetId,
                Comment = transform(Comment),
                UserName = transform(UserName),
                ProjectLinks = FormatProjectLinks(bot, transform)
            };
            return new[] { bot.Text.CheckinFormat.FormatWith(formatter) };
        }

        public override EventRuleElement GetRuleMatch(string collection, IEnumerable<EventRuleElement> eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.Checkin)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && Projects.Keys.IsMatchOrNoPattern(r.TeamProject)
                && TeamNames.IsMatchOrNoPattern(r.TeamName)
                && SubmittedItems.IsMatchOrNoPattern(r.SourcePath));

            return rule;
        }

    }



}

