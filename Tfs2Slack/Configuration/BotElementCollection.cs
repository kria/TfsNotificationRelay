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
    public class BotElementCollection : ConfigurationElementCollection,
        IEnumerable<BotElement>
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new BotElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((BotElement)element).Name;
        }
        public BotElement this[int index]
        {
            get { return (BotElement)BaseGet(index); }
        }

        public new IEnumerator<BotElement> GetEnumerator()
        {
            int count = base.Count;
            for (int i = 0; i < count; i++)
            {
                yield return base.BaseGet(i) as BotElement;
            }
        }

    }
}
