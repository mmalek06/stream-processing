using StreamProcessing.Contracts;

namespace StreamProcessing.Consumer.DataPersistence.ElasticSearch;

public class ElasticSearchAggregatedDataWritingStrategy : IDataWritingStrategy
{
    public async Task Write(ScadaDataPoint dataPoint)
    {
        await Task.CompletedTask;
    }
}