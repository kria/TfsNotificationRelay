/*
 * Tfs2Slack - http://github.com/kria/Tfs2Slack
 * 
 * Copyright (C) 2014 Kristian Adrup
 * 
 * This file is part of Tfs2Slack.
 * 
 * Tfs2Slack is free software: you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version. See included file COPYING for details.
 */

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.EventHandlers
{
    public abstract class BaseHandler : IEventHandler
    {
        protected static Configuration.SettingsElement settings = Configuration.Tfs2SlackSection.Instance.Settings;
        protected static Configuration.TextElement text = Configuration.Tfs2SlackSection.Instance.Text;

        protected abstract IList<string> _ProcessEvent(TeamFoundationRequestContext requestContext, object notificationEventArgs, Configuration.BotElement bot);

        public IList<string> ProcessEvent(TeamFoundationRequestContext requestContext, object notificationEventArgs, Configuration.BotElement bot)
        {
            IList<string> lines = _ProcessEvent(requestContext, notificationEventArgs, bot);
            if (lines != null && lines.Count > 0)
            {
                IList<string> sendLines = lines;
                if (lines != null && lines.Count > settings.MaxLines)
                {
                    int supressedLines = lines.Count - settings.MaxLines;
                    lines = lines.Take(settings.MaxLines).ToList();
                    lines.Add(text.LinesSupressedFormat.FormatWith(new { Count = supressedLines }));
                }
            }

            return lines;
        }
    }
}
