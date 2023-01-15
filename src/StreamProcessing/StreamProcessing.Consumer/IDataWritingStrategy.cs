using StreamProcessing.Contracts;

namespace StreamProcessing.Consumer;

public interface IDataWritingStrategy
{
    Task Write(ScadaDataPoint dataPoint);
}