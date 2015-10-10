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

namespace DevCore.TfsNotificationRelay.Notifications
{
    public class PullRequestReviewerVoteNotification : PullRequestNotification
    {
        public short Vote { get; set; }
        private string FormatAction(BotElement bot)
        {
            switch (Vote)
            {
                case -10: return bot.Text.VoteRejected;
                case 0: return bot.Text.VoteRescinded;
                case 10: return bot.Text.VoteApproved;
                default:
                    return $"voted {Vote} on";
            }
        }

        public override IList<string> ToMessage(BotElement bot, Func<string, string> transform)
        {
            var formatter = new
            {
                TeamProjectCollection = transform(TeamProjectCollection),
                Vote,
                DisplayName = transform(DisplayName),
                ProjectName = transform(ProjectName),
                RepoUri,
                RepoName = transform(RepoName),
                PrId,
                PrUrl,
                PrTitle = transform(PrTitle),
                UserName = transform(UserName),
                Action = FormatAction(bot),
                SourceBranchName = transform(SourceBranch.Name),
                TargetBranchName = transform(TargetBranch.Name)
            };
            return new[] { bot.Text.PullRequestReviewerVoteFormat.FormatWith(formatter) };
        }

        public override EventRuleElement GetRuleMatch(string collection, IEnumerable<EventRuleElement> eventRules)
        {
            var rule = GetRulesMatch(collection, eventRules).FirstOrDefault(r => r.Events.HasFlag(TfsEvents.PullRequestReviewerVote));

            return rule;
        }
    }
}
