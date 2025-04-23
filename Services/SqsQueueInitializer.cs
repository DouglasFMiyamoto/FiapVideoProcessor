using Amazon.SQS.Model;
using Amazon.SQS;

namespace FiapVideoProcessor.Services
{
    public class SqsQueueInitializer
    {
        private readonly IAmazonSQS _sqs;
        private readonly string _queueName = "video-queue";

        public string QueueUrl { get; private set; } = string.Empty;

        public SqsQueueInitializer(IAmazonSQS sqs)
        {
            _sqs = sqs;
        }

        public async Task InitializeAsync()
        {
            var listResponse = await _sqs.ListQueuesAsync(new ListQueuesRequest());

            if (!listResponse.QueueUrls.Any(q => q.EndsWith(_queueName)))
            {
                var createResponse = await _sqs.CreateQueueAsync(_queueName);
                QueueUrl = createResponse.QueueUrl;
            }
            else
            {
                QueueUrl = listResponse.QueueUrls.First(q => q.EndsWith(_queueName));
            }
        }
    }
}
