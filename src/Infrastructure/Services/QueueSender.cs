using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Newtonsoft.Json;

namespace Microsoft.eShopWeb.Infrastructure.Services
{
    public class QueueSender : IQueueSender
    {
        private readonly string _connectionString;

        public QueueSender(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task SendPendingOrderMessage(int orderId)
        {
            return SendMessageAsync("pendingorders", new { orderId });
        }

        private Task SendMessageAsync(string queueName, object messageBody)
        {
            CloudQueueClient client = CloudStorageAccount.Parse(_connectionString).CreateCloudQueueClient();
            CloudQueue queue = client.GetQueueReference(queueName);

            var json = JsonConvert.SerializeObject(messageBody);
            var encodedJson = Encoding.UTF8.GetBytes(json);
            var message = new CloudQueueMessage(encodedJson);
            return queue.AddMessageAsync(message);
        }
    }
}
