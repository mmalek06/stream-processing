using Nest;
using StreamProcessing.Contracts;

namespace StreamProcessing.Consumer.DataPersistence.AggregatedStorage;

public class ElasticSearchAggregatedDataWritingStrategy : IDataWritingStrategy
{
    public const string Index = "daily-turbine-data";
    
    private readonly TurbineDailyDataPoints _aggregate;
    private readonly IElasticClient _client;
    
    private DateOnly? _lastDate;
    
    public ElasticSearchAggregatedDataWritingStrategy(IElasticClient elasticClient)
    {
        _aggregate = new TurbineDailyDataPoints(new List<double>(), new List<double>());
        _client = elasticClient;
    }

    public async Task Write(ScadaDataPoint dataPoint)
    {
        if (!_aggregate.PowerLevels.Any())
            ReInitialize(dataPoint);
        else
        {
            var currentDate = DateOnly.FromDateTime(dataPoint.Date);

            if (currentDate != _lastDate)
            {
                await PersistAggregate();
                
                ReInitialize(dataPoint);
            }
            else
                Update(dataPoint);
        }
    }

    public async Task Flush() =>
        await PersistAggregate();
    
    public void Dispose()
    {
    }

    private void ReInitialize(ScadaDataPoint dataPoint)
    {
        _aggregate.PowerLevels.Clear();
        _aggregate.WindSpeeds.Clear();
        _aggregate.PowerLevels.Add(dataPoint.ActivePowerLevel);
        _aggregate.WindSpeeds.Add(dataPoint.MsWindSpeed);
        
        _lastDate = DateOnly.FromDateTime(dataPoint.Date);
    }
    
    private async Task PersistAggregate()
    {
        await _client.ConfigureIfRequired(Index);
        
        var dailyDataEntity = new TurbineDailyData(
            _lastDate!.Value.ToDateTime(TimeOnly.MinValue),
            _aggregate.PowerLevels.Average(),
            _aggregate.PowerLevels.Min(),
            _aggregate.PowerLevels.Max(),
            _aggregate.WindSpeeds.Average(),
            _aggregate.WindSpeeds.Min(),
            _aggregate.WindSpeeds.Max());

        await _client.IndexDocumentAsync(dailyDataEntity);
    }
    
    private void Update(ScadaDataPoint dataPoint)
    {
        _aggregate!.PowerLevels.Add(dataPoint.ActivePowerLevel);
        _aggregate!.WindSpeeds.Add(dataPoint.MsWindSpeed);
    }

    private record TurbineDailyDataPoints(List<double> PowerLevels, List<double> WindSpeeds);
}