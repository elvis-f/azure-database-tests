using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using TableEntity = Microsoft.WindowsAzure.Storage.Table.TableEntity;

namespace Atea.Homework;

public static class GetLogsInRange
{
    [FunctionName("GetLogsInRange")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequest req, ILogger log)
    {
        log.LogInformation("GET API to list all logs for specific time period (from/to) [DD-MM-YYYY]");

        var from = HelperMethods.GetParameter(req, "from");
        var to = HelperMethods.GetParameter(req, "to");

        if (from == null || to == null)
        {
            return new BadRequestObjectResult("Missing from/to dates!");
        }

        var dtoFrom = HelperMethods.ParseDto(from) ?? new DateTimeOffset();
        var dtoTo = HelperMethods.ParseDto(to) ?? new DateTimeOffset();

        if (dtoFrom == new DateTimeOffset() || dtoTo == new DateTimeOffset())
        {
            return new BadRequestObjectResult("Bad date! Please use mm-DD-yyyy format in your GET request!!");
        }

        var tableConnectionString = "UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://127.0.0.1";
        var tableName = "logs";
        
        var tableClient = new TableClient(tableConnectionString, tableName);
        await tableClient.CreateIfNotExistsAsync();

        var filter = TableQuery.CombineFilters(
            TableQuery.GenerateFilterConditionForDate(
                "CreatedAt",
                QueryComparisons.GreaterThanOrEqual,
                dtoFrom
            ),
            TableOperators.And,
            TableQuery.GenerateFilterConditionForDate(
                "CreatedAt",
                QueryComparisons.LessThanOrEqual,
                dtoTo
            )
        );

        var query = Query(tableClient, filter);
        var parsedQuery = query.Select(x => $"{x.CreatedAt:d}|{x.IsSuccess}|{x.Endpoint}|{x.BlobName}");

        return new OkObjectResult(string.Join("\n", parsedQuery));
    }
    
    private static List<Log> Query(TableClient tableClient, string filter)
    {
        var query = new List<Log>();
        
        try
        {
            // Get all the entities from the table
            var entities = tableClient.Query<Log>(filter);

            // Iterate over the results and print each entity
            foreach (var entity in entities)
            {
                query.Add(entity);
            }
        }
        catch (RequestFailedException e)
        {
            Console.WriteLine($"Request failed: {e}");
        }

        return query;
    }
}