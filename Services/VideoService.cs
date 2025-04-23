using FiapVideoProcessor.Domain.Entities;
using FiapVideoProcessor.Repositories;

namespace FiapVideoProcessor.Services
{
    public interface IVideoService
    {
        Task<Video> UploadVideoAsync(Video video);
        Task<List<Video>> GetVideosByUserAsync(int userId);
        Task UpdateStatusAsync(int videoId, string status);
    }

    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _repository;

        public VideoService(IVideoRepository repository)
        {
            _repository = repository;
        }

        public async Task<Video> UploadVideoAsync(Video video)
        {
            video.Status = "Pending";
            return await _repository.AddAsync(video);
        }

        public async Task<List<Video>> GetVideosByUserAsync(int userId)
        {
            return await _repository.GetByUserIdAsync(userId);
        }

        public async Task UpdateStatusAsync(int videoId, string status)
        {
            await _repository.UpdateStatusAsync(videoId, status);
        }
    }
}
