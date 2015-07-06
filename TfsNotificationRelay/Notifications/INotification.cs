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

namespace DevCore.TfsNotificationRelay.Notifications
{
    public interface INotification
    {
        string TeamProjectCollection { get; set; }

        /// <summary>
        /// Returns the message formatted according the text settings of the bot.
        /// The transform function will be applied to all text from TFS.
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        IList<string> ToMessage(Configuration.BotElement bot, Func<string, string> transform);

        EventRuleElement GetRuleMatch(string collection, Configuration.EventRuleCollection eventRules);
    }
}
