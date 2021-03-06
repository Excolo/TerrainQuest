﻿using System;

namespace TerrainQuest.Generator.Helpers
{
    /// <summary>
    /// A collection of math helpers methods
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Clamp the value to stay within the given interval.
        /// </summary>
        public static T Clamp<T>(T input, T min, T max) where T : IComparable<T>
        {
            if (input.CompareTo(min) < 0)
                return min;
            if (input.CompareTo(max) > 0)
                return max;

            return input;
        }

        /// <summary>
        /// Check whether a value is a power of two (value == x^2)
        /// </summary>
        /// <param name="value"></param>
        public static bool IsPowerOfTwo(int value)
            => IsPowerOfTwo((long)value);

        /// <summary>
        /// Check whether a value is a power of two (value == x^2)
        /// </summary>
        /// <param name="value"></param>
        public static bool IsPowerOfTwo(long value)
        {
            value = Math.Abs(value);
            return (value & (value - 1)) == 0;
        }

        /// <summary>
        /// Based on the given value, return the next power of two.
        /// </summary>
        public static int NextPowerOfTwo(int value)
        {
            value = Math.Max(value, 1);

            value--;
            value |= (value >> 1);
            value |= (value >> 2);
            value |= (value >> 4);
            value |= (value >> 8);
            value |= (value >> 16);

            return value + 1;
        }

        /// <summary>
        /// Normalize a value from within a given interval [min, max]
        /// to stay within the interval [a, b].
        /// The [a, b] interval defaults to [0, 1]
        /// </summary>
        public static double Normalize(double value, double min, double max, 
            double a, double b)
        {
            return (((b - a) * (value - min)) / (max - min)) + a;
        }

        /// <summary>
        /// Normalize a value from within a given interval [min, max]
        /// to stay within the interval [0, 1].
        /// </summary>
        public static double Normalize(double value, double min, double max)
        {
            return ((value - min) / (max - min));
        }

        /// <summary>
        /// Copy a multidimensional array to an array of different height and width
        /// </summary>
        public static T[,] Copy<T>(T[,] original, int newHeight, int newWidth)
            where T : struct
        {
            var height = original.GetLength(0);
            var width = original.GetLength(1);
            var minHeight = Math.Min(height, newHeight);
            var minWidth = Math.Min(width, newWidth);
            var newArray = new T[newHeight, newWidth];

            for (int i = 0; i < minHeight; ++i)
            {
                Array.Copy(original, i * width, newArray, i * newWidth, minWidth);
            }
            return newArray;
        }
    }
}
