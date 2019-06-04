// <copyright file="Designer.cs" company="Math.NET">
// Math.NET Filtering, part of the Math.NET Project
// http://filtering.mathdotnet.com
// http://github.com/mathnet/mathnet-filtering
//
// Copyright (c) 2009-2019 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;

namespace MathNet.Filtering.Butterworth
{
    /// <summary>
    /// Computes the minimum order and the cutoff frequencies, starting from the design parameters of a Butterworth filter.
    /// </summary>
    public static class Designer
    {
        #region Static Methods

        /// <summary>
        /// Designs a low-pass Butterworth filter according to the specification.
        /// </summary>
        /// <param name="passbandFreq">Passband corner frequency, normalized to Nyquist frequency.</param>
        /// <param name="stopbandFreq">Stopband corner frequency, normalized to Nyquist frequency.</param>
        /// <param name="passbandRipple">Maximum allowed passband ripple.</param>
        /// <param name="stopbandAttenuation">Minimum required stopband attenuation.</param>
        /// <returns>Minimum required filter order and computed cutoff frequency.</returns>
        /// <exception cref="ArgumentException">Passband corner frequency is not in [0,1] range.</exception>
        /// <exception cref="ArgumentException">Stopband corner frequency is not in [0,1] range.</exception>
        /// <exception cref="ArgumentException">Passband ripple must be a finite number.</exception>
        /// <exception cref="ArgumentException">Stopband attenuation must be a finite number.</exception>
        /// <exception cref="ArgumentException">Stopband corner frequency must be greater than passband corner frequency.</exception>
        /// <exception cref="ArgumentException">Stopband attenuation must be greater than passband ripple.</exception>
        public static (byte n, double wc) LowPass(double passbandFreq, double stopbandFreq, double passbandRipple, double stopbandAttenuation)
        {
            Helpers.Validators.CheckFrequency(passbandFreq, nameof(passbandFreq));
            Helpers.Validators.CheckFrequency(stopbandFreq, nameof(stopbandFreq));
            Helpers.Validators.CheckDouble(passbandRipple, nameof(passbandRipple));
            Helpers.Validators.CheckDouble(stopbandAttenuation, nameof(stopbandAttenuation));

            if (stopbandFreq < passbandFreq)
            {
                throw new ArgumentException("Stopband corner frequency must be greater than passband corner frequency.", nameof(stopbandFreq));
            }

            if (stopbandAttenuation < passbandRipple)
            {
                throw new ArgumentException("Stopband attenuation must be greater than passband ripple.", nameof(stopbandAttenuation));
            }

            var wwp = Math.Tan(Math.PI * passbandFreq / 2);
            var wws = Math.Tan(Math.PI * stopbandFreq / 2);

            var qp = Math.Log(Math.Pow(10, passbandRipple / 10) - 1);
            var qs = Math.Log(Math.Pow(10, stopbandAttenuation / 10) - 1);

            var n = (byte)Math.Ceiling((qs - qp) / (2 * (Math.Log(wws) - Math.Log(wwp))));

            var wwcp = Math.Exp(Math.Log(wwp) - (qp / 2 / n));

            return (n, Math.Atan(wwcp) * 2 / Math.PI);
        }

        /// <summary>
        /// Designs a high-pass Butterworth filter according to the specification.
        /// </summary>
        /// <param name="stopbandFreq">Stopband corner frequency, normalized to Nyquist frequency.</param>
        /// <param name="passbandFreq">Passband corner frequency, normalized to Nyquist frequency.</param>
        /// <param name="passbandRipple">Maximum allowed passband ripple.</param>
        /// <param name="stopbandAttenuation">Minimum required stopband attenuation.</param>
        /// <returns>Minimum required filter order and computed cutoff frequency.</returns>
        /// <exception cref="ArgumentException">Passband corner frequency is not in [0,1] range.</exception>
        /// <exception cref="ArgumentException">Stopband corner frequency is not in [0,1] range.</exception>
        /// <exception cref="ArgumentException">Passband ripple must be a finite number.</exception>
        /// <exception cref="ArgumentException">Stopband attenuation must be a finite number.</exception>
        /// <exception cref="ArgumentException">Stopband corner frequency must be lesser than passband corner frequency.</exception>
        /// <exception cref="ArgumentException">Stopband attenuation must be greater than passband ripple.</exception>
        public static (byte n, double wc) HighPass(double stopbandFreq, double passbandFreq, double passbandRipple, double stopbandAttenuation)
        {
            Helpers.Validators.CheckFrequency(passbandFreq, nameof(passbandFreq));
            Helpers.Validators.CheckFrequency(stopbandFreq, nameof(stopbandFreq));
            Helpers.Validators.CheckDouble(passbandRipple, nameof(passbandRipple));
            Helpers.Validators.CheckDouble(stopbandAttenuation, nameof(stopbandAttenuation));

            if (stopbandFreq > passbandFreq)
            {
                throw new ArgumentException("Stopband corner frequency must be lesser than passband corner frequency.", nameof(stopbandFreq));
            }

            if (stopbandAttenuation < passbandRipple)
            {
                throw new ArgumentException("Stopband attenuation must be greater than passband ripple.", nameof(stopbandAttenuation));
            }

            var wwp = Math.Tan(Math.PI * passbandFreq / 2);
            var wws = Math.Tan(Math.PI * stopbandFreq / 2);

            var qp = Math.Log(Math.Pow(10, passbandRipple / 10) - 1);
            var qs = Math.Log(Math.Pow(10, stopbandAttenuation / 10) - 1);

            var n = (byte)Math.Ceiling((qs - qp) / (2 * (Math.Log(wwp) - Math.Log(wws))));

            var wwcp = Math.Exp(Math.Log(wwp) + (qp / 2 / n));

            return (n, Math.Atan(wwcp) * 2 / Math.PI);
        }

