namespace FDHiring.Core.Models
{
    public class InterviewQuestion
    {
        public int Id { get; set; }
        public int PositionId { get; set; }
        public int InterviewTypeId { get; set; }
        public string Question { get; set; } = string.Empty;
        public int SortOrder { get; set; }

        // Joined fields
        public string? PositionName { get; set; }
        public string? InterviewTypeName { get; set; }
    }
}