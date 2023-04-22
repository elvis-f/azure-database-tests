using System;
using Azure;
using Azure.Data.Tables;

namespace Atea.Homework;

public class Log : ITableEntity
{
    public bool IsSuccess { get; set; }
    public string Endpoint { get; set; }
    public string BlobName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public Log(bool status, string endpoint, string blobName)
    {
        IsSuccess = status;
        Endpoint = endpoint;
        BlobName = blobName;
        CreatedAt = DateTimeOffset.Now;

        PartitionKey = "partition";
        RowKey = blobName;
    }

    public Log()
    {
    }
}