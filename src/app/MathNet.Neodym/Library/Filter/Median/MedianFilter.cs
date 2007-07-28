using System;
using System.Collections.Generic;
using System.Text;
using MathNet.SignalProcessing.Filter.Utils;

namespace MathNet.SignalProcessing.Filter.Median
{
    public class OnlineMedianFilter : OnlineFilter
    {
        private OrderedShiftBuffer _buffer;

        public OnlineMedianFilter(int windowSize)
        {
            _buffer = new OrderedShiftBuffer(windowSize);
        }

        public override double ProcessSample(double sample)
        {
            _buffer.Append(sample);
            try
            {
                return _buffer.Median;
            }
            catch(NullReferenceException)
            {
                return double.NaN;
            }
        }

        public override void Reset()
        {
            _buffer.Clear();
        }
    }
}
