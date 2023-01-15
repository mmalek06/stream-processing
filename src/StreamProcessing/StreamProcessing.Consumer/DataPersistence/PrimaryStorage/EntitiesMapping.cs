using Cassandra.Mapping;

namespace StreamProcessing.Consumer.DataPersistence.PrimaryStorage;

public static class EntitiesMapping
{
    private static bool _mapped = false;

    public static bool EntitiesMapped() =>
        _mapped;
    
    public static void MapEntities()
    {
        if (_mapped)
            return;
        
        var entitiesMap = new Map<ScadaDataPointEntity>();

        entitiesMap
            .TableName("scada_datapoints")
            .Column(e => e.Date, map => map.WithName("date"))
            .Column(e => e.Time, map => map.WithName("time"))
            .Column(e => e.WindDirection, map => map.WithName("wind_direction"))
            .Column(e => e.TheoreticalPowerCurve, map => map.WithName("theoretical_power_curve"))
            .Column(e => e.ActivePowerLevel, map => map.WithName("active_power_level"))
            .Column(e => e.WindSpeed, map => map.WithName("wind_speed"))
            .PartitionKey(e => e.Date)
            .ClusteringKey(e => e.Time, SortOrder.Ascending);
        
        MappingConfiguration.Global.Define(entitiesMap);

        _mapped = true;
    }
}