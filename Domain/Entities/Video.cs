namespace FiapVideoProcessor.Domain.Entities
{
    public class Video
    {
        public int Id { get; set; }

        public string FileName { get; set; } = default!;

        public string Status { get; set; } = "Pending"; // Pending, Processing, Completed

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public string? ZipPath { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }
    }
}
