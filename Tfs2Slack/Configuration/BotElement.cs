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
    public class BotElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("slackWebhookUrl")]
        public string SlackWebhookUrl
        {
            get { return (string)this["slackWebhookUrl"]; }
        }

        [ConfigurationProperty("slackChannels", IsRequired = true)]
        public string SlackChannels
        {
            get { return (string)this["slackChannels"]; }
        }

        [ConfigurationProperty("slackUsername")]
        public string SlackUsername
        {
            get { return (string)this["slackUsername"]; }
        }

        [ConfigurationProperty("slackIconEmoji")]
        public string SlackIconEmoji
        {
            get { return (string)this["slackIconEmoji"]; }
        }

        [ConfigurationProperty("slackIconUrl")]
        public string SlackIconUrl
        {
            get { return (string)this["slackIconUrl"]; }
        }

        [ConfigurationProperty("slackColor")]
        public string SlackColor
        {
            get { return (string)this["slackColor"]; }
        }

        [ConfigurationProperty("successColor")]
        public string SuccessColor
        {
            get { return (string)this["successColor"]; }
        }

        [ConfigurationProperty("errorColor")]
        public string ErrorColor
        {
            get { return (string)this["errorColor"]; }
        }

        [ConfigurationProperty("eventRules", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(EventRuleCollection),
            AddItemName = "rule")]
        public EventRuleCollection EventRules
        {
            get { return (EventRuleCollection)base["eventRules"]; }
        }
    }
}
