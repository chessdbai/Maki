//-----------------------------------------------------------------------
// <copyright file="EngineOption.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Model.Options
{
    using System.Collections.Generic;
    using ProtoBuf;

    /// <summary>
    /// An option that can be configured to
    /// adjust engine evaluation behavior.
    /// </summary>
    [ProtoContract]
    public class EngineOption
    {
        /// <summary>
        /// Gets or sets the name of the option.
        /// </summary>
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of option this is.
        /// </summary>
        [ProtoMember(2)]
        public OptionType Type { get; set; }

        /// <summary>
        /// Gets or sets the upper bound of the options.
        /// </summary>
        [ProtoMember(3)]
        public int? Minimum { get; set; }

        /// <summary>
        /// Gets or sets the lower bound of the option.
        /// </summary>
        [ProtoMember(4)]
        public int? Maximum { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        [ProtoMember(5)]
        public OptionValue DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the list of valid values.
        /// </summary>
        [ProtoMember(6)]
        public List<string> ValidValues { get; set; }
    }
}