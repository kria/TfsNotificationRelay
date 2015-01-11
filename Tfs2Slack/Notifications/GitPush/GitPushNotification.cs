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

using DevCore.Tfs2Slack.Configuration;
using Microsoft.TeamFoundation.Git.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.Notifications.GitPush
{
    public class GitPushNotification : MultiRowNotification
    {
        private string projectName;
        private string repoName;

        public GitPushNotification(string teamProjecCollection, string projectName, string repoName)
        {
            this.TeamProjectCollection = teamProjecCollection;
            this.projectName = projectName;
            this.repoName = repoName;
        }

        public override bool IsMatch(string collection, Configuration.EventRuleCollection eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.GitPush)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && projectName.IsMatchOrNoPattern(r.TeamProject)
                && repoName.IsMatchOrNoPattern(r.GitRepository));

            if (rule != null) return rule.Notify;

            return false;
        }
    }

}
