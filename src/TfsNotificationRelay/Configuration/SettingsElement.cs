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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.Configuration
{
    public class SettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("logfile")]
        public string Logfile => (string)this["logfile"];

        [ConfigurationProperty("stripUserDomain")]
        public bool StripUserDomain => (bool)this["stripUserDomain"];

        [ConfigurationProperty("commentMaxLength")]
        public int CommentMaxLength => (int)this["commentMaxLength"];

        [ConfigurationProperty("discussionCommentMaxLength")]
        public int DiscussionCommentMaxLength => (int)this["discussionCommentMaxLength"];

        [ConfigurationProperty("maxLines")]
        public int MaxLines => (int)this["maxLines"];

        [ConfigurationProperty("hashLength")]
        public int HashLength => (int)this["hashLength"];

        [ConfigurationProperty("identifyForcePush")]
        public bool IdentifyForcePush => (bool)this["identifyForcePush"];
    }
}
