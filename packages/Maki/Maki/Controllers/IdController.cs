//-----------------------------------------------------------------------
// <copyright file="IdController.cs" company="ChessDB.AI">
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
    /// A controller for an API to get the engine id information.
    /// </summary>
    public class IdController : ControllerBase
    {
        private readonly EngineBridge engineBridge;
        private readonly ILogger<IdController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdController"/> class.
        /// </summary>
        /// <param name="engineBridge">The engine bridge.</param>
        /// <param name="logger">The logger.</param>
        public IdController(
            EngineBridge engineBridge,
            ILogger<IdController> logger)
        {
            this.engineBridge = engineBridge;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the engine ID information.
        /// </summary>
        /// <returns>The list of options.</returns>
        [HttpGet]
        [Route("id")]
        [Consumes("application/json", "application/x-protobuf")]
        [Produces("application/json", "application/x-protobuf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<GetEngineIdResponse> GetOptions()
        {
            return new GetEngineIdResponse()
            {
                Name = this.engineBridge.Name,
                Author = this.engineBridge.Author,
            };
        }
    }
}