using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AdfPipelineTrigger.Models;
using System;

namespace AdfPipelineTrigger
{
    public static class PipelineManager
    {
        [FunctionName("RunPipeline")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Pipeline/RunPipeline")] RunRequest req,
            ILogger log)
        {
            if (req != null && req.IsValid)
            {
                try
                {
                    Runner runner = new Runner();
                    string runId = await runner.CreateRunPipeline(req);
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

        [FunctionName("CreateTrigger")]
        public static async Task<IActionResult> CreateTrigger(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Pipeline/Trigger")] PipelineTriggerRequest req,
            ILogger log)
        {
            if (req != null && req.IsValid)
            {
                try
                {
                    Runner runner = new Runner();
                    await runner.CreatePipelineTrigger(req);
                    return new NoContentResult();
                }
                catch (Exception ex)
                {
                    return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                }
            }
            return new BadRequestResult();
        }

        [FunctionName("DeleteTrigger")]
        public static async Task<IActionResult> DeleteTrigger(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Pipeline/Trigger")] DeleteRequest req,
            ILogger log)
        {
            if (req != null && req.IsValid)
            {
                try
                {
                    Runner runner = new Runner();
                    await runner.DeletePipelineTrigger(req);
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
