using StreamProcessing.Contracts;

namespace StreamProcessing.Consumer.DataPersistence;

public interface IDataWritingStrategy
{
    Task Write(ScadaDataPoint dataPoint);
}