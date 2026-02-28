using System;

namespace FDHiring.Core.Models
{
    public class Interview
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int PositionId { get; set; }
        public int InterviewTypeId { get; set; }
        public string Owner { get; set; } = string.Empty;
        public DateTime? ScheduledDate { get; set; }
        public bool CandidateGo { get; set; }
        public string? Notes { get; set; }

        // Joined fields
        public string? CandidateName { get; set; }
        public string? PositionName { get; set; }
        public string? InterviewTypeName { get; set; }
    }
}
