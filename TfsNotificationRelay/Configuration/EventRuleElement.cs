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
        public object Key => this;

        [ConfigurationProperty("events", IsRequired = true)]
        public TfsEvents Events => (TfsEvents)this["events"];

        [ConfigurationProperty("notify")]
        public bool Notify => (bool)this["notify"];

        [ConfigurationProperty("teamProjectCollection")]
        public string TeamProjectCollection => (string)this["teamProjectCollection"];

        [ConfigurationProperty("teamProject")]
        public string TeamProject => (string)this["teamProject"];

        [ConfigurationProperty("gitRepository")]
        public string GitRepository => (string)this["gitRepository"];

        [ConfigurationProperty("buildDefinition")]
        public string BuildDefinition => (string)this["buildDefinition"];

        [ConfigurationProperty("teamName")]
        public string TeamName => (string)this["teamName"];

        [ConfigurationProperty("workItemType")]
        public string WorkItemType => (string)this["workItemType"];

        [ConfigurationProperty("areaPath")] 
        public string AreaPath => (string)this["areaPath"];

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
        public string WorkItemfields => (string)this["workItemfields"];

        public IEnumerable<string> WorkItemFieldItems => TextHelper.SplitCsv(WorkItemfields);

        [ConfigurationProperty("sourcePath")]
        public string SourcePath => (string)this["sourcePath"];

        [ConfigurationProperty("gitBranch")]
        public string GitBranch => (string)this["gitBranch"];

        [ConfigurationProperty("gitTag")]
        public string GitTag => (string)this["gitTag"];
    }
}
