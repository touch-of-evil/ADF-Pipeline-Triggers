
namespace AdfPipelineTriggers.Models
{
    public abstract class PipelineRequestBase
    {
        public string ResourceGroup { get; set; }

        public string DataFactoryName { get; set; }

        public abstract bool IsValid { get; }
    }
}
