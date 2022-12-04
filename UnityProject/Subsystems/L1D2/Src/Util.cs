///--------------------------------------------------------------------------------------
/// <file>    Util.cs                                      </file>
/// <author>  Blake Martin                                       </author>
/// <date>    Last Edited: 12/03/2022                            </date>
///--------------------------------------------------------------------------------------
/// <summary>
///     These are utility functions for the L1D2 subsystem.
///     These function aid in the operation of the Rosenstein algorithm.
/// </summary>
/// ---------------------------------------------------------------------------------------
/// <remarks>
///     This implementation is based on the original C code written by Rosenstein
///     and published in the book "An Introduction to Chaotic Dynamical Systems" by Strogatz.
///     This implementation is based on the C code found in the Tisean package:
///     https://www.pks.mpg.de/tisean/Tisean_3.0.1/index.html
/// </remarks>
/// -------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// These are utility functions for the L1D2 subsystem.
/// These function aid in the operation of the Rosenstein algorithm.
/// </summary>
class Util
{

    /// <summary>
    /// This code rescales an array of doubles, x, of length l, so that the minimum value is zero and the maximum value is 1.
    /// </summary>
    /// <param name="x">The array of doubles to rescale</param>
    /// <param name="l">The length of the array, x</param>
    /// <param name="min">The minimum value of the array, x</param>
    /// <param name="interval">The maximum value of the array, x</param>
    /// <returns>Rescales the array, x.</returns>
    public static void Rescale_data(ref double[] x, ulong l, ref double min, ref double interval)
    {
        int i;
        //STEP 1: Initialize the minimum and interval to the first element in the array
        min = interval = x[0];
        //STEP 2: Loop through the array to find the minimum and the interval (max - min)
        for (i = 1; i < (int)l; i++)
        {
            if (x[i] < min) min = x[i];
            if (x[i] > interval) interval = x[i];
        }
        interval -= min;
        //STEP 3: If the interval is not 0, normalize the array to the range [0, 1]
        if (interval != 0.0)
        {
            for (i = 0; i < (int)l; i++)
                x[i] = (x[i] - min) / interval;
        }
    }

    /// <summary>
    ///     Rescales the data in the array x by subtracting the minimum value and dividing by the range.
    ///     The new values are stored in the same array.
    /// </summary>
    /// <param name="j">        - The column index in the array x to rescale.</param>
    /// <param name="x">        - The array to rescale.</param>
    /// <param name="l">        - The length of the array x </param>
    /// <param name="min">      - The minimum value in the array x.</param>
    /// <param name="interval"> - The range of the values in the array x </param>
    public static void Rescale_data(int j, double[,] x, ulong l, ref double min, ref double interval)
    {
        // Find the minimum and interval of column j
        // and scale the values of column j to the range [0,1]
        int i;

        min = interval = x[0, j];

        for (i = 1; i < (int)l - 1; i++)
        {
            if (x[i, j] < min) min = x[i, j];
            if (x[i, j] > interval) interval = x[i, j];
        }
        interval -= min;

        if (interval != 0.0)
        {
            for (i = 0; i < (int)l; i++)
                x[i, j] = (x[i, j] - min) / interval;
        }
    }

    /// <summary>
    /// This method calculates the average and the variance of a column of a 2D array
    /// </summary>
    /// <param name="j"></param>
    /// <param name="s"></param>
    /// <param name="l"></param>
    /// <param name="av"></param>
    /// <param name="var"></param>
    public static void Variance(int j, double[,] s, ulong l, ref double av, ref double var)
    {
        ulong i;
        double h;

        av = var = 0.0;
        // calculate the average and variance of a set of data
        for (i = 0; i < l; i++)
        {
            h = s[i, j];
            av += h;
            var += h * h;
        }
        av /= (double)l;
        var = Math.Sqrt(Math.Abs((var) / (double)l - (av) * (av)));
    }
}

