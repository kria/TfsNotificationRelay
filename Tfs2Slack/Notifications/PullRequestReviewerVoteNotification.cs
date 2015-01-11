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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.Notifications
{
    class PullRequestReviewerVoteNotification : BaseNotification
    {
        protected readonly static Configuration.SettingsElement settings = Configuration.Tfs2SlackSection.Instance.Settings;

        public short Vote { get; set; }
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
            switch (Vote)
            {
                case -10: return bot.Text.VoteRejected;
                case 0: return bot.Text.VoteRescinded;
                case 10: return bot.Text.VoteApproved;
                default:
                    return String.Format("voted {0} on", Vote);
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
                Vote = this.Vote,
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
            return new[] { bot.Text.PullRequestReviewerVoteFormat.FormatWith(formatter) };
        }

        public override bool IsMatch(string collection, Configuration.EventRuleCollection eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.PullRequestReviewerVote)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject)
                && RepoName.IsMatchOrNoPattern(r.GitRepository));

            if (rule != null) return rule.Notify;

            return false;
        }
    }
}
