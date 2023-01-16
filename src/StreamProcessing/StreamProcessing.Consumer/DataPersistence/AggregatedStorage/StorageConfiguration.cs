using Nest;

namespace StreamProcessing.Consumer.DataPersistence.AggregatedStorage;

public static class StorageConfiguration
{
    private static bool _indexExists = false;
    
    public static async ValueTask ConfigureIfRequired(this IElasticClient client, string index)
    {
        if (_indexExists)
            return;
        
        if (!(await client.Indices.ExistsAsync(index)).Exists)
        {
            var response = await client.Indices.CreateAsync(
                index,
                c => c.Map<TurbineDailyData>(t => t.AutoMap()));

            _indexExists = true;
        }
    }
}