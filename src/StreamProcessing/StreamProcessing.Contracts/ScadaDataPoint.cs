namespace StreamProcessing.Contracts;

public record ScadaDataPoint(
    DateTime Date,
    double ActivePowerLevel,
    double MsWindSpeed,
    double TheoreticalPowerCurve,
    double WindDirection);