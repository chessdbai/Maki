//-----------------------------------------------------------------------
// <copyright file="EvaluationApiController.cs" company="ChessDB.AI">
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
    using Newtonsoft.Json;

    /// <summary>
    /// API controller for controlling evaluation.
    /// </summary>
    [ApiController]
    public class EvaluationApiController : ControllerBase
    {
        private readonly EngineBridge engineBridge;
        private readonly ILogger<EvaluationApiController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluationApiController"/> class.
        /// </summary>
        /// <param name="engineBridge">The engine bridge.</param>
        /// <param name="logger">The logger.</param>
        public EvaluationApiController(
            EngineBridge engineBridge,
            ILogger<EvaluationApiController> logger)
        {
            this.engineBridge = engineBridge;
            this.logger = logger;
        }

        /// <summary>
        /// Starts the engine evaluation at the current FEN position.
        /// </summary>
        /// <param name="request">The start evaluation request.</param>
        /// <returns>The result.</returns>
        [HttpPost]
        [Route("evaluation")]
        [Consumes("application/json", "application/x-protobuf")]
        [Produces("application/json", "application/x-protobuf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<StartEvaluationResponse> StartEvaluation(StartEvaluationRequest request)
        {
            string requestBody = JsonConvert.SerializeObject(request);
            this.logger.LogDebug(requestBody);
            return this.engineBridge.StartEvaluation(request);
        }
    }
}