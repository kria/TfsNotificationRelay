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
using DevCore.TfsNotificationRelay.Notifications;
using DevCore.TfsNotificationRelay.Slack.Models;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace DevCore.TfsNotificationRelay.Slack
{
    public class DirectMessageNotifier : SlackNotifier
    {
        public override async Task NotifyAsync(IVssRequestContext requestContext, INotification notification, BotElement bot, EventRuleElement matchingRule)
        {
            var token = bot.GetSetting("token");
            if (string.IsNullOrEmpty(token)) throw new ArgumentException("Missing token!");

            var tasks = new List<Task>();
            var slackClient = new SlackClient();

            foreach (string tfsUserName in notification.TargetUserNames)
            {
                var userId = bot.GetMappedUser(tfsUserName);

                if (userId != null)
                {
                    Message slackMessage = ToSlackMessage((dynamic)notification, bot, null, true);
                    if (slackMessage != null)
                    {
                        slackMessage.AsUser = true;
                        var t = Task.Run(async () =>
                        {
                            var response = await slackClient.SendApiMessageAsync(slackMessage, token, userId);
                            response.EnsureSuccessStatusCode();
                            var content = await response.Content.ReadAsStringAsync();
                        });
                        tasks.Add(t);
                    }
                }
            }

            await Task.WhenAll(tasks);
        }

    }
}
