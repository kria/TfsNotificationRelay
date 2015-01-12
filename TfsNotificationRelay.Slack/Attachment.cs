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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.Slack
{
    public class Attachment
    {
        private static readonly string[] mrkdwn_in = { "pretext", "text", "title", "fields", "fallback" };

        public string Fallback { get; set; }
        
        public string Text { get; set; }
        
        public string Pretext { get; set; }
        
        public string Color { get; set; }
        
        [JsonProperty(PropertyName = "mrkdwn_in")]
        public IEnumerable<string> MrkdwnIn 
        {
            get { return mrkdwn_in; }
        }
        public IEnumerable<AttachmentField> Fields { get; set; }
    }
}
