//-----------------------------------------------------------------------
// <copyright file="ProtobufInputFormatter.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Serialization
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.Net.Http.Headers;
    using ProtoBuf;

    /// <summary>
    /// A request formatter to allow clients to send requests serialized
    /// in protocol buffers format.
    /// </summary>
    public class ProtobufInputFormatter : InputFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtobufInputFormatter"/> class.
        /// </summary>
        public ProtobufInputFormatter()
        {
            this.SupportedMediaTypes.Clear();
            this.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/x-protobuf"));
        }

        /// <inheritdoc cref="InputFormatter"/>
        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var type = context.ModelType;
            var request = context.HttpContext.Request;
            MediaTypeHeaderValue.TryParse(request.ContentType, out _);

            object result = Serializer.Deserialize(type, context.HttpContext.Request.Body);
            return InputFormatterResult.SuccessAsync(result);
        }
    }
}