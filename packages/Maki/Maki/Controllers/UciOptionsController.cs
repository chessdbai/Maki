//-----------------------------------------------------------------------
// <copyright file="UciOptionsController.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Controllers
{
    using Maki.Engine;
    using Maki.Model;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A controller to allow clients to enumerate the supported UCI options.
    /// </summary>
    [ApiController]
    public class UciOptionsController : ControllerBase
    {
        private readonly EngineBridge engineBridge;
        private readonly ILogger<UciOptionsController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UciOptionsController"/> class.
        /// </summary>
        /// <param name="engineBridge">The engine bridge.</param>
        /// <param name="logger">The logger.</param>
        public UciOptionsController(
            EngineBridge engineBridge,
            ILogger<UciOptionsController> logger)
        {
            this.engineBridge = engineBridge;
            this.logger = logger;
        }

        /// <summary>
        /// Lists the options supported by the engine.
        /// </summary>
        /// <returns>The list of options.</returns>
        [HttpGet]
        [Route("options")]
        [Consumes("application/json", "application/x-protobuf")]
        [Produces("application/json", "application/x-protobuf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<ListSupportedOptionsResponse> GetOptions()
        {
            return new ListSupportedOptionsResponse()
            {
                Options = this.engineBridge.SupportedOptions,
            };
        }
    }
}