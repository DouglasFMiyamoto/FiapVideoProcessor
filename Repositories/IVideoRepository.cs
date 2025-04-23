using FiapVideoProcessor.Domain.Entities;

namespace FiapVideoProcessor.Repositories
{
    public interface IVideoRepository
    {
        Task<Video> AddAsync(Video video);
        Task<List<Video>> GetByUserIdAsync(int userId);
        Task UpdateStatusAsync(int videoId, string status);
    }
}