        /// <summary>
        /// Designs a band-pass Butterworth filter according to the specification.
        /// </summary>
        /// <param name="lowStopbandFreq">Lower stopband corner frequency, normalized to Nyquist frequency.</param>
        /// <param name="lowPassbandFreq">Lower passband corner frequency, normalized to Nyquist frequency.</param>
        /// <param name="highPassbandFreq">Higher passband corner frequency, normalized to Nyquist frequency.</param>
        /// <param name="highStopbandFreq">Higher stopband corner frequency, normalized to Nyquist frequency.</param>
        /// <param name="passbandRipple">Maximum allowed passband ripple.</param>
        /// <param name="stopbandAttenuation">Minimum required stopband attenuation.</param>
        /// <returns>Minimum required filter order and computed cutoff frequency.</returns>
        /// <exception cref="ArgumentException">Lower passband corner frequency is not in [0,1] range.</exception>
        /// <exception cref="ArgumentException">Lower stopband corner frequency is not in [0,1] range.</exception>
        /// <exception cref="ArgumentException">Higher passband corner frequency is not in [0,1] range.</exception>
        /// <exception cref="ArgumentException">Higher stopband corner frequency is not in [0,1] range.</exception>
        /// <exception cref="ArgumentException">Passband ripple must be a finite number.</exception>
        /// <exception cref="ArgumentException">Stopband attenuation must be a finite number.</exception>
        /// <exception cref="ArgumentException">Lower stopband corner frequency must be lesser than lower passband corner frequency.</exception>
        /// <exception cref="ArgumentException">Lower passband corner frequency must be lesser than higher passband corner frequency.</exception>
        /// <exception cref="ArgumentException">Higher stopband corner frequency must be greater than higher passband corner frequency.</exception>
        /// <exception cref="ArgumentException">Stopband attenuation must be greater than passband ripple.</exception>
        public static (byte n, double wc1, double wc2) BandPass(double lowStopbandFreq, double lowPassbandFreq, double highPassbandFreq, double highStopbandFreq, double passbandRipple, double stopbandAttenuation)
        {
            Helpers.Validators.CheckFrequency(lowStopbandFreq, nameof(lowStopbandFreq));
            Helpers.Validators.CheckFrequency(lowPassbandFreq, nameof(lowPassbandFreq));
            Helpers.Validators.CheckFrequency(highPassbandFreq, nameof(highPassbandFreq));
            Helpers.Validators.CheckFrequency(highPassbandFreq, nameof(highPassbandFreq));
            Helpers.Validators.CheckDouble(passbandRipple, nameof(passbandRipple));
            Helpers.Validators.CheckDouble(stopbandAttenuation, nameof(stopbandAttenuation));

            if (lowStopbandFreq > lowPassbandFreq)
            {
                throw new ArgumentException("Lower stopband corner frequency must be lesser than lower passband corner frequency.", nameof(lowStopbandFreq));
            }

            if (lowPassbandFreq > highPassbandFreq)
            {
                throw new ArgumentException("Lower passband corner frequency must be lesser than higher passband corner frequency.", nameof(highPassbandFreq));
            }

            if (highStopbandFreq < highPassbandFreq)
            {
                throw new ArgumentException("Higher stopband corner frequency must be greater than higher passband corner frequency.", nameof(highStopbandFreq));
            }

            if (stopbandAttenuation < passbandRipple)
            {
                throw new ArgumentException("Stopband attenuation must be greater than passband ripple.", nameof(stopbandAttenuation));
            }

            var wwp1 = Math.Tan(Math.PI * lowPassbandFreq / 2);
            var wwp2 = Math.Tan(Math.PI * highPassbandFreq / 2);
            var wws1 = Math.Tan(Math.PI * lowStopbandFreq / 2);
            var wws2 = Math.Tan(Math.PI * highStopbandFreq / 2);

            var w02 = wwp1 * wwp2;
            if (w02 < wws1 * wws2)
            {
                wws2 = w02 / wws1;
            }
            else
            {
                wws1 = w02 / wws2;
            }

            const double wwp = 1d;
            var wws = (wws2 - wws1) / (wwp2 - wwp1);

            var qp = Math.Log(Math.Pow(10, passbandRipple / 10) - 1);
            var qs = Math.Log(Math.Pow(10, stopbandAttenuation / 10) - 1);
            var n = (byte)Math.Ceiling((qs - qp) / (2 * (Math.Log(wws) - Math.Log(wwp))));

            var wpp1 = Math.Exp(Math.Log(wwp1) - (qp / 2 / n));

            var (wb, wa) = CutoffFrequencies(wwp1, wwp2, wpp1);

            return (n, Math.Atan(wb) * 2 / Math.PI, Math.Atan(wa) * 2 / Math.PI);
        }

