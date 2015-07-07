/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2015 Kristian Adrup
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
    public static class TextHelper
    {
        public static string Truncate(this string text, int len)
        {
            text = text.TrimEnd(Environment.NewLine.ToCharArray());
            int pos = text.IndexOf('\n');
            if (pos > 0 && pos <= len)
                return text.Substring(0, pos);
            if (text.Length <= len)
                return text;
            pos = text.LastIndexOf(' ', len);
            if (pos == -1) pos = len;

            return text.Substring(0, pos) + "...";
        }

        public static string StripDomain(string username)
        {
            int pos = username.IndexOf("\\");
            return pos != -1 ? username.Substring(pos + 1) : username;
        }

        /// <summary>
        /// String formatting with named variables <see href="http://james.newtonking.com/archive/2008/03/29/formatwith-2-0-string-formatting-with-named-variables" />
        /// </summary>
        /// <param name="format"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FormatWith(this string format, object source)
        {
            return FormatWith(format, null, source);
        }

        /// <summary>
        /// String formatting with named variables <see href="http://james.newtonking.com/archive/2008/03/29/formatwith-2-0-string-formatting-with-named-variables" />
        /// </summary>
        /// <param name="format"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FormatWith(this string format, IFormatProvider provider, object source)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            Regex r = new Regex(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
              RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            List<object> values = new List<object>();
            string rewrittenFormat = r.Replace(format, delegate (Match m)
            {
                Group startGroup = m.Groups["start"];
                Group propertyGroup = m.Groups["property"];
                Group formatGroup = m.Groups["format"];
                Group endGroup = m.Groups["end"];

                values.Add((propertyGroup.Value == "0")
                  ? source
                  : DataBinder.Eval(source, propertyGroup.Value));

                return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value
                  + new string('}', endGroup.Captures.Count);
            });

            return string.Format(provider, rewrittenFormat, values.ToArray());
        }

        public static bool IsMatchOrNoPattern(this string input, string pattern)
        {
            return String.IsNullOrEmpty(pattern) || Regex.IsMatch(input, pattern);
        }

        public static bool IsMatchOrNoPattern(this IEnumerable<string> input, string pattern)
        {
            return String.IsNullOrEmpty(pattern) || input.Any(n => Regex.IsMatch(n, pattern));
        }

        public static string HtmlToText(string html)
        {
            var converter = new HtmlToText();
            return converter.ConvertHtml(html);
        }
        public static IEnumerable<string> SplitCsv(string value)
        {
            return value.Split(',').Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f));
        }
    }
}
