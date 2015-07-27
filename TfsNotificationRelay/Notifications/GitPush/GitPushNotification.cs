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
using Microsoft.TeamFoundation.Git.Server;
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
        private string projectName;
        private string repoName;
        private IEnumerable<string> teamNames;
        private IEnumerable<GitRef> refs;
        private string userName;

        public GitPushNotification(string teamProjecCollection, string projectName, string repoName, string userName, IEnumerable<string> teamNames, IEnumerable<GitRef> refs)
        {
            this.TeamProjectCollection = teamProjecCollection;
            this.projectName = projectName;
            this.repoName = repoName;
            this.userName = userName;
            this.teamNames = teamNames;
            this.refs = refs;
        }

        public override EventRuleElement GetRuleMatch(string collection, Configuration.EventRuleCollection eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.GitPush)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && projectName.IsMatchOrNoPattern(r.TeamProject)
                && teamNames.IsMatchOrNoPattern(r.TeamName)
                && repoName.IsMatchOrNoPattern(r.GitRepository)
                && (String.IsNullOrEmpty(r.GitBranch) || refs.Any(n => Regex.IsMatch(n.Name, r.GitBranch)))
                && (String.IsNullOrEmpty(r.GitTag) || refs.Any(n => Regex.IsMatch(n.Name, r.GitTag))));

            return rule;
        }
    }

}
