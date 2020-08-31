using System.Collections.Generic;

namespace AdfPipelineTrigger.Models
{
    public class Pipeline
    {
        public string Name { get; set; }
        public Dictionary<string, object> PipelineParams { get; set; }
    }
}
