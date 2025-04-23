using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;

namespace FiapVideoProcessor.Services
{
    public interface IQueueService
    {
        Task SendMessageAsync<T>(string queueUrl, T message);
    }

    public class QueueService : IQueueService
    {
        private readonly IAmazonSQS _sqs;

        public QueueService(IAmazonSQS sqs)
        {
            _sqs = sqs;
        }

        public async Task SendMessageAsync<T>(string queueUrl, T message)
        {
            var msgBody = JsonSerializer.Serialize(message);

            var request = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = msgBody
            };

            await _sqs.SendMessageAsync(request);
        }
    }
}
