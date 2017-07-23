/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2016 Kristian Adrup
 * 
 * This file is part of TfsNotificationRelay.
 * 
 * TfsNotificationRelay is free software: you can redistribute it and/or 
 * modify it under the terms of the GNU General Public License as published 
 * by the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. See included file COPYING for details.
 */

using DevCore.TfsNotificationRelay.Configuration;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevCore.TfsNotificationRelay.Notifications
{
    public abstract class ReleaseNotification : BaseNotification
    {
        protected static SettingsElement Settings = TfsNotificationRelaySection.Instance.Settings;

        public string ProjectName { get; set; }
        public string ReleaseDefinition { get; set; }
        public string ReleaseName { get; set; }
        public ReleaseStatus ReleaseStatus { get; set; }
        public string ReleaseUrl { get; set; }
        public ReleaseReason ReleaseReason { get; set; }
        public string CreatedByUniqueName { get; set; }
        public string CreatedByDisplayName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserName => Settings.StripUserDomain ? TextHelper.StripDomain(CreatedByUniqueName) : CreatedByUniqueName;

        public override IEnumerable<string> TargetUserNames => new[] { CreatedByUniqueName };
        public string DisplayName => CreatedByDisplayName;

        public IEnumerable<EventRuleElement> GetRulesMatch(string collection, IEnumerable<EventRuleElement> eventRules)
        {
            var rules = eventRules.Where(r => r.Events.HasFlag(TfsEvents.ReleaseCreated)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject)
                && TeamNames.IsMatchOrNoPattern(r.TeamName)
                && ReleaseDefinition.IsMatchOrNoPattern(r.ReleaseDefinition));

            return rules;
        }
    }
}
