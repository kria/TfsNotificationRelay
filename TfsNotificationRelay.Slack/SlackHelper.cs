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
using DevCore.TfsNotificationRelay.Slack.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevCore.TfsNotificationRelay.Slack
{
    class SlackHelper
    {
        public static Message CreateSlackMessage(IEnumerable<string> lines, BotElement bot, string channel, string color, bool asUser) 
        {
            if (lines == null || lines.Count() == 0) return null;

            string header = lines.First();
            var fields = from line in lines.Skip(1) select new AttachmentField() { Value = line, IsShort = false };

            return CreateSlackMessage(header, fields, bot, channel, color, asUser);
        }

        public static Message CreateSlackMessage(string header, IEnumerable<AttachmentField> fields, BotElement bot, string channel, string color, bool asUser)
        {
            if (header == null) return null;
            IEnumerable<Attachment> attachments = null;
            if (fields.Any())
            {
                attachments = new[] {
                    new Attachment() {
                        Fallback = header,
                        Color = color,
                        Fields = fields
                    }
                };
            }
            var message = new Message()
            {
                Channel = channel,
                Text = header,
                Attachments = attachments
            };
            message.Text = header;

            if (!asUser)
            {
                message.Username = bot.GetSetting("username");
                if (!String.IsNullOrEmpty(bot.GetSetting("iconUrl")))
                    message.IconUrl = bot.GetSetting("iconUrl");
                else if (!String.IsNullOrEmpty(bot.GetSetting("iconEmoji")))
                    message.IconEmoji = bot.GetSetting("iconEmoji");
            }

            return message;
        }
    }
}
