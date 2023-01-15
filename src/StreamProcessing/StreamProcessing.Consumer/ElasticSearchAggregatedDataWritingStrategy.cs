using StreamProcessing.Contracts;

namespace StreamProcessing.Consumer;

public class ElasticSearchAggregatedDataWritingStrategy : IDataWritingStrategy
{
    public Task Write(ScadaDataPoint dataPoint)
    {
        throw new NotImplementedException();
    }
}