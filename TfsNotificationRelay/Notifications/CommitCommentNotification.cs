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
using Microsoft.TeamFoundation.Git.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.Notifications
{
    public class CommitCommentNotification : BaseNotification, ICommentNotification
    {
        protected readonly static SettingsElement Settings = TfsNotificationRelaySection.Instance.Settings;

        public string PusherUniqueName { get; set; }
        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string ProjectName { get; set; }
        public string RepoUri { get; set; }
        public string RepoName { get; set; }
        public Sha1Id CommitId { get; set; }
        public string CommitUri { get; set; }
        public string Comment { get; set; }
        public string UserName => Settings.StripUserDomain ? TextHelper.StripDomain(UniqueName) : UniqueName;
        public string PusherUserName => Settings.StripUserDomain ? TextHelper.StripDomain(PusherUniqueName) : PusherUniqueName;

        public override IList<string> ToMessage(BotElement bot, TextElement text, Func<string, string> transform)
        {
            var formatter = new
            {
                TeamProjectCollection = transform(TeamProjectCollection),
                DisplayName = transform(DisplayName),
                ProjectName = transform(ProjectName),
                RepoUri,
                RepoName = transform(RepoName),
                CommitId = transform(CommitId.ToHexString(Settings.HashLength)),
                CommitUri,
                Comment = transform(Comment),
                UserName = transform(UserName),
                PusherUserName = transform(PusherUserName),
                MappedPusherUser = bot.GetMappedUser(PusherUniqueName),
                MappedUser = bot.GetMappedUser(UniqueName)
            };

            return new[] { text.CommitCommentFormat.FormatWith(formatter), Comment };
        }

        public override IEnumerable<string> TargetUserNames =>
            string.IsNullOrEmpty(PusherUniqueName) && PusherUniqueName != UniqueName ? new[] { PusherUniqueName } : Enumerable.Empty<string>();

        public override EventRuleElement GetRuleMatch(string collection, IEnumerable<EventRuleElement> eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.GitCommitComment)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject)
                && RepoName.IsMatchOrNoPattern(r.GitRepository)
                && TeamNames.IsMatchOrNoPattern(r.TeamName));

            return rule;
        }
    }


}
