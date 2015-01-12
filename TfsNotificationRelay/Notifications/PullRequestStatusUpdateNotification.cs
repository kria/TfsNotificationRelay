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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Git.Common;

namespace DevCore.TfsNotificationRelay.Notifications
{
    class PullRequestStatusUpdateNotification : BaseNotification
    {
        protected readonly static Configuration.SettingsElement settings = Configuration.TfsNotificationRelaySection.Instance.Settings;

        public PullRequestStatus Status { get; set; } 
        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string ProjectName { get; set; }
        public string RepoUri { get; set; }
        public string RepoName { get; set; }
        public int PrId { get; set; }
        public string PrUrl { get; set; }
        public string PrTitle { get; set; }
        private string FormatAction(Configuration.BotElement bot)
        {
            switch (Status)
                {
                    case PullRequestStatus.Abandoned: return bot.Text.Abandoned;
                    case PullRequestStatus.Active: return bot.Text.Reactivated;
                    case PullRequestStatus.Completed: return bot.Text.Completed;
                    default:
                        return String.Format("updated status to {0} for", Status.ToString());
                }
        }
        public string UserName
        {
            get { return settings.StripUserDomain ? Utils.StripDomain(UniqueName) : UniqueName; }
        }

        public override IList<string> ToMessage(Configuration.BotElement bot)
        {
            var formatter = new
            {
                TeamProjectCollection = this.TeamProjectCollection,
                Status = this.Status,
                DisplayName = this.DisplayName,
                ProjectName = this.ProjectName,
                RepoUri = this.RepoUri,
                RepoName = this.RepoName,
                PrId = this.PrId,
                PrUrl = this.PrUrl,
                PrTitle = this.PrTitle,
                UserName = this.UserName,
                Action = FormatAction(bot)
            };
            return new[] { bot.Text.PullRequestStatusUpdateFormat.FormatWith(formatter) };
        }

        public override bool IsMatch(string collection, Configuration.EventRuleCollection eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.PullRequestStatusUpdate)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject)
                && RepoName.IsMatchOrNoPattern(r.GitRepository));

            if (rule != null) return rule.Notify;

            return false;
        }
    }
}
