/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2016 Kristian Adrup
 * 
 * This file is part of TfsNotificationRelay.
 * 
 * TfsNotificationRelay is free software: you can redistribute it and/or 
 * modify it under the terms of the GNU General Public License as published 
 * by the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. See included file COPYING for details.
 */

using System;
using System.Text.RegularExpressions;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    class UserField
    {
        private UserField() {}

        private UserField(string displayName, Guid identifier)
        {
            DisplayName = displayName;
            Identifier = identifier;
        }

        public string DisplayName { get; }
        public Guid Identifier { get; }

        public static bool TryParse(string userstring, out UserField field)
        {
            const string pattern = @"^\|(.*)%(.*)\|$";
            field = null;

            var match = Regex.Match(userstring, pattern);
            if (!match.Success) return false;

            Guid id = Guid.Empty;
            if (!Guid.TryParse(match.Groups[2].Value, out id)) return false;

            field = new UserField(match.Groups[1].Value, id);

            return true;
        }
    }
}
