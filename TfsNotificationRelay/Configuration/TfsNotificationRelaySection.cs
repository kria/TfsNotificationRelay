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
        private static TfsNotificationRelaySection instance;

        public static TfsNotificationRelaySection Instance
        {
            get 
            {
                if (instance == null) instance = ConfigurationHelper.GetConfigurationSection<TfsNotificationRelaySection>(Assembly.GetExecutingAssembly(), "tfsNotificationRelay");
                               
                return instance;
            }
        }
        [ConfigurationProperty("settings")]
        public SettingsElement Settings
        {
            get { return (SettingsElement)this["settings"]; }
        }

        [ConfigurationProperty("bots")]
        [ConfigurationCollection(typeof(ConfigurationElementCollection<BotElement>),
            AddItemName = "bot")]
        public ConfigurationElementCollection<BotElement> Bots
        {
            get { return (ConfigurationElementCollection<BotElement>)base["bots"]; }
        }

        [ConfigurationProperty("texts")]
        [ConfigurationCollection(typeof(ConfigurationElementCollection<TextElement>),
            AddItemName = "text")]
        public ConfigurationElementCollection<TextElement> Texts
        {
            get { return (ConfigurationElementCollection<TextElement>)base["texts"]; }
        }

        [ConfigurationProperty("xmlns")]
        public string Xmlns
        {
            get { return (string)this["xmlns"]; }
        }

        [ConfigurationProperty("xmlns:xsi")]
        public string XmlnsXsi
        {
            get { return (string)this["xmlns:xsi"]; }
        }

        [ConfigurationProperty("xsi:noNamespaceSchemaLocation")]
        public string XsiNoNamespaceSchemaLocation
        {
            get { return (string)this["xsi:noNamespaceSchemaLocation"]; }
        }

        protected override void PostDeserialize()
        {
            base.PostDeserialize();
            
            foreach (var bot in Bots)
            {
                var text = this.Texts.FirstOrDefault(t => t.Id == bot.TextId);
                if (text == null) throw new TfsNotificationRelayException(String.Format("Unknown textId ({0}) for bot {1}", bot.TextId, bot.Id));

                bot.Text = text;
            }
        }

    }
}
