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
    public static class ManualTrigger
    {
        [FunctionName("ManualTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Trigger/Manual")] ManualTriggerRequest manualRequest,
            ILogger log)
        {
            if (manualRequest != null && manualRequest.IsValid)
            {
                try
                {
                    TriggerManager runner = new TriggerManager();
                    string runId = await runner.CreateRunPipeline(manualRequest);
                    if (!string.IsNullOrEmpty(runId))
                    {
                        return new OkObjectResult(runId);
                    }
                }
                catch (Exception ex)
                {
                    return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                }
            }
            return new StatusCodeResult(StatusCodes.Status400BadRequest);
        }
    }
}
