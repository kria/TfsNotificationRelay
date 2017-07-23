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
using System.Collections.Generic;

namespace DevCore.TfsNotificationRelay.Slack.Models
{
    public class Message
    {
        public string Channel { get; set; }

        public string Username { get; set; }

        public string Text { get; set; }

        public IEnumerable<Attachment> Attachments { get; set; }

        [JsonProperty(PropertyName = "icon_url")]
        public string IconUrl { get; set; }

        [JsonProperty(PropertyName = "icon_emoji")]
        public string IconEmoji { get; set; }

        [JsonProperty(PropertyName = "as_user")]
        public bool AsUser { get; set; }
    }
}
