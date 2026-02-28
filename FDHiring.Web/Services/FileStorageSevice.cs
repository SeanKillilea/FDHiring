namespace FDHiring.Web.Services
{
    public class FileStorageService
    {
        private readonly string _basePath;

        public FileStorageService(IWebHostEnvironment env, IConfiguration config)
        {
            var relative = config["FileStorage:BasePath"] ?? "AppData/CandidateFiles";
            _basePath = Path.Combine(env.ContentRootPath, relative);
        }

        public async Task<string> SaveFileAsync(int candidateId, IFormFile file)
        {
            var folder = Path.Combine(_basePath, candidateId.ToString());
            Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(file.FileName);
            var guidName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(folder, guidName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return filePath;
        }

        public (byte[] bytes, string contentType) GetFile(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            var ext = Path.GetExtension(filePath).ToLower();
            var contentType = ext switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
            return (bytes, contentType);
        }

        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}