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

using DevCore.TfsNotificationRelay.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.Notifications.GitPush
{
    public class GitPushNotification : MultiRowNotification
    {
        private readonly string _projectName;
        private readonly string _repoName;
        private readonly IEnumerable<string> _teamNames;
        private readonly IEnumerable<GitRef> _refs;
        private readonly string _userName;

        public GitPushNotification(string teamProjecCollection, string projectName, string repoName, string userName, IEnumerable<string> teamNames, IEnumerable<GitRef> refs)
        {
            TeamProjectCollection = teamProjecCollection;
            _projectName = projectName;
            _repoName = repoName;
            _userName = userName;
            _teamNames = teamNames;
            _refs = refs;
        }

        public override EventRuleElement GetRuleMatch(string collection, IEnumerable<EventRuleElement> eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.GitPush)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && _projectName.IsMatchOrNoPattern(r.TeamProject)
                && _teamNames.IsMatchOrNoPattern(r.TeamName)
                && _repoName.IsMatchOrNoPattern(r.GitRepository)
                && (string.IsNullOrEmpty(r.GitBranch) || _refs.Any(n => Regex.IsMatch(n.Name, r.GitBranch)))
                && (string.IsNullOrEmpty(r.GitTag) || _refs.Any(n => Regex.IsMatch(n.Name, r.GitTag))));

            return rule;
        }
    }

}
