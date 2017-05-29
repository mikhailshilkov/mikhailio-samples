using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Azure.EventHubs.Processor;

public class SQLCheckpointManager : ICheckpointManager
{
    private readonly string connectionString;
    private readonly string topic;

    public TimeSpan LeaseRenewInterval => TimeSpan.FromDays(1);

    public TimeSpan LeaseDuration => TimeSpan.MaxValue;

    public SQLCheckpointManager(string connectionString, string topic)
    {
        this.connectionString = connectionString;
        this.topic = topic;
    }

    public async Task<bool> CheckpointStoreExistsAsync()
    {
        var connection = new SqlConnection(this.connectionString);
        var table = await connection.QueryFirstOrDefaultAsync(
            new CommandDefinition("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'EventHubCheckpoint'"));
        return table != null;
    }

    public async Task<bool> CreateCheckpointStoreIfNotExistsAsync()
    {
        var connection = new SqlConnection(this.connectionString);
        var result = await connection.ExecuteScalarAsync<int>(
            @"IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'EventHubCheckpoint')
                BEGIN
                CREATE TABLE EventHubCheckpoint (
                    Topic varchar(100) NOT NULL,
                    PartitionID varchar(100) NOT NULL,
                    SequenceNumber bigint NOT NULL,
                    Offset varchar(20) NOT NULL,
                    CONSTRAINT PK_EventHubCheckpoint PRIMARY KEY CLUSTERED (Topic, PartitionID)
                )
                SELECT 1
                END
                ELSE SELECT 0");
        return result > 0;
    }

    public async Task<Checkpoint> CreateCheckpointIfNotExistsAsync(string partitionId)
    {
        var connection = new SqlConnection(this.connectionString);
        var checkpoint = await connection.QueryFirstOrDefaultAsync(
            new CommandDefinition(
                "SELECT SequenceNumber, Offset FROM EventHubCheckpoint WHERE PartitionID = @PartitionID AND Topic = @Topic",
                new { PartitionID = partitionId, Topic = this.topic }))
            .ConfigureAwait(false);

        if (checkpoint == null)
        {
            var result = new Checkpoint(partitionId);

            await connection.ExecuteAsync(
                "INSERT EventHubCheckpoint (Topic, PartitionID, SequenceNumber, Offset) VALUES (@Topic, @PartitionID, @SequenceNumber, @Offset)",
                new { Topic = this.topic, PartitionID = partitionId, SequenceNumber = result.SequenceNumber, Offset = result.Offset });

            return result;
        }

        return new Checkpoint(partitionId)
        {
            Offset = checkpoint.Offset,
            SequenceNumber = checkpoint.SequenceNumber
        };
    }

    public Task DeleteCheckpointAsync(string partitionId)
    {
        // Make this a no-op to avoid deleting by accident.
        return Task.FromResult(0);
    }

    public async Task<Checkpoint> GetCheckpointAsync(string partitionId)
    {
        var connection = new SqlConnection(this.connectionString);
        var checkpoint = await connection.QueryFirstOrDefaultAsync(
            new CommandDefinition(
                "SELECT SequenceNumber, Offset FROM EventHubCheckpoint WHERE PartitionID = @PartitionID AND Topic = @Topic",
                new { PartitionID = partitionId, Topic = this.topic }))
            .ConfigureAwait(false);
        return checkpoint != null
            ? new Checkpoint(partitionId)
            {
                Offset = checkpoint.Offset,
                SequenceNumber = checkpoint.SequenceNumber
            }
            : null;
    }

    [Obsolete("Use UpdateCheckpointAsync(Lease lease, Checkpoint checkpoint) instead", true)]
    public Task UpdateCheckpointAsync(Checkpoint checkpoint)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateCheckpointAsync(Lease lease, Checkpoint checkpoint)
    {
        var connection = new SqlConnection(this.connectionString);
        await connection.ExecuteAsync(
                "UPDATE EventHubCheckpoint SET SequenceNumber = @SequenceNumber, Offset = @Offset WHERE PartitionID = @PartitionID AND Topic = @Topic",
                new { PartitionID = lease.PartitionId, SequenceNumber = checkpoint.SequenceNumber, Offset = checkpoint.Offset, Topic = this.topic });
    }        
}
