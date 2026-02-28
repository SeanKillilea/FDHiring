namespace FDHiring.Core.Models
{
    public class InterviewAnswer
    {
        public int Id { get; set; }
        public int InterviewId { get; set; }
        public int InterviewQuestionId { get; set; }
        public string? Answer { get; set; }
        public string AnsweredByUser { get; set; } = string.Empty;

        // Joined fields
        public string? Question { get; set; }
        public int SortOrder { get; set; }
    }
}