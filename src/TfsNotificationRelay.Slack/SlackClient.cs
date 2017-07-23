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

using DevCore.TfsNotificationRelay.Slack.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.Slack
{
    class SlackClient : HttpClient
    {
        private const string ApiBaseUrl = "https://slack.com/api/";

        public JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };


        public Task<HttpResponseMessage> SendWebhookMessageAsync(Message message, string webhookUrl)
        {
            string json = JsonConvert.SerializeObject(message, Formatting.Indented, SerializerSettings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return PostAsync(webhookUrl, content);
        }

        public async Task<HttpResponseMessage> SendApiMessageAsync(Message message, string token, string userId)
        {
            var args = new Dictionary<string, string>();
            var channelId = await OpenImChannelAsync(token, userId);
            args.Add("token", token);
            args.Add("channel", channelId);
            args.Add("as_user", message.AsUser.ToString());
            args.Add("text", message.Text);
            args.Add("username", message.Username);
            args.Add("attachments", JsonConvert.SerializeObject(message.Attachments, Formatting.Indented, SerializerSettings));
            args.Add("icon_url", message.IconUrl);
            args.Add("icon_emoji", message.IconEmoji);

            var content = new FormUrlEncodedContent(args);
            return await PostAsync(ApiBaseUrl + "chat.postMessage", content);
        }

        private async Task<string> OpenImChannelAsync(string token, string userId)
        {
            var args = new Dictionary<string, string>();
            args.Add("token", token);
            args.Add("user", userId);
            var content = new FormUrlEncodedContent(args);
            var result = await PostAsync(ApiBaseUrl + "im.open", content);
            result.EnsureSuccessStatusCode();

            var body = await result.Content.ReadAsStringAsync();
            dynamic response = JObject.Parse(body);
            string channelId = response.channel.id;

            return channelId;
        }

    }
}
