using Microsoft.Azure.Management.DataFactory.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdfPipelineTrigger.Models
{
    public class PipelineTriggerRequest : PipelineRequestBase
    {
        public string TriggerName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Frequency { get; set; }

        public int? FrequencyInterval { get; set; }

        public Schedule Schedule { get; set; }

        public List<Pipeline> Pipelines { get; set; }

        internal string GetFrequency()
        {
            switch(Frequency.ToLower())
            {
                case Constants.Schedule.Minute:
                    return RecurrenceFrequency.Minute;
                case Constants.Schedule.Hour:
                    return RecurrenceFrequency.Hour;
                case Constants.Schedule.Day:
                    return RecurrenceFrequency.Day;
                case Constants.Schedule.Week:
                    return RecurrenceFrequency.Week;
                case Constants.Schedule.Month:
                    return RecurrenceFrequency.Month;
                case Constants.Schedule.Year:
                    return RecurrenceFrequency.Year;
                default:
                    return string.Empty;
            }
        }

        public override bool IsValid
        {
            get
            {
                if (!string.IsNullOrEmpty(SubscriptionId) && !string.IsNullOrEmpty(DataFactoryName) &&
                    !string.IsNullOrEmpty(ResourceGroup) && !string.IsNullOrEmpty(TriggerName) &&
                    !string.IsNullOrEmpty(Frequency) && FrequencyInterval.HasValue && FrequencyInterval.Value > 0)
                {
                    string recurrenceFrequency = GetFrequency();
                    if (string.IsNullOrEmpty(recurrenceFrequency))
                    {
                        return false;
                    }
                    if (Pipelines != null && Pipelines.Count > 0 && !Pipelines.Any(x => string.IsNullOrEmpty(x.Name)))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
