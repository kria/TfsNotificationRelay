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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevCore.TfsNotificationRelay.Configuration;

namespace DevCore.TfsNotificationRelay.Slack
{
    class SlackHelper
    {
        public static Slack.Message CreateSlackMessage(IEnumerable<string> lines, BotElement bot, string channel, string color) 
        {
            if (lines == null || lines.Count() == 0) return null;

            string header = lines.First();
            var fields = from line in lines.Skip(1) select new AttachmentField() { Value = line, IsShort = false };

            return CreateSlackMessage(header, fields, bot, channel, color);
        }

        public static Slack.Message CreateSlackMessage(string header, IEnumerable<AttachmentField> fields, BotElement bot, string channel, string color)
        {
            if (header == null) return null;

            var message = new Slack.Message()
            {
                Channel = channel,
                Username = bot.GetSetting("username"),
                Attachments = new[] { 
                    new Attachment() {
                        Fallback = header,
                        Pretext = header,
                        Color = color,
                        Fields = fields
                    }
                }
            };
            if (!String.IsNullOrEmpty(bot.GetSetting("iconUrl")))
                message.IconUrl = bot.GetSetting("iconUrl");
            else if (!String.IsNullOrEmpty(bot.GetSetting("iconEmoji")))
                message.IconEmoji = bot.GetSetting("iconEmoji");

            return message;
        }
    }
}
