//-----------------------------------------------------------------------
// <copyright file="HealthCheckController.cs" company="ChessDB.AI">
// MIT Licensed.
// </copyright>
//-----------------------------------------------------------------------

namespace Maki.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// A simple health check controller.
    /// </summary>
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        /// <summary>
        /// Simple health check API endpoint.
        /// </summary>
        /// <returns>"svc-up".</returns>
        [HttpGet]
        [Produces("text/plain")]
        [Route("health")]
        public ActionResult CheckHealthAsync()
        {
            return this.Ok("svc-up");
        }
    }
}