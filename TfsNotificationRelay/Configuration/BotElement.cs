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
    public class BotElement : ConfigurationElement, IKeyedConfigurationElement
    {
        public object Key { get { return Id; } }

        [ConfigurationProperty("id", IsRequired = true, IsKey = true)]
        public string Id
        {
            get { return (string)this["id"]; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)this["type"]; }
        }

        [ConfigurationProperty("textId", IsRequired = true)]
        public string TextId
        {
            get { return (string)this["textId"]; }
        }

        public TextElement Text { get; set; }

        [ConfigurationProperty("botSettings")]
        [ConfigurationCollection(typeof(NameValueConfigurationCollection))]
        protected NameValueConfigurationCollection BotSettingsConfigurationCollection
        {
            get { return (NameValueConfigurationCollection)base["botSettings"]; }
        }

        private Dictionary<string, string> _BotSettings;
        public Dictionary<string, string> BotSettings
        {
            get
            {
                if (_BotSettings != null) return _BotSettings;
                _BotSettings = new Dictionary<string, string>();
                foreach (NameValueConfigurationElement element in BotSettingsConfigurationCollection)
                {
                    _BotSettings.Add(element.Name, element.Value);
                }
                return _BotSettings;
            }
        }

        public string GetSetting(string name) 
        {
            return BotSettingsConfigurationCollection[name].Value;
        }

        public string GetSetting(string name, string fallback)
        {
            if (!BotSettingsConfigurationCollection.AllKeys.Contains(name)) return fallback;
            return BotSettingsConfigurationCollection[name].Value;
        }

        public IEnumerable<string> GetCsvSetting(string name, string fallback)
        {
            return TextHelper.SplitCsv(GetSetting(name, fallback));
        }

        public IEnumerable<string> GetCsvSetting(string name)
        {
            return TextHelper.SplitCsv(GetSetting(name));
        }

        [ConfigurationProperty("eventRules")]
        [ConfigurationCollection(typeof(EventRuleCollection),
            AddItemName = "rule")]
        public EventRuleCollection EventRules
        {
            get { return (EventRuleCollection)base["eventRules"]; }
        }
    }
}
