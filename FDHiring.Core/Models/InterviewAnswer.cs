using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDHiring.Core.Models
{
    public class InterviewAnswer
    {
        public int Id { get; set; }
        public int InterviewId { get; set; }
        public int InterviewQuestionId { get; set; }
        public string? Answer { get; set; }
        public int AnsweredByUserId { get; set; }

        // Joined fields
        public string? Question { get; set; }
    }
}
