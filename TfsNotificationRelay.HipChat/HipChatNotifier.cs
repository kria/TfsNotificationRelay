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
using DevCore.TfsNotificationRelay.Configuration;
using DevCore.TfsNotificationRelay.Notifications;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.HipChat
{
    public class HipChatNotifier : INotifier
    {
        public async Task NotifyAsync(TeamFoundationRequestContext requestContext, INotification notification, BotElement bot)
        {
            string room = bot.GetSetting("room");
            string baseUrl = bot.GetSetting("apiBaseUrl");
            string authToken = bot.GetSetting("roomNotificationToken");
            string messageFormat = bot.GetSetting("messageFormat");

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            dynamic jobject = new JObject();
            jobject.message_format = messageFormat;
            jobject.color = bot.GetSetting("standardColor");
            jobject.message = String.Join(messageFormat.Equals("text") ? "\n" : "<br/>", notification.ToMessage(bot));

            string json = jobject.ToString();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            string url = baseUrl + "/room/" + room + "/notification";
            requestContext.Trace(0, System.Diagnostics.TraceLevel.Verbose, Constants.TraceArea, "HipChatNotifier", "Sending notification to {0}\n{1}", url, json);

            await httpClient.PostAsync(url, content).ContinueWith(t => t.Result.EnsureSuccessStatusCode());           
        }
    }
}
