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
        public string Logfile
        {
            get { return (string)this["logfile"]; }
        }

        [ConfigurationProperty("stripUserDomain")]
        public bool StripUserDomain
        {
            get { return (bool)this["stripUserDomain"]; }
        }

        [ConfigurationProperty("commentMaxLength")]
        public int CommentMaxLength
        {
            get { return (int)this["commentMaxLength"]; }
        }

        [ConfigurationProperty("maxLines")]
        public int MaxLines
        {
            get { return (int)this["maxLines"]; }
        }

        [ConfigurationProperty("hashLength")]
        public int HashLength
        {
            get { return (int)this["hashLength"]; }
        }

        [ConfigurationProperty("identifyForcePush")]
        public bool IdentifyForcePush
        {
            get { return (bool)this["identifyForcePush"]; }
        }
    }
}
