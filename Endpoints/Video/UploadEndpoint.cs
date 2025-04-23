using FastEndpoints;
using FiapVideoProcessor.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FiapVideoProcessor.Endpoints.Video
{
    public class UploadVideoRequest
    {
        public IFormFile File { get; set; } = default!;
    }

    public class UploadVideoResponse
    {
        public int VideoId { get; set; }
        public string Status { get; set; } = default!;
    }

    [Authorize]
    public class UploadEndpoint : Endpoint<UploadVideoRequest, UploadVideoResponse>
    {
        private readonly IVideoService _videoService;
        private readonly IQueueService _queueService;
        private readonly SqsQueueInitializer _queueInitializer;

        public UploadEndpoint(IVideoService videoService, IQueueService queueService, SqsQueueInitializer queueInitializer)
        {
            _videoService = videoService;
            _queueService = queueService;
            _queueInitializer = queueInitializer;
        }

        public override void Configure()
        {
            Post("/videos/upload");
            AllowFileUploads();
            Summary(s =>
            {
                s.Summary = "Faz upload de vídeo, salva no banco e envia para a fila SQS";
            });
        }

        public override async Task HandleAsync(UploadVideoRequest req, CancellationToken ct)
        {
            if (req.File is null || req.File.Length == 0)
            {
                AddError(r => r.File, "Arquivo inválido.");
                await SendErrorsAsync();
                return;
            }

            var userId = int.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var fileName = $"{Guid.NewGuid()}_{req.File.FileName}";

            var video = new Domain.Entities.Video
            {
                FileName = fileName,
                UserId = userId
            };

            var saved = await _videoService.UploadVideoAsync(video);

            // Envia para a fila
            await _queueService.SendMessageAsync(_queueInitializer.QueueUrl, new
            {
                VideoId = saved.Id,
                FileName = saved.FileName,
                UserId = saved.UserId
            });

            await SendAsync(new UploadVideoResponse
            {
                VideoId = saved.Id,
                Status = saved.Status
            });
        }
    }
}
