// Copyright (c) Bruno Alfirević. All rights reserved.
// Licensed under the MIT license. See license.txt in the project root for license information.

namespace BinaryFactor.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string ReplaceAt(this string str, string replacement, int startIndex, int count)
        {
            return str
                .Remove(startIndex, count)
                .Insert(startIndex, replacement);
        }

        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            if (str1 == null && str2 == null)
                return true;

            if (str1 == null || str2 == null)
                return false;

            return str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string str, string value)
        {
            return str.IndexOf(value, StringComparison.OrdinalIgnoreCase) > 0;
        }

        public static IList<string> SplitLines(this string str, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            return str.Split(new[] { "\r\n", "\r", "\n" }, stringSplitOptions);
        }

        public static IList<string> SplitByRegex(this string str, Regex regex, string groupName = null, StringSplitOptions stringSplitOptions = StringSplitOptions.None)
        {
            var matches = regex.Matches(str)
                .OfType<Match>()
                .Where(match => groupName == null ? match.Success : match.Groups[groupName].Success)
                .ToList();

            if (matches.Count == 0)
                return new List<string> { str };

            var result = new List<string>();

            var lastIndex = 0;
            foreach (var match in matches)
            {
                var stringPart = str.Substring(lastIndex, match.Index - lastIndex);

                if (stringSplitOptions != StringSplitOptions.RemoveEmptyEntries || !string.IsNullOrEmpty(stringPart))
                    result.Add(stringPart);

                lastIndex = match.Index + match.Length;
            }

            var finalPart = str.Substring(lastIndex, str.Length - lastIndex);
            if (stringSplitOptions != StringSplitOptions.RemoveEmptyEntries || !string.IsNullOrEmpty(finalPart))
                result.Add(finalPart);

            return result;
        }

        public static string NullifyIfWhitespace(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? null : str;
        }

        public static string Reverse(this string str)
        {
            var charArray = str.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string RemoveCommonIndentation(this string str, int tabWidth = 4, string ambientIndentation = "")
        {

            static int CalculateBaselineIndentationLength(IList<string> lines, int tabWidth, bool removedEmptyFirstLine)
            {
                if (removedEmptyFirstLine)
                {
                    return CalculateIndentationLength(
                        lines
                            .Skip(1)
                            .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s)) ?? "",
                        tabWidth);
                }
                else
                {
                    return lines
                        .Skip(1)
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(line => CalculateIndentationLength(line, tabWidth))
                        .DefaultIfEmpty()
                        .Min();
                }
            }

            static string AddIndentation(string str, string indentation)
            {
                return indentation + str;
            }

            static string RemoveIndentation(string line, int indentationLengthToRemove, int tabWidth)
            {
                var i = 0;
                var indentationLength = 0;

                while (i < line.Length && char.IsWhiteSpace(line[i]) && indentationLength < indentationLengthToRemove)
                {
                    indentationLength += CharWidth(line[i], indentationLength, tabWidth);
                    i++;
                }

                return line.Remove(0, i);
            }

            static int CalculateIndentationLength(string line, int tabWidth)
            {
                var i = 0;
                var result = 0;

                while (i < line.Length && char.IsWhiteSpace(line[i]))
                {
                    result += CharWidth(line[i], result, tabWidth);
                    i++;
                }

                return result;
            }

            static int CharWidth(char c, int indentation, int tabWidth)
            {
                return c == '\t'
                    ? tabWidth - (indentation % tabWidth)
                    : 1;
            }

            var stringLines = str.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            if (stringLines.Length == 0)
                return "";

            if (stringLines.Length == 1)
                return stringLines[0];

            var removedEmptyFirstLine = false;
            var firstIndex = 0;
            if (string.IsNullOrWhiteSpace(stringLines[0]))
            {
                removedEmptyFirstLine = true;
                firstIndex = 1;
            }

            var baselineIndentationLength = CalculateBaselineIndentationLength(stringLines, tabWidth, removedEmptyFirstLine);

            for (var i = firstIndex; i < stringLines.Length; i++)
            {
                var line = stringLines[i];

                if (removedEmptyFirstLine || i > firstIndex)
                    line = RemoveIndentation(line, baselineIndentationLength, tabWidth);

                if (i > firstIndex && !string.IsNullOrWhiteSpace(line))
                    line = AddIndentation(line, ambientIndentation);

                stringLines[i] = line;
            }

            return string.Join(Environment.NewLine, stringLines, firstIndex, stringLines.Length - firstIndex);
        }
    }
}
