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
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.Notifications.GitPush
{
    public class PushRow : NotificationRow
    {
        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public string RepoUri { get; set; }
        public string ProjectName { get; set; }
        public string RepoName { get; set; }
        public bool IsForcePush { get; set; }
        public string UserName
        {
            get { return settings.StripUserDomain ? TextHelper.StripDomain(UniqueName) : UniqueName; }
        }
        public override string ToString(BotElement bot, Func<string, string> transform)
        {
            var formatter = new
            {
                DisplayName = transform(this.DisplayName),
                RepoUri = this.RepoUri,
                ProjectName = transform(this.ProjectName),
                RepoName = transform(this.RepoName),
                Pushed = this.IsForcePush ? bot.Text.ForcePushed : bot.Text.Pushed,
                UserName = transform(this.UserName)
            };
            return bot.Text.PushFormat.FormatWith(formatter);
        }
    }
}
