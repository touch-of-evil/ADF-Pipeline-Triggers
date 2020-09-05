using System.Collections.Generic;

namespace AdfPipelineTriggers.Models
{
    public class ManualTriggerRequest : PipelineRequestBase
    {
        public string PipelineName { get; set; }
        public Dictionary<string, object> PipelineParams { get; set; }

        public override bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(ResourceGroup) &&
                !string.IsNullOrEmpty(DataFactoryName) && !string.IsNullOrEmpty(PipelineName);
            }
        }
    }
}
