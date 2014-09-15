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

using DevCore.Tfs2Slack.Notifications;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.EventHandlers
{
    public abstract class BaseHandler : ISubscriber
    {
        protected static Configuration.SettingsElement settings = Configuration.Tfs2SlackSection.Instance.Settings;
        protected static Configuration.TextElement text = Configuration.Tfs2SlackSection.Instance.Text;

        public string Name
        {
            get { return "Tfs2Slack handler"; }
        }

        public SubscriberPriority Priority
        {
            get { return SubscriberPriority.Normal; }
        }

        public abstract Type[] SubscribedTypes();

        protected abstract INotification CreateNotification(TeamFoundationRequestContext requestContext, object notificationEventArgs, int maxLines);

        public EventNotificationStatus ProcessEvent(TeamFoundationRequestContext requestContext, NotificationType notificationType,
            object notificationEventArgs, out int statusCode, out string statusMessage, out Microsoft.TeamFoundation.Common.ExceptionPropertyCollection properties)
        {
            statusCode = 0;
            statusMessage = string.Empty;
            properties = null;

            Stopwatch timer = new Stopwatch();
            timer.Start();

            try
            {
                Logger.Log("notificationType={0}, notificationEventArgs={1}", notificationType, notificationEventArgs);

                var config = Configuration.Tfs2SlackSection.Instance;                

                if (notificationType == NotificationType.Notification)
                {
                    var notification = CreateNotification(requestContext, notificationEventArgs, settings.MaxLines);

                    foreach (var bot in config.Bots)
                    {
                        if (!notification.IsMatch(requestContext.ServiceHost.Name, bot.EventRules)) continue;
                        
                        IList<string> lines = notification.ToMessage(bot);
                        if (lines != null && lines.Count > 0)
                        {
                            if (lines.Count < notification.TotalLineCount)
                            {
                                lines.Add(text.LinesSupressedFormat.FormatWith(new { Count = notification.TotalLineCount - lines.Count }));
                            }
                        }

                        string color = notification.Color ?? bot.SlackColor;
                        if (lines != null && lines.Count > 0)
                        {
                            var channels = bot.SlackChannels.Split(',')
                                .Select(chan => new Slack.PayloadSettings(bot.SlackWebhookUrl, chan.Trim(), bot.SlackUsername,
                                    bot.SlackIconEmoji, bot.SlackIconUrl, color));

                            foreach (var chan in channels)
                            {
                                SendToSlack(lines, chan);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            finally
            {
                timer.Stop();
                Logger.Log("Time spent in ProcessEvent: " + timer.Elapsed);
            }

            return EventNotificationStatus.ActionPermitted;
        }

        private async void SendToSlack(IEnumerable<string> lines, Slack.PayloadSettings settings)
        {
            try
            {
                using (var slackClient = new SlackClient())
                {
                    var result = await slackClient.SendMessageAsync(lines, settings);
                    result.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

        }

    }
}
