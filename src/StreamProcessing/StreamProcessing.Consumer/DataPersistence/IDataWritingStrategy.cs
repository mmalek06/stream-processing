using StreamProcessing.Contracts;

namespace StreamProcessing.Consumer.DataPersistence;

public interface IDataWritingStrategy : IDisposable
{
    Task Write(ScadaDataPoint dataPoint);

    /// <summary>
    /// Some implementations may buffer data points for more efficient writing,
    /// as well as decide to aggregate those datapoints. In each case a final
    /// flush is required to ensure all of the data has been saved.
    /// </summary>
    Task Flush();
}