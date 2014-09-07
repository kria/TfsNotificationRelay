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

        [ConfigurationProperty("checkinFormat")]
        public string CheckinFormat
        {
            get { return (string)this["checkinFormat"]; }
        }

        [ConfigurationProperty("changeCountAddFormat")]
        public string ChangeCountAddFormat
        {
            get { return (string)this["changeCountAddFormat"]; }
        }

        [ConfigurationProperty("changeCountDeleteFormat")]
        public string ChangeCountDeleteFormat
        {
            get { return (string)this["changeCountDeleteFormat"]; }
        }

        [ConfigurationProperty("changeCountEditFormat")]
        public string ChangeCountEditFormat
        {
            get { return (string)this["changeCountEditFormat"]; }
        }

        [ConfigurationProperty("changeCountRenameFormat")]
        public string ChangeCountRenameFormat
        {
            get { return (string)this["changeCountRenameFormat"]; }
        }

        [ConfigurationProperty("changeCountSourceRenameFormat")]
        public string ChangeCountSourceRenameFormat
        {
            get { return (string)this["changeCountSourceRenameFormat"]; }
        }

        [ConfigurationProperty("workItemchangedFormat")]
        public string WorkItemchangedFormat
        {
            get { return (string)this["workItemchangedFormat"]; }
        }
        
    }
}
