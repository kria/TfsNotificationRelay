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

using DevCore.TfsNotificationRelay.Configuration;
using Microsoft.TeamFoundation.Git.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.Notifications.GitPush
{
    public class CommitRow : NotificationRow
    {
        public byte[] CommitId { get; set; }
        public CommitRowType Type { get; set; }
        public string CommitUri { get; set; }
        public Dictionary<TfsGitChangeType, int> ChangeCounts { get; set; }
        public DateTime AuthorTime { get; set; }
        public string Author { get; set; }
        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public string Comment { get; set; }
        public IList<string> RefNames { get; set; }

        public override string ToString(BotElement bot, Func<string, string> transform)
        {
            string formattedTime = String.IsNullOrEmpty(bot.Text.DateTimeFormat) ? AuthorTime.ToString() : AuthorTime.ToString(bot.Text.DateTimeFormat);
            var sb = new StringBuilder();
            if (RefNames != null) sb.AppendFormat("{0} ", transform(String.Concat(RefNames)));
            
            sb.Append(bot.Text.CommitFormat.FormatWith(new
            {
                Action = Type == CommitRowType.Commit ? bot.Text.Commit : bot.Text.RefPointer,
                CommitUri = CommitUri,
                CommitId = transform(CommitId.ToHexString(settings.HashLength)),
                ChangeCounts = (ChangeCounts != null) ? String.Join(", ", ChangeCounts.Select(c => ChangeCountToString(bot, c))) : "",
                AuthorTime = formattedTime,
                Author = transform(Author),
                AuthorName = transform(AuthorName),
                AuthorEmail = transform(AuthorEmail),
                Comment = transform(Comment.Truncate(settings.CommentMaxLength))
            }));

            return sb.ToString();
        }

        private static string ChangeCountToString(BotElement bot, KeyValuePair<TfsGitChangeType, int> changeCount)
        {
            string format = null;
            switch (changeCount.Key)
            {
                case TfsGitChangeType.Add: format = bot.Text.ChangeCountAddFormat; break;
                case TfsGitChangeType.Delete: format = bot.Text.ChangeCountDeleteFormat; break;
                case TfsGitChangeType.Edit: format = bot.Text.ChangeCountEditFormat; break;
                case TfsGitChangeType.Rename: format = bot.Text.ChangeCountRenameFormat; break;
                case TfsGitChangeType.SourceRename: format = bot.Text.ChangeCountSourceRenameFormat; break;
                default: 
                    Logger.Log("Unknown combo: " + changeCount.Key);
                    format = bot.Text.ChangeCountUnknownFormat;
                    break;
            }
            return format.FormatWith(new { Count = changeCount.Value });
        }
    }

    public enum CommitRowType
    {
        Commit,
        RefUpdate
    }
}
