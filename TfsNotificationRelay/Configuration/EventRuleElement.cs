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

using Microsoft.TeamFoundation.Build.Server;
using System.Collections.Generic;
using System.Configuration;

namespace DevCore.TfsNotificationRelay.Configuration
{
    public class EventRuleElement : ConfigurationElement, IKeyedConfigurationElement
    {
        public object Key { get { return this; } }
        
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

        [ConfigurationProperty("teamProjectCollection")]
        public string TeamProjectCollection
        {
            get { return (string)this["teamProjectCollection"]; }
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

        [ConfigurationProperty("teamName")]
        public string TeamName
        {
            get { return (string)this["teamName"]; }
        }

        [ConfigurationProperty("workItemType")]
        public string WorkItemType
        {
          get { return (string)this["workItemType"]; }
        }

        [ConfigurationProperty("areaPath")] 
        public string AreaPath
        {
            get { return (string)this["areaPath"]; }
        }

        [ConfigurationProperty("buildStatuses")]
        public BuildStatus BuildStatuses
        {
            get
            {
                if (this["buildStatuses"] == null) return BuildStatus.All;
                return (BuildStatus)this["buildStatuses"];
            }
        }

        [ConfigurationProperty("workItemfields")]
        public string WorkItemfields
        {
            get { return (string)this["workItemfields"]; }
        }

        public IEnumerable<string> WorkItemFieldItems
        {
            get {
                return TextHelper.SplitCsv(WorkItemfields);
            }
        }

        [ConfigurationProperty("sourcePath")]
        public string SourcePath
        {
            get { return (string)this["sourcePath"]; }
        }
    }
}
