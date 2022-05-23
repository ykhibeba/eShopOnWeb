using System.IO;
using System.Threading.Tasks;
using eShopWeb.Functions.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace eShopWeb.Functions;

public static class OrderDeliveryProcessor
{
    [FunctionName("OrderDeliveryProcessorFunction")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        [CosmosDB(
            databaseName: "eShopWeb",
            collectionName: "delivery-order",
            ConnectionStringSetting = "CosmosDbConnectionString")]IAsyncCollector<dynamic> cosmoDb,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");


        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var order = JsonConvert.DeserializeObject<OrderDeliver>(requestBody);
        order.Total = order.CalculateTotal();
        await cosmoDb.AddAsync(order);

        return new OkObjectResult("Success save into CosmosDb.");
    }
}
