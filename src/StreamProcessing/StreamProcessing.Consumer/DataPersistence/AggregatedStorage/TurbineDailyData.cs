namespace StreamProcessing.Consumer.DataPersistence.AggregatedStorage;

public record TurbineDailyData(
    DateTime Date,
    double AvgPower,
    double MinPower,
    double MaxPower,
    double AvgWindSpeed,
    double MinWindSpeed,
    double MaxWindSpeed);