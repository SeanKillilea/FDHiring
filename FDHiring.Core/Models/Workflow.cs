using System;

namespace FDHiring.Core.Models
{
    public class Workflow
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int PositionId { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Owner { get; set; }
        public string? StepName { get; set; }
        public int StepOrder { get; set; }
        public bool Complete { get; set; }

        // Joined fields
        public string? CandidateName { get; set; }
        public string? PositionName { get; set; }
    }
}