using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDHiring.Core.Models
{
    public class Workflow
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int PositionId { get; set; }
        public int CurrentStepId { get; set; }
        public DateTime StartDate { get; set; }
        public bool Active { get; set; }

        // Joined fields
        public string? CurrentStepName { get; set; }
        public int? CurrentStepOrder { get; set; }
    }
}
