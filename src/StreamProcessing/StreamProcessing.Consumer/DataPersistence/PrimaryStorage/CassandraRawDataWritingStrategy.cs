using Cassandra;
using Cassandra.Data.Linq;
using StreamProcessing.Contracts;

namespace StreamProcessing.Consumer.DataPersistence.PrimaryStorage;

public class CassandraRawDataWritingStrategy : IDataWritingStrategy
{
    private readonly string _keyspace;
    private readonly ICluster _cluster;

    private ISession? _session; 

    public CassandraRawDataWritingStrategy(string keyspace, ICluster cluster)
    {
        _keyspace = keyspace;
        _cluster = cluster;
    }
    
    public async Task Write(ScadaDataPoint dataPoint)
    {
        if (!EntitiesMapping.EntitiesMapped())
            EntitiesMapping.MapEntities();

        _session ??= await _cluster.ConnectAsync(_keyspace);

        var table = _session.GetTable<ScadaDataPointEntity>();

        await table.Insert(dataPoint.ToEntity()).ExecuteAsync();
    }

    public Task Flush() =>
        Task.CompletedTask;

    public void Dispose()
    {
        _cluster.Dispose();
        _session?.Dispose();
    }
}