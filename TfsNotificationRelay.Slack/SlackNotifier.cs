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

using DevCore.TfsNotificationRelay;
using DevCore.TfsNotificationRelay.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevCore.TfsNotificationRelay.Configuration;
using DevCore.TfsNotificationRelay.Notifications.GitPush;

namespace DevCore.TfsNotificationRelay.Slack
{
    public class SlackNotifier : INotifier
    {
        public void Notify(INotification notification, BotElement bot)
        {
            var channels = bot.GetSetting("channels").Split(',').Select(chan => chan.Trim());

            foreach (string channel in channels)
            {
                var slackMessage = ToSlackMessage((dynamic)notification, bot, channel);
                if (slackMessage != null)
                    SendToSlack(slackMessage, bot.GetSetting("webhookUrl"));
            }
        }

        private async void SendToSlack(Slack.Message message, string webhookUrl)
        {
            using (var slackClient = new SlackClient())
            {
                var result = await slackClient.SendMessageAsync(message, webhookUrl);
                result.EnsureSuccessStatusCode();
            }
        }

        public Message ToSlackMessage(INotification notification, BotElement bot, string channel)
        {
            var lines = notification.ToMessage(bot);

            return SlackHelper.CreateSlackMessage(lines, bot, channel, bot.GetSetting("standardColor"));
        }

        public Message ToSlackMessage(BuildCompletionNotification notification, BotElement bot, string channel)
        {
            var lines = notification.ToMessage(bot);
            var color = notification.IsSuccessful ? bot.GetSetting("successColor") : bot.GetSetting("errorColor");

            return SlackHelper.CreateSlackMessage(lines, bot, channel, color);
        }

        public Message ToSlackMessage(WorkItemChangedNotification notification, BotElement bot, string channel)
        {
            string header = notification.ToMessage(bot).First();
            var fields = new[] { 
                new AttachmentField(bot.Text.State, notification.State, true), 
                new AttachmentField(bot.Text.AssignedTo, notification.AssignedTo, true) 
            };

            return SlackHelper.CreateSlackMessage(header, fields, bot, channel, bot.GetSetting("standardColor"));
        }


    }
}
