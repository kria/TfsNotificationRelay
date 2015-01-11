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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.Notifications
{
    public abstract class MultiRowNotification : List<NotificationRow>, INotification
    {
        public string TeamProjectCollection { get; set; }

        public int TotalLineCount { get; set; }

        public IList<string> ToMessage(Configuration.BotElement bot)
        {
            var lines = this.Select(r => r.ToString(bot)).ToList();
            if (lines != null && lines.Count > 0)
            {
                if (lines.Count < TotalLineCount)
                {
                    lines.Add(bot.Text.LinesSupressedFormat.FormatWith(new { Count = TotalLineCount - lines.Count }));
                }
            }

            return lines;
        }

        public abstract bool IsMatch(string collection, Configuration.EventRuleCollection eventRules);
    }
}
