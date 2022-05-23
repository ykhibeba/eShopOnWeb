using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace eShopWeb.Functions;

public static class OrderItemsReserver
{
    private static readonly HttpClient _httpClient = new();

    [FunctionName("OrderItemsReserver")]
    public static async Task Run(
        [ServiceBusTrigger("eshop-order", Connection = "ServiceBusConnectionString")] string myQueueItem,
        ILogger log)
    {
        var logicAppUrl = GetEnvironmentVariable("LogicAppEmailConnectionString");
        try
        {
            var blobContainerClient = new BlobContainerClient(GetEnvironmentVariable("BlobStorageConnectionString"), "eshop-container", new BlobClientOptions
            {
                Retry = { Mode = RetryMode.Fixed, Delay = TimeSpan.FromSeconds(5), MaxRetries = 3 }
            });

            var blobClient = blobContainerClient.GetBlobClient($"{Guid.NewGuid()}.json");

            await using MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(myQueueItem));
            await blobClient.UploadAsync(ms);
        }
        catch (Exception)
        {
            var response = await _httpClient.PostAsync(logicAppUrl, new StringContent(myQueueItem, Encoding.UTF8, "application/json"));
        }
    }

    public static string GetEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    }
}
