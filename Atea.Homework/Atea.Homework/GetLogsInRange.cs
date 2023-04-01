using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

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

        DateTimeOffset dtoFrom;
        DateTimeOffset dtoTo;
        
        try
        {
            dtoFrom = DateTimeOffset.ParseExact(from, "dd-MM-yyyy", null);
            dtoTo = DateTimeOffset.ParseExact(to, "dd-MM-yyyy", null);
        }
        catch (FormatException)
        {
            return new BadRequestObjectResult("Bad date format! Please use \"DD-MM-YYYY\"");
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

        var query = tableClient.Query<Log>(filter);
        var parsedQuery = query.Select(l => $"{l.IsSuccess}|{l.Endpoint}|{l.BlobName}");

        return new OkObjectResult(string.Join("\n", parsedQuery));
    }
}