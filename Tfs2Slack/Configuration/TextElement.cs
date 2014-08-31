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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.Tfs2Slack.Configuration
{
    public class TextElement : ConfigurationElement
    {
        [ConfigurationProperty("pushFormat")]
        public string PushFormat
        {
            get { return (string)this["pushFormat"]; }
        }

        [ConfigurationProperty("pushed")]
        public string Pushed
        {
            get { return (string)this["pushed"]; }
        }

        [ConfigurationProperty("forcePushed")]
        public string ForcePushed
        {
            get { return (string)this["forcePushed"]; }
        }

        [ConfigurationProperty("commit")]
        public string Commit
        {
            get { return (string)this["commit"]; }
        }

        [ConfigurationProperty("refPointer")]
        public string RefPointer
        {
            get { return (string)this["refPointer"]; }
        }

        [ConfigurationProperty("deleted")]
        public string Deleted
        {
            get { return (string)this["deleted"]; }
        }

        [ConfigurationProperty("commitFormat")]
        public string CommitFormat
        {
            get { return (string)this["commitFormat"]; }
        }

        [ConfigurationProperty("linesSupressedFormat")]
        public string LinesSupressedFormat
        {
            get { return (string)this["linesSupressedFormat"]; }
        }

        [ConfigurationProperty("dateTimeFormat")]
        public string DateTimeFormat
        {
            get { return (string)this["dateTimeFormat"]; }
        }

        [ConfigurationProperty("buildFormat")]
        public string BuildFormat
        {
            get { return (string)this["buildFormat"]; }
        }

        [ConfigurationProperty("projectCreatedFormat")]
        public string ProjectCreatedFormat
        {
            get { return (string)this["projectCreatedFormat"]; }
        }

        [ConfigurationProperty("projectDeletedFormat")]
        public string ProjectDeletedFormat
        {
            get { return (string)this["projectDeletedFormat"]; }
        }

        public string FormatPushText(string userName, string repoUri, string projectName, string repoName, bool isForcePush)
        {
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

        public string FormatBuildText(string buildUrl, string projectName, string buildNumber, string buildStatus)
        {
            return BuildFormat
                .Replace("@buildUrl", buildUrl)
                .Replace("@projectName", projectName)
                .Replace("@buildNumber", buildNumber)
                .Replace("@buildStatus", buildStatus);
        }

        public string FormatProjectCreatedText(string projectUrl, string projectName)
        {
            return ProjectCreatedFormat
                .Replace("@projectUrl", projectUrl)
                .Replace("@projectName", projectName);
        }

        public string FormatProjectDeletedText(string projectUri)
        {
            return ProjectDeletedFormat.Replace("@projectUri", projectUri);
        }
    }
}
