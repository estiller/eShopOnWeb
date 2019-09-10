using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace OrderProcessor
{
    public static class Functions
    {
        [FunctionName("ProcessOrder")]
        public static async Task Run([QueueTrigger("pendingorders", Connection = "StorageAccount")]string queueItem, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Queue trigger function processed: {queueItem}");
            IConfigurationRoot config = LoadConfiguration(context);

            dynamic messageObj = JsonConvert.DeserializeObject(queueItem);
            int orderId = messageObj.orderId;

            log.LogInformation($"Doing some heavy work on order: {orderId}");
            await Task.Delay(1000);

            using (var dbContext = new CatalogContext(CreateDbOptions(config)))
            {
                var orderRepo = new OrderRepository(dbContext);
                var order = await orderRepo.GetByIdWithItemsAsync(orderId);
                order.Status = "Completed";
                await orderRepo.UpdateAsync(order);
            }
        }

        private static DbContextOptions<CatalogContext> CreateDbOptions(IConfigurationRoot config)
        {
            return new DbContextOptionsBuilder<CatalogContext>().UseSqlServer(config["CatalogConnection"]).Options;
        }

        private static IConfigurationRoot LoadConfiguration(ExecutionContext context)
        {
            return new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
