using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalSignalProcess
{
    public static class Sequence
    {
        public static int UnitImpulse(int n)
        {
            return n == 0 ? 1 : 0;
        }

        public static double FilterElement(int n, double frequency)
        {
            return n == 0 ? frequency / Math.PI : Math.Sin(frequency * n) / (Math.PI * n);
        }
    }

    public class InfiniteSequence
    {
        private short[] sequence;

        public int Size { get { return sequence.Length; } }

        public short this[int index]
        {
            get
            {
                if (0 <= index && index <= sequence.Length - 1)
                {
                    return sequence[index];
                }
                else
                {
                    return 0;
                }
            }
        }

        public InfiniteSequence(short[] sequence) { this.sequence = sequence; }

        public short[] Truncation(int indexBegin, int size)
        {
            short[] result = new short[size];
            for(int i = 0; i < size; i++)
            {
                result[i] = this[indexBegin + i];
            }
            return result;
        }
    }

    public static class Basic
    {
        public const double TAU = 2 * Math.PI;

        public static double Sinc(double x)
        {
            if (x == 0)
            {
                return 1;
            }
            else
            {
                return Math.Sin(x) / x;
            }
        }
    }

    public struct Frequency
    {
        public readonly double SampleFrequency; //采样频率 Hz

        public readonly double AnalogFrequency; //模拟频率 Hz

        public double AnalogAngularFrequency { get { return Basic.TAU * AnalogFrequency; } }

        public double DigitalFrequency { get { return AnalogAngularFrequency / SampleFrequency; } }

        public Frequency(double analogFrequency,double sampleFrequency)
        {
            this.AnalogFrequency = analogFrequency;
            this.SampleFrequency = sampleFrequency;
        }
    }

    public struct Complex
    {
        public double Real;
        public double Imaginary;

        public double AbsoluteSquare { get { return Math.Pow(this.Real, 2) + Math.Pow(this.Imaginary, 2); } }
        public double Absolute { get { return Math.Sqrt(this.AbsoluteSquare); } }

        public Complex(double real = 0) : this(real, 0) { }
        public Complex(double real, double imaginary)
        {
            this.Real = real;
            this.Imaginary = imaginary;
        }

        #region 等于
        public override bool Equals(Object value)
        {
            if (value.GetType() == typeof(Complex))
            {
                return this.Real == ((Complex)value).Real && this.Imaginary == ((Complex)value).Imaginary;
            }
            else
            {
                return false;
            }
        }
        public static bool operator ==(Complex left, Complex right)
        {
            return left.Real == right.Real && left.Imaginary == right.Imaginary;
        }

        public static bool operator !=(Complex left, Complex right)
        {
            return !(left == right);
        }
        #endregion

        #region 加法
        public static Complex operator +(Complex left, Complex right)
        {
            left.Real += right.Real;
            left.Imaginary += right.Imaginary;
            return left;
        }

        public static Complex operator +(double left, Complex right)
        {
            right.Real += left;
            return right;
        }

        public static Complex operator +(Complex left, double right)
        {
            left.Real += right;
            return left;
        }
        #endregion

        #region 减法
        public static Complex operator -(Complex left, Complex right)
        {
            left.Real -= right.Real;
            left.Imaginary -= right.Imaginary;
            return left;
        }

        public static Complex operator -(double left, Complex right)
        {
            right.Real = left - right.Real;
            right.Imaginary = -right.Imaginary;
            return right;
        }

        public static Complex operator -(Complex left, double right)
        {
            left.Real -= right;
            return left;
        }
        #endregion

        #region 乘法
        public static Complex operator *(Complex left, Complex right)
        {
            return new Complex
            (
                left.Real * right.Real - left.Imaginary * right.Imaginary,
                left.Real * right.Imaginary + left.Imaginary * right.Real
            );
        }

        public static Complex operator *(double left, Complex right)
        {
            right.Real *= left;
            right.Imaginary *= left;
            return right;
        }

        public static Complex operator *(Complex left, double right)
        {
            left.Real *= right;
            left.Imaginary *= right;
            return left;
        }
        #endregion

        #region 除法
        public static Complex operator /(Complex left, double right)
        {
            left.Real /= right;
            left.Imaginary /= right;
            return left;
        }
        public static Complex operator /(double left, Complex right)
        {
            return left * !right / right.AbsoluteSquare;
        }
        public static Complex operator /(Complex left, Complex right)
        {
            return left * !right / right.AbsoluteSquare;
        }
        #endregion

        #region 共轭
        public static Complex operator !(Complex value)
        {
            value.Imaginary = -value.Imaginary;
            return value;
        }
        #endregion

        public static Complex TwiddleFactor(int power, int length)
        {
            return new Complex(Math.Cos(-2 * Math.PI * power / length), Math.Sin(-2 * Math.PI * power / length));
        }
    }
}
