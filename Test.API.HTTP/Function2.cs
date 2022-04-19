using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Test.API.HTTP
{
    public static class Function2
    {
        [FunctionName("EVHOrderUpdate")]
        public static async Task EVHOrderUpdate(
            [EventHubTrigger("f39-evh-tutorial-orders", Connection = "eventHubNamespace")] EventData[] events,
            ILogger log)
        {
            var exceptions = new List<Exception>();

            foreach (EventData eventData in events)
            {
                try
                {
                    dynamic messages = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(eventData.Body.Array));
                    foreach (dynamic message in messages)
                    {
                        string content = message.data;
                        dynamic data = JsonConvert.DeserializeObject(content);
                        foreach (dynamic item in data.Items)
                        {

                            var httpClient = HttpClientFactory.Create();
                            var url = "https://f39-function-tutorial.azurewebsites.net/api/item/stock?code=fB9bTKM7QSy3/7q51HwX07DzlOi/GIDkFFak0a7vwQl1gaYvLllQDw==";
                            HttpContent httpContent = new StringContent(
                            JsonConvert.SerializeObject(item),
                            Encoding.UTF8,
                            "application/json");
                            dynamic result = await httpClient.PutAsync(url, httpContent);
                            Console.WriteLine(result);
                            await Task.Yield();
                        }
                    }
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }
    }
}