        /// <summary>
        /// Designs a band-stop Butterworth filter according to the specification.
        /// </summary>
        /// <param name="lowPassbandFreq">Lower passband corner frequency, normalized to Nyquist frequency.</param>
        /// <param name="lowStopbandFreq">Lower stopband corner frequency, normalized to Nyquist frequency.</param>
        /// <param name="highStopbandFreq">Higher stopband corner frequency, normalized to Nyquist frequency.</param>
        /// <param name="highPassbandFreq">Higher passband corner frequency, normalized to Nyquist frequency.</param>
        /// <param name="passbandRipple">Maximum allowed passband ripple.</param>
        /// <param name="stopbandAttenuation">Minimum required stopband attenuation.</param>
        /// <returns>Minimum required filter order and computed cutoff frequency.</returns>
        /// <exception cref="ArgumentException">Lower passband corner frequency is not in [0,1] range.</exception>
        /// <exception cref="ArgumentException">Lower stopband corner frequency is not in [0,1] range.</exception>
        /// <exception cref="ArgumentException">Higher passband corner frequency is not in [0,1] range.</exception>
        /// <exception cref="ArgumentException">Higher stopband corner frequency is not in [0,1] range.</exception>
        /// <exception cref="ArgumentException">Passband ripple must be a finite number.</exception>
        /// <exception cref="ArgumentException">Stopband attenuation must be a finite number.</exception>
        /// <exception cref="ArgumentException">Lower stopband corner frequency must be greater than lower passband corner frequency.</exception>
        /// <exception cref="ArgumentException">Lower stopband corner frequency must be lesser than higher passband corner frequency.</exception>
        /// <exception cref="ArgumentException">Higher stopband corner frequency must be lesser than higher passband corner frequency.</exception>
        /// <exception cref="ArgumentException">Stopband attenuation must be greater than passband ripple.</exception>
        public static (byte n, double wc1, double wc2) BandStop(double lowPassbandFreq, double lowStopbandFreq, double highStopbandFreq, double highPassbandFreq, double passbandRipple, double stopbandAttenuation)
        {
            Helpers.Validators.CheckFrequency(lowStopbandFreq, nameof(lowStopbandFreq));
            Helpers.Validators.CheckFrequency(lowPassbandFreq, nameof(lowPassbandFreq));
            Helpers.Validators.CheckFrequency(highPassbandFreq, nameof(highPassbandFreq));
            Helpers.Validators.CheckFrequency(highPassbandFreq, nameof(highPassbandFreq));
            Helpers.Validators.CheckDouble(passbandRipple, nameof(passbandRipple));
            Helpers.Validators.CheckDouble(stopbandAttenuation, nameof(stopbandAttenuation));

            if (lowPassbandFreq > lowStopbandFreq)
            {
                throw new ArgumentException("Lower stopband corner frequency must be greater than lower passband corner frequency.", nameof(lowStopbandFreq));
            }

            if (lowStopbandFreq > highStopbandFreq)
            {
                throw new ArgumentException("Lower stopband corner frequency must be lesser than higher passband corner frequency.", nameof(highStopbandFreq));
            }

            if (highStopbandFreq > highPassbandFreq)
            {
                throw new ArgumentException("Higher stopband corner frequency must be lesser than higher passband corner frequency.", nameof(highStopbandFreq));
            }

            if (stopbandAttenuation < passbandRipple)
            {
                throw new ArgumentException("Stopband attenuation must be greater than passband ripple.", nameof(stopbandAttenuation));
            }

            var wwp1 = Math.Tan(Math.PI * lowPassbandFreq / 2);
            var wwp2 = Math.Tan(Math.PI * highPassbandFreq / 2);
            var wws1 = Math.Tan(Math.PI * lowStopbandFreq / 2);
            var wws2 = Math.Tan(Math.PI * highStopbandFreq / 2);

            var w02 = wwp1 * wwp2;
            if (w02 > wws1 * wws2)
            {
                wwp2 = (wws1 * wws2) / wwp1;
            }
            else
            {
                wwp1 = (wws1 * wws2) / wwp2;
            }

            var wwp = w02 / (wwp2 - wwp1);
            var wws = w02 / (wws2 - wws1);

            wws /= wwp;
            wwp = 1;

            var qp = Math.Log(Math.Pow(10, passbandRipple / 10) - 1);
            var qs = Math.Log(Math.Pow(10, stopbandAttenuation / 10) - 1);
            var n = (byte)Math.Ceiling((qs - qp) / (2 * (Math.Log(wws) - Math.Log(wwp))));

            var wpp1 = Math.Exp(Math.Log(wwp1) + (qp / 2 / n));
            var wpp2 = Math.Exp(Math.Log(wwp2) + (qp / 2 / n));
            var wsp1 = Math.Exp(Math.Log(wws1) + (qs / 2 / n));
            var wsp2 = Math.Exp(Math.Log(wws2) + (qs / 2 / n));

            var (wb, wa) = CutoffFrequencies(wwp1, wwp2, wpp1);

            return (n, Math.Atan(wb) * 2 / Math.PI, Math.Atan(wa) * 2 / Math.PI);
        }

