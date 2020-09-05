using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AdfPipelineTriggers.Models;

namespace AdfPipelineTriggers
{
    public static class ScheduledTrigger
    {
        [FunctionName("ScheduledTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Trigger/Schedule")] ScheduledTriggerRequest request,
            ILogger log)
        {
            if (request != null)
            {
                try
                {
                    TriggerManager manager = new TriggerManager();
                    await manager.ScheduleTrigger(request);
                    return new OkResult();
                }
                catch(Exception ex)
                {
                    return new JsonResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                }
            }
            return new BadRequestResult();
        }
    }
}
