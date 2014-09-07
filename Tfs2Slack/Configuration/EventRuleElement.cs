/*
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
    public class EventRuleElement : ConfigurationElement
    {
        [ConfigurationProperty("events", IsRequired = true)]
        public TfsEvents Events
        {
            get { return (TfsEvents)this["events"]; }
        }

        [ConfigurationProperty("notify")]
        public bool Notify
        {
            get { return (bool)this["notify"]; }
        }

        [ConfigurationProperty("teamProject")]
        public string TeamProject
        {
            get { return (string)this["teamProject"]; }
        }

        [ConfigurationProperty("gitRepository")]
        public string GitRepository
        {
            get { return (string)this["gitRepository"]; }
        }

        [ConfigurationProperty("buildDefinition")]
        public string BuildDefinition
        {
            get { return (string)this["buildDefinition"]; }
        }
    }
}
