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

using DevCore.Tfs2Slack.Slack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.Notifications
{
    class WorkItemChangedNotification : BaseNotification
    {
        protected static Configuration.TextElement text = Configuration.Tfs2SlackSection.Instance.Text;
        protected static Configuration.SettingsElement settings = Configuration.Tfs2SlackSection.Instance.Settings;

        public bool IsNew { get; set; }
        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string WiUrl { get; set; }
        public string WiType { get; set; }
        public int WiId { get; set; }
        public string WiTitle { get; set; }
        public string ProjectName { get; set; }
        public bool IsStateChanged { get; set; }
        public bool IsAssignmentChanged { get; set; }
        public string AssignedTo { get; set; }
        public string State { get; set; }

        public string UserName
        {
            get { return settings.StripUserDomain ? Utils.StripDomain(UniqueName) : UniqueName; }
        }

        public string Action
        {
            get { return IsNew ? text.Created : text.Updated;  }
        }

        public override IList<string> ToMessage(Configuration.BotElement bot)
        {
            var lines = new List<string>();
            lines.Add(text.WorkItemchangedFormat.FormatWith(this));
            lines.Add(String.Format("*State:* {0}", State));
            lines.Add(String.Format("*AssignedTo:* {0} ", AssignedTo));

            return lines;
        }

        public override Slack.Message ToSlackMessage(Configuration.BotElement bot, string channel)
        {
            string header = text.WorkItemchangedFormat.FormatWith(this);

            var message = new Slack.Message()
            {
                Channel = channel,
                Username = bot.SlackUsername,
                Attachments = new[] { 
                    new Attachment() {
                        Fallback = header,
                        Pretext = header,
                        Color = bot.SlackColor,
                        Fields = new[] { new AttachmentField(text.State, State, true), new AttachmentField(text.AssignedTo, AssignedTo, true) }
                    }
                }
            };
            if (!String.IsNullOrEmpty(bot.SlackIconUrl))
                message.IconUrl = bot.SlackIconUrl;
            else if (!String.IsNullOrEmpty(bot.SlackIconEmoji))
                message.IconEmoji = bot.SlackIconEmoji;

            return message;
        }

        public override bool IsMatch(string collection, Configuration.EventRuleCollection eventRules)
        {
            var rule = eventRules.FirstOrDefault(r =>
                (r.Events.HasFlag(TfsEvents.WorkItemStateChange) && IsStateChanged
                || r.Events.HasFlag(TfsEvents.WorkItemAssignmentChange) && IsAssignmentChanged)
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject));

            if (rule != null) return rule.Notify;

            return false;
        }
    }
}
