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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.Configuration
{
    public class TfsNotificationRelaySection : ConfigurationSection
    {
        private static TfsNotificationRelaySection _instance;

        public static TfsNotificationRelaySection Instance
        {
            get 
            {
                if (_instance == null) _instance = ConfigurationHelper.GetConfigurationSection<TfsNotificationRelaySection>(Assembly.GetExecutingAssembly(), "tfsNotificationRelay");
                               
                return _instance;
            }
        }
        [ConfigurationProperty("settings")]
        public SettingsElement Settings => (SettingsElement)this["settings"];

        [ConfigurationProperty("bots")]
        [ConfigurationCollection(typeof(ConfigurationElementCollection<BotElement>),
            AddItemName = "bot")]
        public ConfigurationElementCollection<BotElement> Bots => (ConfigurationElementCollection<BotElement>)base["bots"];

        [ConfigurationProperty("texts")]
        [ConfigurationCollection(typeof(ConfigurationElementCollection<TextElement>),
            AddItemName = "text")]
        public ConfigurationElementCollection<TextElement> Texts => (ConfigurationElementCollection<TextElement>)base["texts"];

        [ConfigurationProperty("userMaps")]
        [ConfigurationCollection(typeof(ConfigurationElementCollection<UserMapElement>),
            AddItemName = "userMap")]
        public ConfigurationElementCollection<UserMapElement> UserMaps => (ConfigurationElementCollection<UserMapElement>)base["userMaps"];

        [ConfigurationProperty("xmlns")]
        public string Xmlns => (string)this["xmlns"];

        [ConfigurationProperty("xmlns:xsi")]
        public string XmlnsXsi => (string)this["xmlns:xsi"];

        [ConfigurationProperty("xsi:noNamespaceSchemaLocation")]
        public string XsiNoNamespaceSchemaLocation => (string)this["xsi:noNamespaceSchemaLocation"];

        protected override void PostDeserialize()
        {
            base.PostDeserialize();
            
            foreach (var bot in Bots)
            {
                // Link up inheritance first
                if (!string.IsNullOrEmpty(bot.BasedOn))
                {
                    var baseBot = Bots.FirstOrDefault(b => b.Id == bot.BasedOn);
                    if (baseBot == null) throw new TfsNotificationRelayException($"Unknown basedOn ({bot.BasedOn}) for bot {bot.Id}");
                    bot.BaseBot = baseBot;
                }

                var text = Texts.FirstOrDefault(t => t.Id == bot.TextId);
                if (text == null) throw new TfsNotificationRelayException($"Unknown textId ({bot.TextId}) for bot {bot.Id}");

                bot.Text = text;
                
                if (!string.IsNullOrEmpty(bot.UserMapId))
                {
                    var userMap = UserMaps.FirstOrDefault(m => m.Id == bot.UserMapId);
                    if (userMap == null) throw new TfsNotificationRelayException($"Unknown userMapId ({bot.UserMapId}) for bot {bot.Id}");
                    bot.UserMap = userMap;
                }
                else
                {
                    bot.UserMap = new UserMapElement();
                }
            }
        }

    }
}
