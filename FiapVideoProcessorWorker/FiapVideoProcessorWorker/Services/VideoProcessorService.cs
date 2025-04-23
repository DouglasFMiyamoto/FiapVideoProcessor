using Amazon.SQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using FiapVideoProcessor.Context;
using Microsoft.EntityFrameworkCore;

namespace FiapVideoProcessorWorker.Services
{
    public class VideoProcessorService : BackgroundService
    {
        private readonly ILogger<VideoProcessorService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAmazonSQS _sqs;
        private readonly string _queueUrl = "http://localstack:4566/000000000000/video-queue";

        public VideoProcessorService(ILogger<VideoProcessorService> logger,
                                     IServiceProvider serviceProvider,
                                     IAmazonSQS sqs)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _sqs = sqs;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker de vídeo iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                var messages = await _sqs.ReceiveMessageAsync(new Amazon.SQS.Model.ReceiveMessageRequest
                {
                    QueueUrl = _queueUrl,
                    MaxNumberOfMessages = 5,
                    WaitTimeSeconds = 5
                }, stoppingToken);

                foreach (var msg in messages.Messages)
                {
                    try
                    {
                        var payload = JsonSerializer.Deserialize<VideoMessage>(msg.Body);
                        if (payload is null) continue;

                        _logger.LogInformation("Processando vídeo {FileName} (Id: {Id})", payload.FileName, payload.VideoId);

                        using var scope = _serviceProvider.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var video = await db.Videos.FirstOrDefaultAsync(v => v.Id == payload.VideoId, stoppingToken);
                        if (video is null) continue;

                        // Simula o "processamento"
                        await Task.Delay(2000, stoppingToken);
                        video.Status = "Processed";

                        await db.SaveChangesAsync(stoppingToken);
                        await _sqs.DeleteMessageAsync(_queueUrl, msg.ReceiptHandle, stoppingToken);

                        _logger.LogInformation("Vídeo {Id} processado com sucesso!", video.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar mensagem da fila");
                    }
                }
            }
        }

        private record VideoMessage
        {
            public int VideoId { get; set; }
            public string FileName { get; set; } = default!;
            public int UserId { get; set; }
        }
    }
}
