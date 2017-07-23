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

using DevCore.TfsNotificationRelay.Configuration;

namespace DevCore.TfsNotificationRelay.Notifications
{
    public class Build2015CompletionNotification : BuildCompletionNotification
    {
        protected override string GetBuildFormat(TextElement text)
        {
            return string.IsNullOrEmpty(text.Build2015Format) ? text.BuildFormat : text.Build2015Format;
        }
    }
}
