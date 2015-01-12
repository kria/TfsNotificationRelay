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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.Notifications
{
    public class WorkItemChangedNotification : BaseNotification
    {
        protected static Configuration.SettingsElement settings = Configuration.TfsNotificationRelaySection.Instance.Settings;

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

        private string FormatAction(Configuration.BotElement bot)
        {
            return IsNew ? bot.Text.Created : bot.Text.Updated;
        }

        public override IList<string> ToMessage(Configuration.BotElement bot)
        {
            var lines = new List<string>();
            var formatter = new
            {
                TeamProjectCollection = this.TeamProjectCollection,
                DisplayName = this.DisplayName,
                ProjectName = this.ProjectName,
                WiUrl = this.WiUrl,
                WiType = this.WiType,
                WiId = this.WiId,
                WiTitle = this.WiTitle,
                IsStateChanged = this.IsStateChanged,
                IsAssignmentChanged = this.IsAssignmentChanged,
                AssignedTo = this.AssignedTo,
                State = this.State,
                UserName = this.UserName,
                Action = FormatAction(bot)
            };
            lines.Add(bot.Text.WorkItemchangedFormat.FormatWith(formatter));
            lines.Add(String.Format("State: {0}", State));
            lines.Add(String.Format("AssignedTo: {0} ", AssignedTo));

            return lines;
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
