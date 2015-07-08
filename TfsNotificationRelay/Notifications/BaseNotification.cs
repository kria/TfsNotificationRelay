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

namespace DevCore.TfsNotificationRelay.Notifications
{
    public abstract class BaseNotification : INotification
    {
        public string TeamProjectCollection { get; set; }
        public IEnumerable<string> TeamNames { get; set; }

        public abstract IList<string> ToMessage(Configuration.BotElement bot, Func<string, string> transform);

        public abstract EventRuleElement GetRuleMatch(string collection, Configuration.EventRuleCollection eventRules);
    }
}
