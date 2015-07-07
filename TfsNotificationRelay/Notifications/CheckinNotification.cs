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
        protected static Configuration.SettingsElement settings = Configuration.TfsNotificationRelaySection.Instance.Settings;

        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string ChangesetUrl { get; set; }
        public int ChangesetId { get; set; }
        public Dictionary<string, string> Projects { get; set; }
        public string Comment { get; set; }
        public IEnumerable<string> SubmittedItems { get; set; }

        private string FormatProjectLinks(Configuration.BotElement bot, Func<string, string> transform)
        {
            return String.Join(", ", Projects.Select(x => bot.Text.ProjectLinkFormat
                .FormatWith(new { ProjectName = transform(x.Key), ProjectUrl = x.Value })));
        }
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
                ChangesetUrl = this.ChangesetUrl,
                ChangesetId = this.ChangesetId,
                Comment = transform(this.Comment),
                UserName = transform(this.UserName),
                ProjectLinks = FormatProjectLinks(bot, transform)
            };
            return new[] { bot.Text.CheckinFormat.FormatWith(formatter) };
        }

        public override EventRuleElement GetRuleMatch(string collection, Configuration.EventRuleCollection eventRules)
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

