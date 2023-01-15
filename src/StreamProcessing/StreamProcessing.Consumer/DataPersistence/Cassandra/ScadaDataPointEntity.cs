using Cassandra;

namespace StreamProcessing.Consumer.DataPersistence.Cassandra;

public record ScadaDataPointEntity(
    LocalDate Date,
    LocalTime Time,
    double ActivePowerLevel,
    double WindSpeed,
    double TheoreticalPowerCurve,
    double WindDirection);