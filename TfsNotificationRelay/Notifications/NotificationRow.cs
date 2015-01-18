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
using System.Threading.Tasks;
using DevCore.TfsNotificationRelay.Configuration;

namespace DevCore.TfsNotificationRelay.Notifications
{
    public abstract class NotificationRow
    {
        protected readonly static Configuration.SettingsElement settings = Configuration.TfsNotificationRelaySection.Instance.Settings;

        public abstract string ToString(BotElement bot, Func<string, string> transform);
    }
}
