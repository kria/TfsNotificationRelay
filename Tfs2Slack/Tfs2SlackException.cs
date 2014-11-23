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

using System;

namespace DevCore.Tfs2Slack
{
    class Tfs2SlackException : Exception
    {
        public Tfs2SlackException()
        {
        }

        public Tfs2SlackException(string message)
            : base(message)
        {
        }

        public Tfs2SlackException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
