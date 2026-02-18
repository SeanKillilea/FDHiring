using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDHiring.Core.Models
{
    public class WorkflowHistory
    {
        public int Id { get; set; }
        public int WorkflowId { get; set; }
        public int StepId { get; set; }
        public DateTime CompletedDate { get; set; }
        public int CompletedByUserId { get; set; }
        public string? Notes { get; set; }

        // Joined fields
        public string? StepName { get; set; }
        public string? CompletedByName { get; set; }
    }
}
