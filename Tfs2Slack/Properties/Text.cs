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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.Properties
{
    partial class Text
    {
        Text() : base(new AssemblySettings.ConfigurationFileApplicationSettings(
                Assembly.GetExecutingAssembly(), typeof(Text)
                )) { }

        public string FormatPushText(string userName, string repoUri, string projectName, string repoName, bool isForcePush) {
            return PushFormat
                .Replace("@userName", userName)
                .Replace("@pushed", isForcePush ? ForcePushed : Pushed)
                .Replace("@repoUri", repoUri)
                .Replace("@projectName", projectName)
                .Replace("@repoName", repoName);
        }

        public string FormatCommitText(string action, string commitUri, string commitId, DateTime authorTime, string authorName, string comment)
        {
            string formattedTime = String.IsNullOrEmpty(DateTimeFormat) ? authorTime.ToString() : authorTime.ToString(DateTimeFormat);
            return CommitFormat
                .Replace("@action", action)
                .Replace("@commitUri", commitUri)
                .Replace("@commitId", commitId)
                .Replace("@authorTime", formattedTime)
                .Replace("@authorName", authorName)
                .Replace("@comment", comment);
        }

        public string FormatLinesSupressedText(int count)
        {
            return LinesSupressedFormat.Replace("@count", count.ToString());
        }

    }
}
