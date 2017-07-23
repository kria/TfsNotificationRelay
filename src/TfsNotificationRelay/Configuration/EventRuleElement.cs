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
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DevCore.TfsNotificationRelay.Configuration
{
    public class EventRuleElement : ConfigurationElement, IKeyedConfigurationElement
    {
        public object Key => this;

        [ConfigurationProperty("events", DefaultValue = TfsEvents.All)]
        public TfsEvents Events => (TfsEvents)this["events"];

        [ConfigurationProperty("notify", IsRequired = true)]
        public bool Notify => (bool)this["notify"];

        [ConfigurationProperty("teamProjectCollection")]
        public string TeamProjectCollection => (string)this["teamProjectCollection"];

        [ConfigurationProperty("teamProject")]
        public string TeamProject => (string)this["teamProject"];

        [ConfigurationProperty("gitRepository")]
        public string GitRepository => (string)this["gitRepository"];

        [ConfigurationProperty("buildDefinition")]
        public string BuildDefinition => (string)this["buildDefinition"];

        [ConfigurationProperty("releaseDefinition")]
        public string ReleaseDefinition => (string)this["releaseDefinition"];

        [ConfigurationProperty("teamName")]
        public string TeamName => (string)this["teamName"];

        [ConfigurationProperty("workItemType")]
        public string WorkItemType => (string)this["workItemType"];

        [ConfigurationProperty("areaPath")]
        public string AreaPath => (string)this["areaPath"];

        [ConfigurationProperty("buildStatuses", DefaultValue = BuildStatus.All)]
        public BuildStatus BuildStatuses => (BuildStatus)this["buildStatuses"];

        [ConfigurationProperty("workItemfields")]
        public string WorkItemfields => (string)this["workItemfields"];

        public IEnumerable<string> WorkItemFieldItems => TextHelper.SplitCsv(WorkItemfields);

        [ConfigurationProperty("sourcePath")]
        public string SourcePath => (string)this["sourcePath"];

        [ConfigurationProperty("gitBranch")]
        public string GitBranch => (string)this["gitBranch"];

        [ConfigurationProperty("gitTag")]
        public string GitTag => (string)this["gitTag"];

        [ConfigurationProperty("environment")]
        public string Environment => (string)this["environment"];

        [ConfigurationProperty("environmentStatuses")]
        public string EnvironmentStatuses => (string)this["environmentStatuses"];

        [ConfigurationProperty("text")]
        public string Text => (string)this["text"];

        public IEnumerable<EnvironmentStatus> EnvironmentStatusesEnums { get; private set; }

        protected override void PostDeserialize()
        {
            base.PostDeserialize();

            if (EnvironmentStatuses == "" || EnvironmentStatuses == "All")
                EnvironmentStatusesEnums = Enumerable.Empty<EnvironmentStatus>();
            else
                EnvironmentStatusesEnums = TextHelper.SplitCsv(EnvironmentStatuses).Cast<EnvironmentStatus>().ToArray();
        }
    }
}
