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
    public class WorkItemCommentNotification : WorkItemNotification
    {
        public string Comment { get; set; }
        public string CommentHtml { get; set; }

        public override IList<string> ToMessage(Configuration.BotElement bot, Func<string, string> transform)
        {
            var lines = new List<string>();
            var formatter = new
            {
                TeamProjectCollection = transform(this.TeamProjectCollection),
                DisplayName = transform(this.DisplayName),
                ProjectName = transform(this.ProjectName),
                AreaPath = transform(this.AreaPath),
                WiUrl = this.WiUrl,
                WiType = transform(this.WiType),
                WiId = this.WiId,
                WiTitle = transform(this.WiTitle),
                UserName = transform(this.UserName),
                Action = bot.Text.CommentedOn
            };
            lines.Add(bot.Text.WorkItemchangedFormat.FormatWith(formatter));

            lines.Add(Comment);

            return lines;
        }

        public override EventRuleElement GetRuleMatch(string collection, Configuration.EventRuleCollection eventRules)
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
