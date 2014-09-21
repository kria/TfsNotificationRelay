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

namespace DevCore.Tfs2Slack.Slack
{
    class SlackHelper
    {
        public static Slack.Message CreateSlackMessage(IEnumerable<string> lines, Configuration.BotElement bot, string channel, string color) 
        {
            if (lines == null || lines.Count() == 0) return null;

            var message = new Slack.Message()
            {
                Channel = channel,
                Username = bot.SlackUsername,
                Attachments = new[] { 
                    new Attachment() {
                        Fallback = lines.First(),
                        Pretext = lines.First(),
                        Color = color,
                        Fields = from line in lines.Skip(1) select new AttachmentField() { Value = line, IsShort = false } 
                    }
                }
            };
            if (!String.IsNullOrEmpty(bot.SlackIconUrl))
                message.IconUrl = bot.SlackIconUrl;
            else if (!String.IsNullOrEmpty(bot.SlackIconEmoji))
                message.IconEmoji = bot.SlackIconEmoji;

            return message;
        }
    }
}
