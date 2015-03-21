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
using Microsoft.TeamFoundation.Git.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.Notifications.GitPush
{
    public class RefUpdateRow : NotificationRow
    {
        public byte[] NewObjectId { get; set; }
        public TfsGitObjectType ObjectType { get; set; }
        public IList<string> RefNames { get; set; }

        public override string ToString(BotElement bot, Func<string, string> transform)
        {
            return String.Format("{0} {1} {2} {3}",
                transform(String.Concat(RefNames)), bot.Text.RefPointer, transform(ObjectType.ToString()), transform(NewObjectId.ToHexString()));
        }
    }
}
