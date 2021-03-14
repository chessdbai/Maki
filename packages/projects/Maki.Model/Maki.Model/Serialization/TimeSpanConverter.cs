//-----------------------------------------------------------------------
// <copyright file="TimeSpanConverter.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Model.Serialization
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// A custom Json converter for nullable TimeSpans.
    /// </summary>
    public class TimeSpanConverter : JsonConverter<TimeSpan?>
    {
        /// <summary>
        /// Writes a JSON value from a nullable time span.
        /// </summary>
        /// <param name="writer">The JSON writer.</param>
        /// <param name="value">The nullable TimeSpan value.</param>
        /// <param name="serializer">The serializer.</param>
        public override void WriteJson(JsonWriter writer, TimeSpan? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue((string)null);
            }
            else
            {
                writer.WriteValue(value!.Value.TotalMilliseconds);
            }
        }

        /// <summary>
        /// Attempts to read a nullable TimeSpan from a reader.
        /// </summary>
        /// <param name="reader">The JSON reader.</param>
        /// <param name="objectType">The type of object.</param>
        /// <param name="existingValue">The existing value, if any.</param>
        /// <param name="hasExistingValue">A value indicating whether or not there is an existing value.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns>The nullable timespan.</returns>
        /// <exception cref="ArgumentException">On an invalid JSON token type or value.</exception>
        public override TimeSpan? ReadJson(JsonReader reader, Type objectType, TimeSpan? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.Integer)
            {
                long totalMs = (long)reader.Value!;
                return TimeSpan.FromMilliseconds(totalMs);
            }

            if (reader.TokenType == JsonToken.Float)
            {
                double totalMs = (double)reader.Value!;
                return TimeSpan.FromMilliseconds(totalMs);
            }

            if (reader.TokenType == JsonToken.String)
            {
                if (!double.TryParse((string)reader.Value, out var doubleVal))
                {
                    throw new ArgumentException($"Cannot deserialize TimeSpan. Value '{reader.Value}' could not be parsed to a double.");
                }

                return TimeSpan.FromMilliseconds(doubleVal);
            }

            throw new ArgumentException($"Cannot deserialize timespan. Value '{reader.Value}' is neither a JSON null nor an integer.");
        }
    }
}