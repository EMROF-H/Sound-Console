using DigitalSignalProcess;
using SoundConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Filter
{
    public enum Type
    {
        LowPass,
        HighPass,
        BandPass,
        BandStop
    }

    public class Filter
    {
        private readonly Type Type;

        private readonly int Order;
        private readonly Frequency ParameterLow;
        private readonly Frequency ParameterHigh;

        private double[] UnitImpulseResponse;

        public Filter(Type type, int order, Frequency parameterLow, Frequency parameterHigh, Windows.Type windowType = Windows.Type.Rectangle)
        {
            this.Type = type;
            this.Order = (order % 2 == 0) ? order + 1 : order;
            this.ParameterLow = parameterLow;
            this.ParameterHigh = parameterHigh;

            GeneratingIdealFilter(windowType);
        }

        private void GeneratingIdealFilter(Windows.Type windowType)
        {
            this.UnitImpulseResponse = new double[this.Order];
            int alpha = (this.Order - 1) / 2;
            for(int i = 0; i < this.Order; i++)
            {
                this.UnitImpulseResponse[i] = IdealUnitImpulseResponse(i - alpha) * Windows.WindowFunction(i, this.Order, windowType);
            }
        }

        private double IdealUnitImpulseResponse(int n)
        {
            switch(this.Type)
            {
                case Type.LowPass:  return Sequence.FilterElement(n, ParameterLow.DigitalFrequency);
                case Type.HighPass: return Sequence.UnitImpulse(n) - Sequence.FilterElement(n, ParameterHigh.DigitalFrequency);
                case Type.BandPass: return Sequence.FilterElement(n, ParameterHigh.DigitalFrequency) - Sequence.FilterElement(n, ParameterLow.DigitalFrequency);
                case Type.BandStop: return Sequence.UnitImpulse(n) + Sequence.FilterElement(n, ParameterLow.DigitalFrequency) - Sequence.FilterElement(n, ParameterHigh.DigitalFrequency);
                default: return 1;
            }
        }

        //public short[] Process(short[] input, ProgressBar progressBar)
        public async Task<short[]> Process(short[] input, ProgressBar progressBar)
        {
            InfiniteSequence inputInfinite = new InfiniteSequence(input);
            short[] result = new short[input.Length];

            await Task.Run(() =>
            {
                int node = result.Length / 500;
                for (int index = 0; index < result.Length; index++)
                {
                    double value = 0;
                    for (int n = 0; n < this.Order; n++)
                    {
                        value += inputInfinite[index - n] * UnitImpulseResponse[n];
                    }
                    result[index] = (short)value;

                    if (index % node == 0)
                    {
                        MainWindow.MainDispatcher.Invoke(() =>
                        {
                            progressBar.Value = 100.0 * index / result.Length;
                        });
                    }
                }
            });

            return result;
        }

        /*
        #region 运算符重载
        public static short[] operator *(short[] left,Filter right)
        {
            return right.Process(left);
        }

        public static short[] operator *(Filter left, short[] right)
        {
            return left.Process(right);
        }
        #endregion
        */

        public static Type ToFilterType(string s)
        {
            if(s == "Low Pass")
            {
                return Type.LowPass;
            }
            else if(s == "High Pass")
            {
                return Type.HighPass;
            }
            else if(s == "Band Pass")
            {
                return Type.BandPass;
            }
            else if(s == "Band Stop")
            {
                return Type.BandStop;
            }
            else
            {
                return Type.LowPass;
            }
        }
    }

    //public delegate double Window(int n, int N);
    public static class Windows
    {
        public enum Type
        {
            Rectangle,
            Hanning,
            Hamming,
            Blackman,
            Bartlett
        }

        public static Windows.Type ToWindowTyoe(string s)
        {
            if (s == "Rectangle")
            {
                return Windows.Type.Rectangle;
            }
            else if (s == "Hanning")
            {
                return Windows.Type.Hanning;
            }
            else if (s == "Hamming")
            {
                return Windows.Type.Hamming;
            }
            else if (s == "Blackman")
            {
                return Windows.Type.Blackman;
            }
            else if(s== "Bartlett")
            {
                return Windows.Type.Bartlett;
            }
            else
            {
                return Windows.Type.Rectangle;
            }
        }

        public static double WindowFunction(int n, int N, Type type)
        {
            switch(type)
            {
                case Type.Rectangle: return Rectangle(n, N);
                case Type.Hanning:   return Hanning(n, N);
                case Type.Hamming:   return Hamming(n, N);
                case Type.Blackman:  return Blackman(n, N);
                case Type.Bartlett:  return Bartlett(n, N);
                default: return Rectangle(n, N);
            }
        }

        private static double Rectangle(int n, int N)
        {
            if (0 <= n && n <= N - 1)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private static double Hanning(int n, int N)
        {
            if (0 <= n && n <= N - 1)
            {
                return 0.50 * BlackmanFamilyFactor(0, n, N)
                     + 0.50 * BlackmanFamilyFactor(1, n, N);
            }
            else
            {
                return 0;
            }
        }

        private static double Hamming(int n, int N)
        {
            if (0 <= n && n <= N - 1)
            {
                return 0.54 * BlackmanFamilyFactor(0, n, N)
                     + 0.46 * BlackmanFamilyFactor(1, n, N);
            }
            else
            {
                return 0;
            }
        }

        private static double Blackman(int n, int N)
        {
            if (0 <= n && n <= N - 1)
            {
                return 0.42 * BlackmanFamilyFactor(0, n, N)
                     + 0.50 * BlackmanFamilyFactor(1, n, N)
                     + 0.08 * BlackmanFamilyFactor(2, n, N);
            }
            else
            {
                return 0;
            }
        }

        private static double Bartlett(int n, int N)
        {
            if (0 <= n && n <= N - 1)
            {
                if (n <= (N - 1) / 2)
                {
                    return 2.0 * n / (N - 1);
                }
                else
                {
                    return 2.0 - 2.0 * n / (N - 1);
                }
            }
            else
            {
                return 0;
            }
        }

        private static double BlackmanFamilyFactor(int order, int n, int N)
        {
            return ((order % 2 == 0) ? 1 : -1) * Math.Cos(order * Basic.TAU * n / (N - 1));
        }
    }
}
