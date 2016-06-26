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
using DevCore.TfsNotificationRelay.Notifications;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.Smtp
{
    public class SmtpNotifier : INotifier
    {
        public Task NotifyAsync(TeamFoundationRequestContext requestContext, INotification notification, BotElement bot, EventRuleElement matchingRule)
        {
            if (!notification.TargetUserNames.Any())
                return Task.FromResult(0);

            var config = TfsNotificationRelaySection.Instance;
            string host = bot.GetSetting("host", "127.0.0.1");
            int port = bot.GetIntSetting("port", 25);
            string fromAddress = bot.GetSetting("fromAddress");
            string fromName = bot.GetSetting("fromName");
            string subjectTextId = bot.GetSetting("subjectTextId", "plaintext");
            bool isHtml = bot.GetSetting("isHtml") == "true";
            var subjectTextElement = config.Texts.FirstOrDefault(t => t.Id == subjectTextId) ?? bot.Text;
            string subject = notification.ToMessage(bot, subjectTextElement, s => s).First();

            var client = new SmtpClient(host, port);

            var message = new MailMessage();
            message.From = new MailAddress(fromAddress, fromName, Encoding.UTF8);
            message.SubjectEncoding = Encoding.UTF8;
            message.Subject = subject;
            message.IsBodyHtml = isHtml;
            message.BodyEncoding = Encoding.UTF8;
            message.Body = string.Join(isHtml ? "<br/>": "\n", notification.ToMessage(bot, s => s));

            var identityService = requestContext.GetService<ITeamFoundationIdentityService>();

            foreach (var username in notification.TargetUserNames)
            {
                var identity = identityService.ReadIdentity(requestContext, IdentitySearchFactor.AccountName, username);
                var email = identityService.GetPreferredEmailAddress(requestContext, identity.TeamFoundationId);
                if (string.IsNullOrEmpty(email))
                {
                    string errmsg = $"TfsNotificationRelay.Smtp.SmtpNotifier: User {username} doesn't have an email address.";
                    TeamFoundationApplicationCore.Log(requestContext, errmsg, 0, EventLogEntryType.Warning);
                }
                else
                {
                    message.To.Add(email);
                }
            }

            if (message.To.Any())
            {
                requestContext.Trace(0, TraceLevel.Info, Constants.TraceArea, "SmtpNotifier", 
                    string.Format("Sending {0} email notification to: {1}.", notification.GetType(), string.Join(", ", message.To.Select(m => m.Address))));

                return client.SendMailAsync(message);
            }
            else
            {
                requestContext.Trace(0, TraceLevel.Warning, Constants.TraceArea, "SmtpNotifier",
                    string.Format("No recipients to send {0} email notification to.", notification.GetType()));

                return Task.FromResult(0);
            }

            
        }
    }
}
