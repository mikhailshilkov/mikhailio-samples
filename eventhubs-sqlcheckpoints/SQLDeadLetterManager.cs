using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Azure.EventHubs;

public class DeadLetter<T>
{
    public DeadLetter(T data, DateTime failureTime, Exception exception)
    {
        this.Data = data;
        this.FailureTime = failureTime;
        this.Exception = exception;
    }

    public T Data { get; }

    public DateTime FailureTime { get; }

    public Exception Exception { get; }
}

public interface IDeadLetterManager
{
    Task AddFailedEvents(IEnumerable<DeadLetter<EventData>> deadLetters);

    Task<bool> CreateDeadLetterStoreIfNotExistsAsync();
}

public class SQLDeadLetterManager : IDeadLetterManager
{
    private readonly string connectionString;
    private readonly string topic;

    public SQLDeadLetterManager(string connectionString, string topic)
    {
        this.connectionString = connectionString;
        this.topic = topic;
    }

    public Task AddFailedEvents(IEnumerable<DeadLetter<EventData>> deadLetters)
    {
        var connection = new SqlConnection(this.connectionString);
        return connection.ExecuteAsync(
            @"insert EventHubDeadLetter (Topic,PartitionID,SequenceNumber,Offset) 
            values(@Topic,@PartitionID,@SequenceNumber,@Offset)",
            deadLetters
                .Select(dl => new
                {
                    Topic = this.topic,
                    PartitionID = dl.Data.SystemProperties.PartitionKey,
                    dl.Data.SystemProperties.SequenceNumber,
                    dl.Data.SystemProperties.Offset,
                    dl.FailureTime,
                    Error = dl.Exception.ToString()
                }));
    }

    public async Task<bool> CreateDeadLetterStoreIfNotExistsAsync()
    {
        var connection = new SqlConnection(this.connectionString);
        var result = await connection.ExecuteScalarAsync<int>(
            @"IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'EventHubDeadLetter')
                BEGIN
                CREATE TABLE EventHubDeadLetter (
                    Topic varchar(100) NOT NULL,
                    PartitionID varchar(100) NOT NULL,
                    SequenceNumber bigint NOT NULL,
                    Offset varchar(20) NOT NULL,
                    FailedAt datetime NOT NULL,
                    Error nvarchar(max) NOT NULL,
                    CONSTRAINT PK_EventHubDeadLetter PRIMARY KEY CLUSTERED (Topic, PartitionID)
                )
                SELECT 1
                END
                ELSE SELECT 0");
        return result > 0;
    }
}
