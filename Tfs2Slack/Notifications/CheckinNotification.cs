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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.Notifications
{
    class CheckinNotification : BaseNotification
    {
        protected static Configuration.TextElement text = Configuration.Tfs2SlackSection.Instance.Text;
        protected static Configuration.SettingsElement settings = Configuration.Tfs2SlackSection.Instance.Settings;

        public string UniqueName { get; set; }
        public string ChangesetUrl { get; set; }
        public int ChangesetId { get; set; }
        public Dictionary<string, string> Projects { get; set; }
        public string Comment { get; set; }
        public string ProjectLinks 
        {
            get { return String.Join(", ", Projects.Select(x => String.Format("<{0}|{1}>", x.Value, x.Key))); }
        }
        public string UserName
        {
            get { return settings.StripUserDomain ? Utils.StripDomain(UniqueName) : UniqueName; }
        }

        public override IList<string> ToMessage(Configuration.BotElement bot)
        {
            return new[] { text.CheckinFormat.FormatWith(this) };
        }

        public override bool IsMatch(string collection, Configuration.EventRuleCollection eventRules)
        {
            var rule = eventRules.FirstOrDefault(r => r.Events.HasFlag(TfsEvents.Checkin)
                && collection.IsMatchOrNoPattern(r.Collection)
                && (String.IsNullOrEmpty(r.TeamProject) || Projects.Keys.Any(n => Regex.IsMatch(n, r.TeamProject))));

            if (rule != null) return rule.Notify;

            return false;
        }

    }



}

