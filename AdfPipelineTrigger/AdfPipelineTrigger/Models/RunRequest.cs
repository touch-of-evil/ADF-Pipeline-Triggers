using System.Collections.Generic;

namespace AdfPipelineTrigger.Models
{
    public class RunRequest : PipelineRequestBase
    {
        public string PipelineName { get; set; }
        public Dictionary<string, object> PipelineParams { get; set; }

        public override bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(SubscriptionId) && !string.IsNullOrEmpty(ResourceGroup) &&
                !string.IsNullOrEmpty(DataFactoryName) && !string.IsNullOrEmpty(PipelineName);
            }
        }
    }
}
