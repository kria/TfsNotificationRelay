/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2015 Kristian Adrup
 * 
 * This file is part of TfsNotificationRelay.
 * 
 * TfsNotificationRelay is free software: you can redistribute it and/or 
 * modify it under the terms of the GNU General Public License as published 
 * by the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. See included file COPYING for details.
 */

using DevCore.TfsNotificationRelay.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace DevCore.TfsNotificationRelay.Notifications
{
    public abstract class PullRequestNotification : BaseNotification
    {
        protected readonly static Configuration.SettingsElement settings = Configuration.TfsNotificationRelaySection.Instance.Settings;

        public string CreatorUserName { get; set; }
        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string ProjectName { get; set; }
        public string RepoUri { get; set; }
        public string RepoName { get; set; }
        public int PrId { get; set; }
        public string PrUrl { get; set; }
        public string PrTitle { get; set; }
        public GitRef SourceBranch { get; set; }
        public GitRef TargetBranch { get; set; }
        public IEnumerable<string> ReviewerUserNames { get; set; }

        public string UserName
        {
            get { return settings.StripUserDomain ? TextHelper.StripDomain(UniqueName) : UniqueName; }
        }

        public override IEnumerable<string> TargetUserNames
        {
            get
            {
                if (CreatorUserName != UniqueName)
                    return new[] { CreatorUserName };
                else
                    return Enumerable.Empty<string>();
            }
        }

        public IEnumerable<EventRuleElement> GetRulesMatch(string collection, IEnumerable<EventRuleElement> eventRules)
        {
            var rules = eventRules.Where(r => collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject)
                && TeamNames.IsMatchOrNoPattern(r.TeamName)
                && RepoName.IsMatchOrNoPattern(r.GitRepository)
                && TargetBranch.Name.IsMatchOrNoPattern(r.GitBranch));

            return rules;
        }
    }
}
