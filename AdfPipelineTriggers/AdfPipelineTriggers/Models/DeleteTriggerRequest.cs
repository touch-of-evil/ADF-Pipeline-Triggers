
namespace AdfPipelineTriggers.Models
{
    public class DeleteTriggerRequest : PipelineRequestBase
    {
        public string TriggerName { get; set; }
        public override bool IsValid => !string.IsNullOrEmpty(TriggerName) && !string.IsNullOrEmpty(ResourceGroup) &&
                !string.IsNullOrEmpty(DataFactoryName);
    }
}
