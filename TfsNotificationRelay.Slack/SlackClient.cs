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
        public Task<HttpResponseMessage> SendMessageAsync(Message message, string webhookUrl)
        {
            string json = JsonConvert.SerializeObject(message, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            //Logger.Log(json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return PostAsync(webhookUrl, content);
        }

        [Obsolete]
        public Task<HttpResponseMessage> SendMessageAsync(IEnumerable<string> lines, PayloadSettings settings)
        {
            dynamic json = JObject.FromObject(new
            {
                channel = settings.Channel,
                username = settings.Username,
                attachments = new[] {
                        new {
                            fallback = lines.First(),
                            pretext = lines.First(),
                            color = settings.Color,
                            mrkdwn_in = new [] { "pretext", "text", "title", "fields", "fallback" },
                            fields = from line in lines.Skip(1) select new { value = line, @short = false }
                        }
                    }
            });
            if (!String.IsNullOrEmpty(settings.IconUrl))
                json.icon_url = settings.IconUrl;
            else if (!String.IsNullOrEmpty(settings.IconEmoji))
                json.icon_emoji = settings.IconEmoji;

            //Logger.Log(json.ToString());

            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            return PostAsync(settings.WebhookUrl, content);
        }
    }
}
