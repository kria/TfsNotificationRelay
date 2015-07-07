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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.Configuration
{
    public class TextElement : ConfigurationElement, IKeyedConfigurationElement
    {
        public object Key { get { return Id; } }

        [ConfigurationProperty("id", IsRequired= true)]
        public string Id
        {
            get { return (string)this["id"]; }
        }

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

        [ConfigurationProperty("timeSpanFormat")]
        public string TimeSpanFormat
        {
            get { return (string)this["timeSpanFormat"]; }
        }

        [ConfigurationProperty("buildFormat")]
        public string BuildFormat
        {
            get { return (string)this["buildFormat"]; }
        }

        [ConfigurationProperty("buildQualityChangedFormat")]
        public string BuildQualityChangedFormat
        {
            get { return (string)this["buildQualityChangedFormat"]; }
        }

        [ConfigurationProperty("buildQualityNotSet")]
        public string BuildQualityNotSet
        {
            get { return (string)this["buildQualityNotSet"]; }
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

        [ConfigurationProperty("projectLinkFormat")]
        public string ProjectLinkFormat
        {
            get { return (string)this["projectLinkFormat"]; }
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

        [ConfigurationProperty("changeCountUnknownFormat")]
        public string ChangeCountUnknownFormat
        {
            get { return (string)this["changeCountUnknownFormat"]; }
        }

        [ConfigurationProperty("workItemchangedFormat")]
        public string WorkItemchangedFormat
        {
            get { return (string)this["workItemchangedFormat"]; }
        }

        [ConfigurationProperty("updated")]
        public string Updated
        {
            get { return (string)this["updated"]; }
        }

        [ConfigurationProperty("created")]
        public string Created
        {
            get { return (string)this["created"]; }
        }

        [ConfigurationProperty("commentedOn")]
        public string CommentedOn
        {
            get { return (string)this["commentedOn"]; }
        }

        [ConfigurationProperty("comment")]
        public string Comment
        {
            get { return (string)this["comment"]; }
        }

        [ConfigurationProperty("state")]
        public string State
        {
            get { return (string)this["state"]; }
        }

        [ConfigurationProperty("assignedTo")]
        public string AssignedTo
        {
            get { return (string)this["assignedTo"]; }
        }

        [ConfigurationProperty("pullRequestCreatedFormat")]
        public string PullRequestCreatedFormat
        {
            get { return (string)this["pullRequestCreatedFormat"]; }
        }

        [ConfigurationProperty("pullRequestStatusUpdateFormat")]
        public string PullRequestStatusUpdateFormat
        {
            get { return (string)this["pullRequestStatusUpdateFormat"]; }
        }

        [ConfigurationProperty("pullRequestReviewerVoteFormat")]
        public string PullRequestReviewerVoteFormat
        {
            get { return (string)this["pullRequestReviewerVoteFormat"]; }
        }

        [ConfigurationProperty("voteApproved")]
        public string VoteApproved
        {
            get { return (string)this["voteApproved"]; }
        }

        [ConfigurationProperty("voteRejected")]
        public string VoteRejected
        {
            get { return (string)this["voteRejected"]; }
        }

        [ConfigurationProperty("voteRescinded")]
        public string VoteRescinded
        {
            get { return (string)this["voteRescinded"]; }
        }

        [ConfigurationProperty("completed")]
        public string Completed
        {
            get { return (string)this["completed"]; }
        }

        [ConfigurationProperty("abandoned")]
        public string Abandoned
        {
            get { return (string)this["abandoned"]; }
        }

        [ConfigurationProperty("reactivated")]
        public string Reactivated
        {
            get { return (string)this["reactivated"]; }
        }

        [ConfigurationProperty("repositoryCreatedFormat")]
        public string RepositoryCreatedFormat
        {
            get { return (string)this["repositoryCreatedFormat"]; }
        }

        [ConfigurationProperty("workItemFieldTransitionFormat")]
        public string WorkItemFieldTransitionFormat
        {
            get { return (string)this["workItemFieldTransitionFormat"]; }
        }
    }
}
