//-----------------------------------------------------------------------
// <copyright file="OptionType.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Model.Options
{
    /// <summary>
    /// The different types of options.
    /// </summary>
    public enum OptionType
    {
        /// <summary>
        /// A UCI option that has a value of either true or false.
        /// </summary>
        Boolean,

        /// <summary>
        /// A UCI option that is a free-form number
        /// within an upper bound and/or lower bound.
        /// </summary>
        NumberRange,

        /// <summary>
        /// A UCI option with a enumerated set of valid
        /// options to choose from.
        /// </summary>
        MultipleChoice,

        /// <summary>
        /// A UCI option that can trigger a command to be
        /// sent to the engine.
        /// </summary>
        Button,

        /// <summary>
        /// A free-form string option.
        /// </summary>
        Text,
    }
}