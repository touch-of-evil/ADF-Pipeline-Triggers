using System.Collections.Generic;

namespace AdfPipelineTriggers.Models
{
    public class Pipeline
    {
        public string Name { get; set; }
        public Dictionary<string, object> PipelineParams { get; set; }
    }
}
