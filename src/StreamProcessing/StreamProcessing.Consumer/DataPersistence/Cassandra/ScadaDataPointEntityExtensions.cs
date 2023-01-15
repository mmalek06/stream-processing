using Cassandra;
using StreamProcessing.Contracts;

namespace StreamProcessing.Consumer.DataPersistence.Cassandra;

public static class ScadaDataPointEntityExtensions
{
    public static ScadaDataPointEntity ToEntity(this ScadaDataPoint dataPoint) =>
        new(
            new LocalDate(dataPoint.Date.Year, dataPoint.Date.Month, dataPoint.Date.Day),
            new LocalTime(dataPoint.Date.Hour, dataPoint.Date.Minute, dataPoint.Date.Second, dataPoint.Date.Nanosecond),
            dataPoint.ActivePowerLevel,
            dataPoint.MsWindSpeed,
            dataPoint.TheoreticalPowerCurve,
            dataPoint.WindDirection);
}