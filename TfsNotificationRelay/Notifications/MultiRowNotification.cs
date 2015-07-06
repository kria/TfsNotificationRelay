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
    public abstract class MultiRowNotification : List<NotificationRow>, INotification
    {
        public string TeamProjectCollection { get; set; }

        public int TotalLineCount { get; set; }

        public IList<string> ToMessage(Configuration.BotElement bot, Func<string, string> transform)
        {
            var lines = this.Select(r => r.ToString(bot, transform)).ToList();
            if (lines != null && lines.Count > 0)
            {
                if (lines.Count < TotalLineCount)
                {
                    lines.Add(bot.Text.LinesSupressedFormat.FormatWith(new { Count = TotalLineCount - lines.Count }));
                }
            }

            return lines;
        }

        public abstract EventRuleElement GetRuleMatch(string collection, Configuration.EventRuleCollection eventRules);
    }
}
