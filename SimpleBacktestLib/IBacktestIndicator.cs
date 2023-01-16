namespace SimpleBacktestLib;

public interface IBacktestIndicator<T>
{
    /// <summary>
    /// Define the implementation of a custom indicator.
    /// Accessible through TickData.Indicators[nameof(MyIndicatorType)]
    /// </summary>
    /// <param name="historicalData"></param>
    /// <param name="currentIndex"></param>
    /// <returns></returns>
    T Calculate(IEnumerable<double> historicalData, int currentIndex);
}
