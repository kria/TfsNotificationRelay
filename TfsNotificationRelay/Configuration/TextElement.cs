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
        public object Key => Id;

        [ConfigurationProperty("id", IsRequired= true)]
        public string Id => (string)this["id"];

        [ConfigurationProperty("pushFormat")]
        public string PushFormat => (string)this["pushFormat"];

        [ConfigurationProperty("pushed")]
        public string Pushed => (string)this["pushed"];

        [ConfigurationProperty("forcePushed")]
        public string ForcePushed => (string)this["forcePushed"];

        [ConfigurationProperty("commit")]
        public string Commit => (string)this["commit"];

        [ConfigurationProperty("refPointer")]
        public string RefPointer => (string)this["refPointer"];

        [ConfigurationProperty("deleted")]
        public string Deleted => (string)this["deleted"];

        [ConfigurationProperty("commitFormat")]
        public string CommitFormat => (string)this["commitFormat"];

        [ConfigurationProperty("linesSupressedFormat")]
        public string LinesSupressedFormat => (string)this["linesSupressedFormat"];

        [ConfigurationProperty("dateTimeFormat")]
        public string DateTimeFormat => (string)this["dateTimeFormat"];

        [ConfigurationProperty("timeSpanFormat")]
        public string TimeSpanFormat => (string)this["timeSpanFormat"];

        [ConfigurationProperty("buildFormat")]
        public string BuildFormat => (string)this["buildFormat"];

        [ConfigurationProperty("build2015Format")]
        public string Build2015Format => (string)this["build2015Format"];

        [ConfigurationProperty("buildQualityChangedFormat")]
        public string BuildQualityChangedFormat => (string)this["buildQualityChangedFormat"];

        [ConfigurationProperty("buildQualityNotSet")]
        public string BuildQualityNotSet => (string)this["buildQualityNotSet"];

        [ConfigurationProperty("projectCreatedFormat")]
        public string ProjectCreatedFormat => (string)this["projectCreatedFormat"];

        [ConfigurationProperty("projectDeletedFormat")]
        public string ProjectDeletedFormat => (string)this["projectDeletedFormat"];

        [ConfigurationProperty("checkinFormat")]
        public string CheckinFormat => (string)this["checkinFormat"];

        [ConfigurationProperty("projectLinkFormat")]
        public string ProjectLinkFormat => (string)this["projectLinkFormat"];

        [ConfigurationProperty("changeCountAddFormat")]
        public string ChangeCountAddFormat => (string)this["changeCountAddFormat"];

        [ConfigurationProperty("changeCountDeleteFormat")]
        public string ChangeCountDeleteFormat => (string)this["changeCountDeleteFormat"];

        [ConfigurationProperty("changeCountEditFormat")]
        public string ChangeCountEditFormat => (string)this["changeCountEditFormat"];

        [ConfigurationProperty("changeCountRenameFormat")]
        public string ChangeCountRenameFormat => (string)this["changeCountRenameFormat"];

        [ConfigurationProperty("changeCountSourceRenameFormat")]
        public string ChangeCountSourceRenameFormat => (string)this["changeCountSourceRenameFormat"];

        [ConfigurationProperty("changeCountUnknownFormat")]
        public string ChangeCountUnknownFormat => (string)this["changeCountUnknownFormat"];

        [ConfigurationProperty("workItemchangedFormat")]
        public string WorkItemchangedFormat => (string)this["workItemchangedFormat"];

        [ConfigurationProperty("updated")]
        public string Updated => (string)this["updated"];

        [ConfigurationProperty("created")]
        public string Created => (string)this["created"];

        [ConfigurationProperty("commentedOn")]
        public string CommentedOn => (string)this["commentedOn"];

        [ConfigurationProperty("comment")]
        public string Comment => (string)this["comment"];

        [ConfigurationProperty("state")]
        public string State => (string)this["state"];

        [ConfigurationProperty("assignedTo")]
        public string AssignedTo => (string)this["assignedTo"];

        [ConfigurationProperty("pullRequestCreatedFormat")]
        public string PullRequestCreatedFormat => (string)this["pullRequestCreatedFormat"];

        [ConfigurationProperty("pullRequestStatusUpdateFormat")]
        public string PullRequestStatusUpdateFormat => (string)this["pullRequestStatusUpdateFormat"];

        [ConfigurationProperty("pullRequestReviewerVoteFormat")]
        public string PullRequestReviewerVoteFormat => (string)this["pullRequestReviewerVoteFormat"];

        [ConfigurationProperty("voteApproved")]
        public string VoteApproved => (string)this["voteApproved"];

        [ConfigurationProperty("voteRejected")]
        public string VoteRejected => (string)this["voteRejected"];

        [ConfigurationProperty("voteRescinded")]
        public string VoteRescinded => (string)this["voteRescinded"];

        [ConfigurationProperty("completed")]
        public string Completed => (string)this["completed"];

        [ConfigurationProperty("abandoned")]
        public string Abandoned => (string)this["abandoned"];

        [ConfigurationProperty("reactivated")]
        public string Reactivated => (string)this["reactivated"];

        [ConfigurationProperty("repositoryCreatedFormat")]
        public string RepositoryCreatedFormat => (string)this["repositoryCreatedFormat"];

        [ConfigurationProperty("repositoryRenamedFormat")]
        public string RepositoryRenamedFormat => (string)this["repositoryRenamedFormat"];

        [ConfigurationProperty("repositoryDeletedFormat")]
        public string RepositoryDeletedFormat => (string)this["repositoryDeletedFormat"];

        [ConfigurationProperty("workItemFieldTransitionFormat")]
        public string WorkItemFieldTransitionFormat => (string)this["workItemFieldTransitionFormat"];

        [ConfigurationProperty("branchFormat")]
        public string BranchFormat => (string)this["branchFormat"];

        [ConfigurationProperty("tagFormat")]
        public string TagFormat => (string)this["tagFormat"];

        [ConfigurationProperty("refSeparator")]
        public string RefSeparator => (string)this["refSeparator"];

        [ConfigurationProperty("commitCommentFormat")]
        public string CommitCommentFormat => (string)this["commitCommentFormat"];

        [ConfigurationProperty("pullRequestCommentFormat")]
        public string PullRequestCommentFormat => (string)this["pullRequestCommentFormat"];

        [ConfigurationProperty("changesetCommentFormat")]
        public string ChangesetCommentFormat => (string)this["changesetCommentFormat"];

        [ConfigurationProperty("releaseCreatedFormat")]
        public string ReleaseCreatedFormat => (string)this["releaseCreatedFormat"];

        [ConfigurationProperty("releaseEnvironmentCompletedFormat")]
        public string ReleaseEnvironmentCompletedFormat => (string)this["releaseEnvironmentCompletedFormat"];
    }
}
