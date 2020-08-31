using AdfPipelineTrigger.Models;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdfPipelineTrigger
{
    public class Runner
    {
        public async Task<string> CreateRunPipeline(RunRequest inputRequest)
        {
            // Authenticate and create a data factory management client
            DataFactoryManagementClient client = await GetDataFactoryClient(inputRequest.SubscriptionId);

            AzureOperationResponse<CreateRunResponse> runResponse = await client.Pipelines.
                CreateRunWithHttpMessagesAsync(
                inputRequest.ResourceGroup,
                inputRequest.DataFactoryName,
                inputRequest.PipelineName,
                parameters: inputRequest.PipelineParams
            );

            return runResponse.Body.RunId;
        }

        private async Task<DataFactoryManagementClient> GetDataFactoryClient(string subscriptionId)
        {
            AuthenticationContext context = new AuthenticationContext(Environment.GetEnvironmentVariable("Azure_ContextUrl") + 
                Environment.GetEnvironmentVariable("Azure_TenantId"));
            ClientCredential cc = new ClientCredential(Environment.GetEnvironmentVariable("ADF-ApplicationId"), Environment.GetEnvironmentVariable("ADF-ApplicationSecret"));
            AuthenticationResult result = await context.AcquireTokenAsync(Environment.GetEnvironmentVariable("Azure_ManagementUrl"), cc);
            ServiceClientCredentials cred = new TokenCredentials(result.AccessToken);
            DataFactoryManagementClient client = new DataFactoryManagementClient(cred)
            {
                SubscriptionId = subscriptionId
            };
            return client;
        }

        public async Task CreatePipelineTrigger(PipelineTriggerRequest request)
        {
            // Authenticate and create a data factory management client
            DataFactoryManagementClient client = await GetDataFactoryClient(request.SubscriptionId);

            ScheduleTrigger scheduleTrigger = GetScheduledTrigger(request);

            if (scheduleTrigger != null)
            {
                // Now, create the trigger by invoking the CreateOrUpdate method
                TriggerResource triggerResource = new TriggerResource()
                {
                    Properties = scheduleTrigger
                };
                await client.Triggers.CreateOrUpdateAsync(request.ResourceGroup, request.DataFactoryName, request.TriggerName, triggerResource);

                // Activate the trigger
                client.Triggers.Start(request.ResourceGroup, request.DataFactoryName, request.TriggerName);
            }
            else
            {
                throw new Exception("Unable to create scheduled trigger");
            }
        }

        public async Task DeletePipelineTrigger(DeleteRequest inputRequest)
        {
            DataFactoryManagementClient client = await GetDataFactoryClient(inputRequest.SubscriptionId);

            await client.Triggers.StopAsync(inputRequest.ResourceGroup, inputRequest.DataFactoryName, inputRequest.TriggerName);

            await client.Triggers.DeleteAsync(inputRequest.ResourceGroup, inputRequest.DataFactoryName, inputRequest.TriggerName);
        }

        private ScheduleTrigger GetScheduledTrigger(PipelineTriggerRequest request)
        {
            ScheduleTrigger trigger = new ScheduleTrigger()
            {
                Pipelines = GetPipelineForScheduleTrigger(request.Pipelines),
                Recurrence = GetScheduledTriggerRecurrence(request)
            };
            return trigger;
        }

        private ScheduleTriggerRecurrence GetScheduledTriggerRecurrence(PipelineTriggerRequest request)
        {
            ScheduleTriggerRecurrence recurrence = new ScheduleTriggerRecurrence()
            {
                TimeZone = "UTC"
            };
            recurrence.StartTime = request.StartDate.HasValue ? request.StartDate.Value.ToUniversalTime() : request.StartDate;
            recurrence.EndTime = request.EndDate.HasValue ? request.EndDate.Value.ToUniversalTime() : request.EndDate;
            if (request.Schedule != null)
            {
                recurrence.Schedule = new RecurrenceSchedule();
                if (request.Frequency.Equals(Constants.Schedule.Month, StringComparison.InvariantCultureIgnoreCase))
                {                    
                    if (request.Schedule.ScheduleRecurrences != null && request.Schedule.ScheduleRecurrences.Count > 0)
                    {
                        recurrence.Schedule.MonthlyOccurrences = GetMonthlyScheduleRecurrence(request.Schedule.ScheduleRecurrences);
                    }
                    else
                    {
                        recurrence.Schedule.MonthDays = request.Schedule.MonthDays;
                    }
                } else if (request.Frequency.Equals(Constants.Schedule.Week, StringComparison.InvariantCultureIgnoreCase))
                {
                    recurrence.Schedule.WeekDays = GetScheduleWeekDays(request.Schedule.WeekDays);
                }
                if (request.Schedule.Hours != null)
                {
                    recurrence.Schedule.Hours = request.Schedule.Hours;
                }
                if (request.Schedule.Minutes != null)
                {
                    recurrence.Schedule.Minutes = request.Schedule.Minutes;
                }
            }
            recurrence.Frequency = request.GetFrequency();
            recurrence.Interval = request.FrequencyInterval;
            return recurrence;
        }

        private List<DaysOfWeek?> GetScheduleWeekDays(List<int?> inputList)
        {
            List<DaysOfWeek?> weekdays = new List<DaysOfWeek?>();
            foreach(int? day in inputList)
            {
                if (day.HasValue)
                {
                    DaysOfWeek daysOfWeek = Enum.Parse<DaysOfWeek>(day.ToString());
                    weekdays.Add(daysOfWeek);
                }
            }
            return weekdays;
        }

        private List<RecurrenceScheduleOccurrence> GetMonthlyScheduleRecurrence(List<ScheduleRecurrence> occurrences)
        {
            List<RecurrenceScheduleOccurrence> scheduleOccurrences = new List<RecurrenceScheduleOccurrence>();
            foreach(ScheduleRecurrence occurrence in occurrences)
            {
                RecurrenceScheduleOccurrence scheduleOccurrence = new RecurrenceScheduleOccurrence();
                if (occurrence.Day.HasValue)
                {
                    scheduleOccurrence.Day = Enum.Parse<Microsoft.Azure.Management.DataFactory.Models.DayOfWeek>(occurrence.Day.ToString());
                    scheduleOccurrence.Occurrence = occurrence.Occurence;
                }
                scheduleOccurrences.Add(scheduleOccurrence);
            }
            return scheduleOccurrences;
        }

        private List<TriggerPipelineReference> GetPipelineForScheduleTrigger(List<Pipeline> pipelines)
        {
            List<TriggerPipelineReference> pipelineReferences = new List<TriggerPipelineReference>();
            foreach(Pipeline pipeline in pipelines)
            {
                TriggerPipelineReference reference = new TriggerPipelineReference()
                {
                    PipelineReference = new PipelineReference(pipeline.Name)
                };
                if (pipeline.PipelineParams != null && pipeline.PipelineParams.Count > 0)
                {
                    reference.Parameters = pipeline.PipelineParams;
                }
                pipelineReferences.Add(reference);
            }
            return pipelineReferences;
        }
    }
}
