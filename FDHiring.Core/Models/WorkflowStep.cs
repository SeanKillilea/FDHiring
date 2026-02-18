using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDHiring.Core.Models
{
    public class WorkflowStep
    {
        public int Id { get; set; }
        public int StepOrder { get; set; }
        public string StepName { get; set; } = string.Empty;
    }
}
