//-----------------------------------------------------------------------
// <copyright file="OptionExtensions.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Engine.UCI
{
    using System;
    using Maki.Model;

    /// <summary>
    /// Class containing extension methods on UCI <see cref="OptionValue" />s.
    /// </summary>
    public static class OptionExtensions
    {
        /// <summary>
        /// Gets the UCI option value string for an option value.
        /// </summary>
        /// <param name="optionValue">The option value.</param>
        /// <returns>The UCI string.</returns>
        public static string ToUciValueString(this OptionValue optionValue)
        {
            if (!string.IsNullOrEmpty(optionValue.TextValue))
            {
                return optionValue.TextValue;
            }

            if (optionValue.BooleanValue != null)
            {
                return optionValue.BooleanValue.Value.ToString();
            }

            if (optionValue.IntegerValue != null)
            {
                return optionValue.IntegerValue.Value.ToString();
            }

            throw new ArgumentException($"The given option value had no value set.");
        }
    }
}