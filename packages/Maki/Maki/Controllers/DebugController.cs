//-----------------------------------------------------------------------
// <copyright file="DebugController.cs" company="ChessDB.AI">
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
    /// A controller for managing the debug flag state of the engine.
    /// </summary>
    public class DebugController : ControllerBase
    {
        private readonly EngineBridge engineBridge;
        private readonly ILogger<DebugController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugController"/> class.
        /// </summary>
        /// <param name="engineBridge">The engine bridge.</param>
        /// <param name="logger">The logger.</param>
        public DebugController(
            EngineBridge engineBridge,
            ILogger<DebugController> logger)
        {
            this.engineBridge = engineBridge;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the engine ID information.
        /// </summary>
        /// <param name="request">The <see cref="SetEngineDebugFlagRequest"/> request object.</param>
        /// <returns>The list of options.</returns>
        [HttpPost]
        [Route("debug")]
        [Consumes("application/json", "application/x-protobuf")]
        [Produces("application/json", "application/x-protobuf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult SetDebugModeFlag(SetEngineDebugFlagRequest request)
        {
            this.engineBridge.SetDebugMode(request.EnableDebugMode);
            return this.Ok();
        }
    }
}