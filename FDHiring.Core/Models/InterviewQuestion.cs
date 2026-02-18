using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDHiring.Core.Models
{
    public class InterviewQuestion
    {
        public int Id { get; set; }
        public int PositionId { get; set; }
        public int InterviewNumber { get; set; }
        public string Question { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
