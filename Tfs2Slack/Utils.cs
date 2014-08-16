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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack
{
    public static class Utils
    {
        public static string ToHexString(this byte[] buffer)
        {
            return BitConverter.ToString(buffer).Replace("-", "").ToLower();
        }

        public static string ToShortHexString(this byte[] buffer)
        {
            return buffer.ToHexString().Substring(0, 6);
        }

        public static bool IsZero(this byte[] buffer)
        {
            return buffer.All(b => b == 0);
        }

        public static string Truncate(this string text, int len)
        {
            text = text.TrimEnd(Environment.NewLine.ToCharArray());
            int pos = text.IndexOf('\n');
            if (pos > 0 && pos <= len)
                return text.Substring(0, pos);
            if (text.Length <= len)
                return text;
            pos = text.LastIndexOf(' ', len);
            if (pos == -1) pos = len;

            return text.Substring(0, pos) + "...";
        }

        public static string StripDomain(string username)
        {
            int pos = username.IndexOf("\\");
            return pos != -1 ? username.Substring(pos + 1) : username;
        }
    }
}
