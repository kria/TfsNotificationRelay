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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.Notifications.GitPush
{
    class PushRow : NotificationRow
    {
        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string RepoUri { get; set; }
        public string ProjectName { get; set; }
        public string RepoName { get; set; }
        public bool IsForcePush { get; set; }
        public string UserName
        {
            get { return settings.StripUserDomain ? Utils.StripDomain(UniqueName) : UniqueName; }
        }
        public override string ToString(BotElement bot)
        {
            var formatter = new
            {
                DisplayName = this.DisplayName,
                RepoUri = this.RepoUri,
                ProjectName = this.ProjectName,
                RepoName = this.RepoName,
                Pushed = this.IsForcePush ? bot.Text.ForcePushed : bot.Text.Pushed,
                UserName = this.UserName
            };
            return bot.Text.PushFormat.FormatWith(formatter);
        }
    }
}
