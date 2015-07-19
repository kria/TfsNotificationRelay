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
    public class DeleteRow : NotificationRow
    {
        public IEnumerable<GitRef> Refs { get; set; }

        public override string ToString(BotElement bot, Func<string, string> transform)
        {
            return String.Format("{0} {1}", Refs.ToString(bot, transform), bot.Text.Deleted);
        }
    }
}
