using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalSignalProcess
{
    public static class TransformsFunctions
    {
        public static int GetOrder(int input)
        {
            int n, temp = input;
            for (n = 0; temp != 0; n++)
            {
                temp >>= 1;
            }
            temp = 1 << n - 1;
            return temp == input ? n - 1 : n;
        }

        public static int UpperTwoIntegerPower(int input)
        {
            int n, result = input;
            for (n = 0; result != 0; n++)
            {
                result >>= 1;
            }
            result = 1 << (n - 1);
            return result == input ? result : result << 1;
        }

        public static void BinaryReverse<T>(T[] array)
        {
            int order = GetOrder(array.Length);

            int index, bitIndex;
            int indexReverse, lowOperator, highOperator, low, high;

            for (index = 0; index < array.Length; index++)
            {
                indexReverse = index;
                for (bitIndex = 0; bitIndex < order / 2; bitIndex++)
                {
                    lowOperator = 1 << bitIndex;
                    highOperator = 1 << (order - 1 - bitIndex);

                    low = index & lowOperator;
                    high = index & highOperator;

                    if (low != 0)
                    {
                        indexReverse |= highOperator;
                    }
                    else
                    {
                        indexReverse &= ~highOperator;
                    }
                    if (high != 0)
                    {
                        indexReverse |= lowOperator;
                    }
                    else
                    {
                        indexReverse &= ~lowOperator;
                    }
                }

                if (index < indexReverse)
                {
                    T temp = array[index];
                    array[index] = array[indexReverse];
                    array[indexReverse] = temp;
                }
            }
        }

        public static double[] LogarithmTransform(double[] input)
        {
            double[] result = new double[input.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Math.Log(input[i] + 1);
            }
            return result;
        }

        public static double[] LogarithmTransform(Complex[] input)
        {
            double[] result = new double[input.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Math.Log(input[i].Absolute + 1);
            }
            return result;
        }
    }

    public static class FourierTransforms
    {
        public static Complex[] FastFourierTransform(short[] input)
        {
            int length = input.Length;
            int order = TransformsFunctions.GetOrder(length);

            TransformsFunctions.BinaryReverse(input);

            Complex[] output = new Complex[length];
            for (int i = 0; i < length; i++)
            {
                output[i] = new Complex(input[i]);
            }

            for (int level = 1; level <= order; level++)
            {
                int butterflyWidth = 1 << (level - 1);
                int butterflyCount = 1 << (order - level);
                for (int butterflyIndex = 0; butterflyIndex < butterflyCount; butterflyIndex++)
                {
                    for (int buttterflyInsideIndex = 0; buttterflyInsideIndex < butterflyWidth; buttterflyInsideIndex++)
                    {
                        int index = buttterflyInsideIndex + 2 * butterflyWidth * butterflyIndex;
                        Complex temp = output[index + butterflyWidth] * Complex.TwiddleFactor(buttterflyInsideIndex * butterflyCount, output.Length);
                        output[index + butterflyWidth] = output[index] - temp;
                        output[index] += temp;
                    }
                }
            }

            return output;
        }
    }
}
