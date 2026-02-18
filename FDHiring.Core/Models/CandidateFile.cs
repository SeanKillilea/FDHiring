namespace FDHiring.Core.Models
{
    public class CandidateFile
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string? FileDescription { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public long FileSize { get; set; }
        public bool IsUserPicture { get; set; }
        public DateTime UploadDate { get; set; }
        public int UploadedByUserId { get; set; }

        // Joined field
        public string? UploadedByName { get; set; }
    }
}