using System.Globalization;
using Confluent.Kafka;
using StreamProcessing.Contracts;
using StreamProcessing.Producer;
using StreamProcessing.Resources;

Console.WriteLine(@"Running publisher - this will simulate the process of sending data belonging to one wind turbine.");

var lines = GetLines();
var config = new ProducerConfig
{
    BootstrapServers = "localhost:29092"
};
using var producer = new ProducerBuilder<Null, ScadaDataPoint>(config)
    .SetValueSerializer(new ScadaDataPointSerializer())
    .Build();

await Task.WhenAll(lines.Select(async line =>
{
    var dataPoint = CreateDataPoint(line);
    
    await producer.ProduceAsync("scada-stream", new Message<Null, ScadaDataPoint> { Value = dataPoint });
}));

Console.WriteLine(@"Publisher finished, press any key to exit...");
Console.ReadKey();

IEnumerable<string> GetLines() =>
    ((string)Resource.ResourceManager.GetObject("T1", Resource.Culture)!)
        .Trim()
        .Split(Environment.NewLine)
        .Skip(1);

ScadaDataPoint CreateDataPoint(string s)
{
    var parts = s
        .Split(',')
        .Select(x => x.Trim())
        .ToArray();
    var date = DateTime.ParseExact(parts[0], "dd MM yyyy HH:mm", CultureInfo.InvariantCulture);
    var activePowerLevel = double.Parse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture);
    var msWindSpeed = double.Parse(parts[2], NumberStyles.Number, CultureInfo.InvariantCulture);
    var theoreticalPowerCurve = double.Parse(parts[3], NumberStyles.Number, CultureInfo.InvariantCulture);
    var windDirection = double.Parse(parts[4], NumberStyles.Number, CultureInfo.InvariantCulture);
    
    return new ScadaDataPoint(date, activePowerLevel, msWindSpeed, theoreticalPowerCurve, windDirection);
}