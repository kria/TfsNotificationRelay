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

namespace DevCore.TfsNotificationRelay.Notifications
{
    public class WorkItemCommentNotification : WorkItemNotification, ICommentNotification
    {
        public string Comment { get; set; }
        public string CommentHtml { get; set; }

        public override IList<string> ToMessage(BotElement bot, Func<string, string> transform)
        {
            var lines = new List<string>();
            var formatter = new
            {
                TeamProjectCollection = transform(TeamProjectCollection),
                DisplayName = transform(DisplayName),
                ProjectName = transform(ProjectName),
                AreaPath = transform(AreaPath),
                WiUrl,
                WiType = transform(WiType),
                WiId,
                WiTitle = transform(WiTitle),
                UserName = transform(UserName),
                Action = bot.Text.CommentedOn,
                AssignedToUserName = transform(AssignedToUserName),
                MappedAssignedToUser = bot.GetMappedUser(AssignedToUniqueName),
                MappedUser = bot.GetMappedUser(UniqueName)
            };
            lines.Add(bot.Text.WorkItemchangedFormat.FormatWith(formatter));
            lines.Add(TextHelper.Truncate(Comment, Settings.DiscussionCommentMaxLength));

            return lines;
        }

        public override EventRuleElement GetRuleMatch(string collection, IEnumerable<EventRuleElement> eventRules)
        {
            var rule = eventRules.FirstOrDefault(r =>
                r.Events.HasFlag(TfsEvents.WorkItemComment)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject)
                && TeamNames.IsMatchOrNoPattern(r.TeamName)
                && WiType.IsMatchOrNoPattern(r.WorkItemType)
                && AreaPath.IsMatchOrNoPattern(r.AreaPath));

            return rule;
        }

    }
}
