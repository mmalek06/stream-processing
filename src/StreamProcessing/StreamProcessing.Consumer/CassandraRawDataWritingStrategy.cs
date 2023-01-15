using Cassandra;
using StreamProcessing.Contracts;

namespace StreamProcessing.Consumer;

public class CassandraRawDataWritingStrategy : IDataWritingStrategy
{
    private readonly Cluster _cluster;

    public CassandraRawDataWritingStrategy(params string[] hosts)
    {
        _cluster = Cluster.Builder()
            .AddContactPoints(hosts)
            .Build();
    }
    
    public Task Write(ScadaDataPoint dataPoint)
    {
        throw new NotImplementedException();
    }
}