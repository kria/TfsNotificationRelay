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

namespace DevCore.TfsNotificationRelay.Slack
{
    class PayloadSettings
    {
        public PayloadSettings() { }

        public PayloadSettings(string webhookUrl, string channel, 
            string username, string iconEmoji, string iconUrl, string color)
        {
            this.WebhookUrl = webhookUrl;
            this.Channel = channel;
            this.Username = username;
            this.IconEmoji = iconEmoji;
            this.IconUrl = iconUrl;
            this.Color = color;
        }
        public string WebhookUrl { get; set; }
        public string Channel { get; set; }
        public string Username { get; set; }
        public string IconEmoji { get; set; }
        public string IconUrl { get; set; }
        public string Color { get; set; }
    }
}
