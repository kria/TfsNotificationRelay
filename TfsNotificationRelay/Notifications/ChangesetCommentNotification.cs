/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2015 Kristian Adrup
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
    public class ChangesetCommentNotification : BaseNotification, ICommentNotification
    {
        protected static SettingsElement Settings = TfsNotificationRelaySection.Instance.Settings;

        public string CreatorUniqueName { get; set; }
        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string ChangesetUrl { get; set; }
        public int ChangesetId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectUrl { get; set; }
        public string Comment { get; set; }
        public string SourcePath { get; set; }

        public string UserName => Settings.StripUserDomain ? TextHelper.StripDomain(UniqueName) : UniqueName;

        public string CreatorUserName => Settings.StripUserDomain ? TextHelper.StripDomain(CreatorUniqueName) : CreatorUniqueName;

        public override IList<string> ToMessage(BotElement bot, TextElement text, Func<string, string> transform)
        {
            var formatter = new
            {
                TeamProjectCollection = transform(TeamProjectCollection),
                DisplayName = transform(DisplayName),
                ChangesetUrl,
                ChangesetId,
                Comment = transform(Comment),
                UserName = transform(UserName),
                ProjectName = transform(ProjectName),
                ProjectUrl,
                CreatorUserName = transform(CreatorUserName),
                MappedCreatorUser = bot.GetMappedUser(CreatorUniqueName),
                MappedUser = bot.GetMappedUser(UniqueName)
            };

            return new[] { text.ChangesetCommentFormat.FormatWith(formatter), Comment };
        }

        public override EventRuleElement GetRuleMatch(string collection, IEnumerable<EventRuleElement> eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.ChangesetComment)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject)
                && TeamNames.IsMatchOrNoPattern(r.TeamName)
                && SourcePath.IsMatchOrNoPattern(r.SourcePath)
                && Comment.IsMatchOrNoPattern(r.Text));

            return rule;
        }

    }



}

