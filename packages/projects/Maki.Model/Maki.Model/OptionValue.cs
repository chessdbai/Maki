//-----------------------------------------------------------------------
// <copyright file="OptionValue.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Model
{
    using System.ComponentModel.DataAnnotations;
    using ProtoBuf;

    /// <summary>
    /// An engine option name and value to set.
    /// </summary>
    [ProtoContract]
    public class OptionValue
    {
        /// <summary>
        /// Gets or sets the text value of the option.
        /// </summary>
        [ProtoMember(1)]
        public string TextValue { get; set; }

        /// <summary>
        /// Gets or sets the boolean value of the option.
        /// </summary>
        [ProtoMember(2)]
        public bool? BooleanValue { get; set; }

        /// <summary>
        /// Gets or sets the integer value of the option.
        /// </summary>
        [ProtoMember(3)]
        public int? IntegerValue { get; set; }
    }
}