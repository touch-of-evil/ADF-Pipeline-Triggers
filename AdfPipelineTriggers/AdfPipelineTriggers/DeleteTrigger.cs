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
    public static class DeleteTrigger
    {
        [FunctionName("DeleteTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Trigger")] DeleteTriggerRequest deleteRequest,
            ILogger log)
        {
            if (deleteRequest != null && deleteRequest.IsValid)
            {
                try
                {
                    TriggerManager runner = new TriggerManager();
                    await runner.DeletePipelineTrigger(deleteRequest);
                    return new OkResult();
                }
                catch (Exception ex)
                {
                    return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                }
            }
            return new BadRequestResult();
        }
    }
}
