//-----------------------------------------------------------------------
// <copyright file="ProtobufOutputFormatter.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Serialization
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.Net.Http.Headers;
    using ProtoBuf;

    /// <summary>
    /// An output formatter to serialize responses in Protocol Buffers format.
    /// </summary>
    public class ProtobufOutputFormatter : OutputFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufOutputFormatter"/> class.
        /// </summary>
        public ProtobufOutputFormatter()
        {
            this.SupportedMediaTypes.Clear();
            this.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/x-protobuf"));
        }

        /// <summary>
        /// Writes the actual response body for the output result.
        /// </summary>
        /// <param name="context">The output context.</param>
        /// <returns>An awaitable task.</returns>
        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var response = context.HttpContext.Response;

            Serializer.Serialize(response.Body, context.Object);
            return Task.FromResult(response);
        }
    }
}