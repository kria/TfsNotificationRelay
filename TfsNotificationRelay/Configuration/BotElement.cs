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

        [ConfigurationProperty("type")]
        public string Type
        {
            get { return GetProperty("type"); }
        }

        [ConfigurationProperty("textId")]
        public string TextId
        {
            get { return GetProperty("textId"); }
        }

        public TextElement Text { get; set; }

        [ConfigurationProperty("userMapId")]
        public string UserMapId
        {
            get { return GetProperty("userMapId"); }
        }

        public UserMapElement UserMap { get; set; }

        public string GetMappedUser(string tfsUserName)
        {
            var userMapping = UserMap.FirstOrDefault(u => u.TfsUser == tfsUserName);
            return userMapping != null ? userMapping.MappedUser : null;
        }

        [ConfigurationProperty("basedOn")]
        public string BasedOn
        {
            get { return (string)this["basedOn"]; }
        }

        public BotElement BaseBot { get; set; }

        [ConfigurationProperty("inheritRules")]
        public bool InheritRules
        {
            get { return GetProperty<bool>("inheritRules"); }
        }

        [ConfigurationProperty("botSettings")]
        [ConfigurationCollection(typeof(NameValueConfigurationCollection))]
        protected NameValueConfigurationCollection BotSettingsConfigurationCollection
        {
            get { return (NameValueConfigurationCollection)this["botSettings"]; }
        }

        public string GetSetting(string name) 
        {
            var setting = BotSettingsConfigurationCollection[name];
            if (setting != null) return setting.Value;
            if (BaseBot == null) return null;
            return BaseBot.GetSetting(name);
        }

        public string GetSetting(string name, string fallback)
        {
            return GetSetting(name) ?? fallback;
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
        protected EventRuleCollection EventRules
        {
            get { return (EventRuleCollection)this["eventRules"]; }
        }

        /// <summary>
        /// Get all rules for whole basedOn hierarchy, starting from root ancestor
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EventRuleElement> GetRules()
        {
            if (InheritRules && BaseBot != null)
            {
                foreach (var rule in BaseBot.GetRules())
                    yield return rule;
            }
            foreach (var rule in EventRules)
                yield return rule;
        }

        protected string GetProperty(string propertyName)
        {
            var propertyValue = (string)this[propertyName];

            // We have to check for empty string since missing string elements are deserialized as string.Empty!?
            if (!string.IsNullOrEmpty(propertyValue) || BaseBot == null)
            {
                return propertyValue;
            }

            return BaseBot.GetProperty(propertyName);
        }

        protected T GetProperty<T>(string propertyName)
        {
            var propertyValue = (T)this[propertyName];

            if (propertyValue != null || BaseBot == null)
            {
                return propertyValue;
            }

            return BaseBot.GetProperty<T>(propertyName);
        }

    }
}
