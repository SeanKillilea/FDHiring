using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDHiring.Core.Models
{
    public class Interview
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int WorkflowId { get; set; }
        public int InterviewNumber { get; set; }
        public int InterviewedByUserId { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status { get; set; } = "Scheduled";
        public string? Notes { get; set; }

        // Joined fields
        public string? InterviewedByName { get; set; }
    }
}
