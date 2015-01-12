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

namespace DevCore.TfsNotificationRelay
{
    class TfsNotificationRelayException : Exception
    {
        public TfsNotificationRelayException()
        {
        }

        public TfsNotificationRelayException(string message)
            : base(message)
        {
        }

        public TfsNotificationRelayException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
