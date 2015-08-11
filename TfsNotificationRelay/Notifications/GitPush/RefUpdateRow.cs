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
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;

namespace DevCore.TfsNotificationRelay.Notifications.GitPush
{
    public class RefUpdateRow : NotificationRow
    {
        public Sha1Id NewObjectId { get; set; }
        public GitObjectType ObjectType { get; set; }
        public IEnumerable<GitRef> Refs { get; set; }

        public override string ToString(BotElement bot, Func<string, string> transform)
        {
            return String.Format("{0} {1} {2} {3}", Refs.ToString(bot, transform), bot.Text.RefPointer, transform(ObjectType.ToString()), transform(NewObjectId.ToHexString()));
        }
    }
}