        /// <summary>
        /// Designs a notch Butterworth filter according to the specification.
        /// </summary>
        /// <param name="centralFreq">Filter central frequency, normalized to Nyquist frequency.</param>
        /// <param name="Q">Quality factor.</param>
        /// <param name="passbandRipple">Maximum allowed passband ripple.</param>
        /// <param name="stopbandAttenuation">Minimum required stopband attenuation.</param>
        /// <returns>Minimum required filter order and computed cutoff frequency.</returns>
        /// <exception cref="ArgumentException">Central frequency is not in [0,1] range.</exception>
        /// <exception cref="ArgumentException">Q must be a finite number.</exception>
        /// <exception cref="ArgumentException">Passband ripple must be a finite number.</exception>
        /// <exception cref="ArgumentException">Stopband attenuation must be a finite number.</exception>
        /// <exception cref="ArgumentException">Invalid quality factor.</exception>
        public static (byte n, double wc1, double wc2) Notch(double centralFreq, double Q, double passbandRipple, double stopbandAttenuation)
        {
            Helpers.Validators.CheckFrequency(centralFreq, nameof(centralFreq));
            Helpers.Validators.CheckDouble(Q, nameof(Q));
            Helpers.Validators.CheckDouble(passbandRipple, nameof(passbandRipple));
            Helpers.Validators.CheckDouble(stopbandAttenuation, nameof(stopbandAttenuation));

            if (Q <= -centralFreq / ((centralFreq * centralFreq) - 1))
            {
                throw new ArgumentException("Invalid quality factor.", nameof(Q));
            }

            var sqrt = Math.Sqrt((4 * Q * Q) + 1);
            var wp1 = ((sqrt * centralFreq) - centralFreq) / (2 * Q);
            var wp2 = ((sqrt * centralFreq) + centralFreq) / (2 * Q);
            return BandStop(wp1, centralFreq - 1e-10, centralFreq + 1e-10, wp2, passbandRipple, stopbandAttenuation);
        }

        #endregion Static Methods

        #region Private Static Methods

        /// <summary>
        /// Computes the cutoff frequencies for a band-pass or a band-stop filter.
        /// </summary>
        private static (double wb, double wa) CutoffFrequencies(double wwp1, double wwp2, double wpp1)
        {
            var w0 = Math.Sqrt(wwp1 * wwp2);
            var Q = w0 / (wwp2 - wwp1);
            var wc1 = wwp1;

            var wp = wpp1 / wc1;
            var wa = Math.Abs(wp + Math.Sqrt((wp * wp) + (4 * Q * Q))) / (2 * Q / w0);
            var wb = Math.Abs(wp - Math.Sqrt((wp * wp) + (4 * Q * Q))) / (2 * Q / w0);

            return (wb, wa);
        }

        #endregion Private Static Methods
    }
}
