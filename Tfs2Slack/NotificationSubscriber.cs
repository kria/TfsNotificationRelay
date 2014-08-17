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

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Integration.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.TeamFoundation.Build.Server;

namespace DevCore.Tfs2Slack
{
    class NotificationSubscriber : ISubscriber
    {
        private Properties.Settings settings = Properties.Settings.Default;
        private Properties.Text text = Properties.Text.Default;

        public string Name
        {
            get { return "Tfs2Slack handler"; }
        }

        public SubscriberPriority Priority
        {
            get { return SubscriberPriority.Normal; }
        }

        public Type[] SubscribedTypes()
        {
            return new Type[] { typeof(PushNotification), typeof(BuildCompletionNotificationEvent) };
        }

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
                Log(String.Format("notificationType={0}, notificationEventArgs={1}", notificationType, notificationEventArgs));

                List<string> lines = null;

                if (notificationType == NotificationType.Notification && notificationEventArgs is PushNotification)
                {
                    lines = PushHandler.CreateMessage(requestContext, notificationEventArgs as PushNotification);
                }
                else if (notificationType == NotificationType.Notification && notificationEventArgs is BuildCompletionNotificationEvent)
                {
                    lines = BuildHandler.CreateMessage(requestContext, notificationEventArgs as BuildCompletionNotificationEvent); 
                }

                if (lines != null && lines.Count > 0)
                {
                    //Log(lines);
                    List<string> sendLines = lines;
                    if (lines != null && lines.Count > settings.MaxLines)
                    {
                        sendLines = lines.Take(settings.MaxLines).ToList();
                        sendLines.Add(text.FormatLinesSupressedText(lines.Count - settings.MaxLines));
                    }

                    Task.Run(() => SendToSlack(sendLines));
                }
                
            }
            catch (Exception ex)
            {
                Log(ex);
            }
            finally
            {
                timer.Stop();
                Log("Time spent in ProcessEvent: " + timer.Elapsed);
            }

            return EventNotificationStatus.ActionPermitted;
        }

        private void SendToSlack(IEnumerable<string> lines)
        {
            try
            {
                dynamic json = JObject.FromObject(new
                {
                    channel = settings.SlackChannel,
                    username = settings.SlackUsername,
                    attachments = new[] {
                        new {
                            fallback = lines.First(),
                            pretext = lines.First(),
                            color = settings.SlackColor,
                            mrkdwn_in = new [] { "pretext", "text", "title", "fields", "fallback" },
                            fields = from line in lines.Skip(1) select new { value = line, @short = false }
                        }
                    }
                });
                if (!String.IsNullOrEmpty(settings.SlackIconUrl))
                    json.icon_url = settings.SlackIconUrl;
                else if (!String.IsNullOrEmpty(settings.SlackIconEmoji))
                    json.icon_emoji = settings.SlackIconEmoji;

                Log(json.ToString());

                using (var client = new HttpClient())
                {
                    var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                    var result = client.PostAsync(settings.SlackWebhookUrl, content).Result;
                    result.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        private void Log(IEnumerable<string> lines)
        {
            if (String.IsNullOrEmpty(settings.Logfile)) return;

            using (StreamWriter sw = File.AppendText(settings.Logfile))
            {
                foreach (string line in lines)
                {
                    sw.WriteLine("[{0}] {1}", DateTime.Now, line);
                }
            }
        }
        private void Log(string line)
        {
            Log(new[] { line });
        }
        private void Log(Exception ex)
        {
            Log(ex.ToString());
        }

        
    }

}
