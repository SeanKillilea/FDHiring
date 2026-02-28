using System;

namespace FDHiring.Core.Models
{
    public class WorkflowStep
    {
        public int Id { get; set; }
        public int PositionId { get; set; }
        public string StepName { get; set; } = string.Empty;
        public int StepOrder { get; set; }

        // Joined field
        public string? PositionName { get; set; }
    }
}