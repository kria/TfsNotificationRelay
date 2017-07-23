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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;

namespace DevCore.TfsNotificationRelay
{
    public static class Utils
    {
        public static string ToHexString(this byte[] buffer)
        {
            return BitConverter.ToString(buffer).Replace("-", "").ToLower();
        }

        public static string ToHexString(this byte[] buffer, int length)
        {
            return buffer.ToHexString().Substring(0, length);
        }

        public static string ToShortHexString(this byte[] buffer)
        {
            return buffer.ToHexString().Substring(0, 7);
        }

        public static bool IsZero(this byte[] buffer)
        {
            return buffer.All(b => b == 0);
        }

        public static string Dump(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
