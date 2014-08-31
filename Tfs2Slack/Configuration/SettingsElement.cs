﻿/*
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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.Configuration
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
    }
}
