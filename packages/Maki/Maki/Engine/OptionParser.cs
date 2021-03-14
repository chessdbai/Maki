//-----------------------------------------------------------------------
// <copyright file="OptionParser.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Engine
{
    using System;
    using System.Collections.Generic;
    using Maki.Model;
    using Maki.Model.Options;

    /// <summary>
    /// A utility class to parse UCI options.
    /// </summary>
    public class OptionParser
    {
        #pragma warning disable SA1306
        private static Dictionary<string, Action<EngineOption, string>> KeywordHandlers = new Dictionary<string, Action<EngineOption, string>>(new[]
        {
            new KeyValuePair<string, Action<EngineOption, string>>("name", (op, str) => op.Name = str),
            new KeyValuePair<string, Action<EngineOption, string>>("type", (op, str) => op.Type = UciOptionTypeToOptionType(str)),
            new KeyValuePair<string, Action<EngineOption, string>>("default", (op, str) => op.DefaultValue = UciDefaultToOptionValue(op.Type, str)),
            new KeyValuePair<string, Action<EngineOption, string>>("min", (op, str) => op.Minimum = int.Parse(str)),
            new KeyValuePair<string, Action<EngineOption, string>>("max", (op, str) => op.Maximum = int.Parse(str)),
            new KeyValuePair<string, Action<EngineOption, string>>("var", (op, str) =>
            {
                if (op.ValidValues == null)
                {
                    op.ValidValues = new List<string>();
                }

                op.ValidValues.Add(str);
            }),
        });
        #pragma warning restore SA1306

        /// <summary>
        /// Parses a UCI option line in the UCI init header to a EngineOption object.
        /// </summary>
        /// <param name="optionLine">The string option line.</param>
        /// <returns>The parsed EngineOption.</returns>
        public static EngineOption ParseOptionLine(string optionLine)
        {
            if (!optionLine.ToLower().StartsWith("option"))
            {
                throw new ArgumentException($"Option line should start with 'option'.");
            }

            var option = new EngineOption();
            var words = optionLine.Split(' ');
            var buffer = new List<string>();
            string keyword = null;
            foreach (var word in words)
            {
                if (KeywordHandlers.ContainsKey(word.ToLower()))
                {
                    if (buffer.Count > 0 && keyword != null)
                    {
                        string keywordVal = string.Join(' ', buffer);
                        KeywordHandlers[keyword](option, keywordVal);
                    }

                    buffer.Clear();
                    keyword = word;
                }
                else
                {
                    buffer.Add(word);
                }
            }

            if (buffer.Count > 0 && keyword != null)
            {
                string keywordVal = string.Join(' ', buffer);
                KeywordHandlers[keyword](option, keywordVal);
            }

            return option;
        }

        /// <summary>
        /// Converts the UCI option type name to the OptionType enum.
        /// </summary>
        /// <param name="uciOptionType">The UCI option type name.</param>
        /// <returns>The corresponding <see cref="OptionType"/>.</returns>
        internal static OptionType UciOptionTypeToOptionType(string uciOptionType) => uciOptionType.ToLower() switch
        {
            "check" => OptionType.Boolean,
            "button" => OptionType.Button,
            "string" => OptionType.Text,
            "combo" => OptionType.MultipleChoice,
            "spin" => OptionType.NumberRange,
            _ => throw new ArgumentException($"Unknown UCI option type '{uciOptionType}'."),
        };

        /// <summary>
        /// Convert a default value string to its object value representation.
        /// </summary>
        /// <param name="optionType">The option type, which gives this function the method with which to convert the string to the option value.</param>
        /// <param name="defaultStr">The default value string.</param>
        /// <returns>The option value.</returns>
        internal static OptionValue UciDefaultToOptionValue(OptionType optionType, string defaultStr)
        {
            if (optionType == OptionType.Boolean)
            {
                return new OptionValue()
                {
                    BooleanValue = bool.Parse(defaultStr),
                };
            }

            if (optionType == OptionType.Text || optionType == OptionType.MultipleChoice)
            {
                return new OptionValue()
                {
                    TextValue = defaultStr == "<empty>" ? string.Empty : defaultStr,
                };
            }

            if (optionType == OptionType.NumberRange)
            {
                return new OptionValue()
                {
                    IntegerValue = int.Parse(defaultStr),
                };
            }

            return null;
        }
    }
}