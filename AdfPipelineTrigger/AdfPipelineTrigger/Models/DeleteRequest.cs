using System;

namespace AdfPipelineTrigger.Models
{
    public class DeleteRequest : PipelineRequestBase
    {
        public string TriggerName { get; set; }
        public override bool IsValid => true;
    }
}
