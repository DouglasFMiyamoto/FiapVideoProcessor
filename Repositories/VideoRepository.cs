using FiapVideoProcessor.Context;
using FiapVideoProcessor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiapVideoProcessor.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly AppDbContext _context;

        public VideoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Video> AddAsync(Video video)
        {
            _context.Videos.Add(video);
            await _context.SaveChangesAsync();
            return video;
        }

        public async Task<List<Video>> GetByUserIdAsync(int userId)
        {
            return await _context.Videos
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.UploadedAt)
                .ToListAsync();
        }

        public async Task UpdateStatusAsync(int videoId, string status)
        {
            var video = await _context.Videos.FindAsync(videoId);
            if (video is not null)
            {
                video.Status = status;
                await _context.SaveChangesAsync();
            }
        }
    }
}
