using System.Collections.Generic;
using System.Linq;

namespace QuantConnect.Indicators;

public class WindowExponentialMovingAverage : Indicator, IIndicatorWarmUpPeriodProvider
{
    private readonly decimal _k;
    private readonly int _windowSize;
    private readonly Queue<decimal> _queue;

    /// <summary>
    /// Gets a flag indicating when this indicator is ready and fully initialized
    /// </summary>
    public override bool IsReady => Samples >= _windowSize;

    /// <summary>
    /// Required period, in data points, for the indicator to be ready and fully initialized.
    /// </summary>
    public int WarmUpPeriod => _windowSize;

    /// <summary>
    /// Initializes a new instance of the WilderMovingAverage class with the default name, k and windowSize
    /// </summary>
    /// <param name="k">The ratio = 1/period of the Wilder Moving Average</param>
    /// <param name="windowSize">The size of the counting window of the Wilder Moving Average</param>
    public WindowExponentialMovingAverage(decimal k, int windowSize)
        : this("WEMA" + k + "_" + windowSize, k, windowSize)
    {
    }

    /// <summary>
    /// Initializes a new instance of the WilderMovingAverage class with the specified name and period
    /// </summary>
    /// <param name="name">The name of this indicator</param>
    /// <param name="k">The ratio = 1/period of the Wilder Moving Average</param>
    /// <param name="windowSize">The size of the counting window of the Wilder Moving Average</param>
    public WindowExponentialMovingAverage(string name, decimal k, int windowSize)
        : base(name)
    {
        this._k = k;
        this._windowSize = windowSize;
        _queue = new Queue<decimal>();
    }

    /// <summary/>
    /// Computes the next value of this indicator from window
    private decimal CalculateValue()
    {
        return _queue.ToArray().Aggregate<decimal, decimal>(0, (current, e) => e * _k + current * (1M - _k));
    }

    /// <summary>
    /// Computes the next value of this indicator from the given state
    /// </summary>
    /// <param name="input">The input given to the indicator</param>
    /// <returns>A new value for this indicator</returns>
    protected override decimal ComputeNextValue(IndicatorDataPoint input)
    {
        _queue.Enqueue(input.Value);

        while (_queue.Count > _windowSize)
        {
            _queue.Dequeue();
        }

        return _queue.Count == _windowSize
            ? CalculateValue()
            : input.Value;
    }
}
