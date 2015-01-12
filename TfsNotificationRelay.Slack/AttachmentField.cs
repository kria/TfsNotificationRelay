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
    public class AttachmentField
    {
        public AttachmentField() {}

        public AttachmentField(string title, string value, bool isShort = false) 
        {
            this.Title = title;
            this.Value = value;
            this.IsShort = isShort;
        }

        public string Title { get; set; }
        
        public string Value { get; set; }

        [JsonProperty(PropertyName = "short")]
        public bool IsShort { get; set; }
    }
}
