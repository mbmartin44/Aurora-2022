///-------------------------------------------------------------------------------------------
/// <file>    ExtensionMethods.cs     </file>
/// <author>  Blake Martin            </author>
/// <date>    Last Edited: 12/03/2022 </date>
///-------------------------------------------------------------------------------------------
/// <summary> Contains extension methods for the L1D2 subsystem. </summary>
///-------------------------------------------------------------------------------------------
/// <remarks>
///     This implementation is based on the original C code written by Rosenstein
///     and published in the book "An Introduction to Chaotic Dynamical Systems" by Strogatz.
///     This implementation is based on the C code found in the Tisean package:
///     https://www.pks.mpg.de/tisean/Tisean_3.0.1/index.html
/// </remarks>
/// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Filtering;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.Statistics;

/// <summary>
/// This class contains extension methods used in the L1D2 library.
/// </summary>
public static class ExtensionMethods
{

    #region Subcollection Extension Methods *********************************************************************************

    /// <summary>
    /// This function returns a subcollection of the specified collection.
    /// </summary>
    public static T[] SubArray<T>(this T[] data, int minIndex, int maxIndex)
    {
        int subArrLength = maxIndex - minIndex;
        T[] result = new T[subArrLength];
        Array.Copy(data, minIndex, result, 0, maxIndex);
        return result;
    }

    /// <summary>
    /// Returns the rest of the array after the given index.
    /// </summary>
    /// <param name="data">The array to get the rest of.</param>
    /// <param name="startIndex">The index to start from.</param>
    /// <returns>The rest of the array after the given index.</returns>
    public static T[] RestOfArray<T>(this T[] data, int startIndex)
    {
        int subArrLength = data.Length - startIndex;
        T[] result = new T[subArrLength];
        Array.Copy(data, startIndex, result, 0, subArrLength);
        return result;
    }

    /// <summary>
    /// Returns the subarray of data up to stopIndex (inclusive), or the whole array if stopIndex is greater than the array length.
    /// </summary>
    /// <typeparam name="T">The type of the array</typeparam>
    /// <param name="data">The array to get the subarray from</param>
    /// <param name="stopIndex">The index to stop at, inclusive</param>
    /// <returns>The subarray of data up to stopIndex (inclusive), or the whole array if stopIndex is greater than the array length.</returns>
    public static T[] BoundedLengthSubarray<T>(this T[] data, int stopIndex)
    {
        if (stopIndex != 0)
        {
            T[] result = new T[stopIndex];
            Array.Copy(data, 0, result, 0, stopIndex);
            return result;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the first n elements of a list.
    /// </summary>
    /// <typeparam name="T">Type of list</typeparam>
    /// <param name="data">List to be evaluated</param>
    /// <param name="index">Number of elements to return</param>
    /// <returns></returns>
    public static List<T> BoundedLengthSublist<T>(this List<T> data, int index)
    {
        if (index != 0)
        {
            T[] result = new T[index];
            Array.Copy(data.ToArray(), 0, result, 0, index);
            return result.ToList();
        }
        else
        {
            return null;
        }
    }
    #endregion*************************************

    #region Digital Signal Processing ExtensionMethods ***********************************************************************************************************

    private const int brainbitSamplingRate = 250;
    private const int defaultCutoffFreq = 10;


    /// <summary>
    /// Apply a low pass filter to a double array
    /// </summary>
    /// <param name="data">The double array to filter</param>
    /// <param name="FIR">Whether to use a FIR filter (true) or an IIR filter (false)</param>
    /// <param name="samplingRate">The sampling rate of the data</param>
    /// <param name="cutoff">The cutoff frequency of the filter</param>
    /// <returns>The filtered data</returns>
    public static double[] ApplyLPF(this double[] data, bool FIR = true, int samplingRate = brainbitSamplingRate, double cutoff = defaultCutoffFreq)
    {
        // Verify that the cutoff frequency is valid
        if (samplingRate <= 0 || cutoff <= 0 || data == null)
        {
            throw new InvalidParameterException();
        }
        // Create the filter
        return FIR ? OnlineFilter.CreateLowpass(ImpulseResponse.Finite, samplingRate, cutoff).ProcessSamples(data)
            : OnlineFilter.CreateLowpass(ImpulseResponse.Infinite, samplingRate, cutoff).ProcessSamples(data);
    }

    /// <summary>
    /// This function takes in an array of doubles and an order, and returns the denoised array
    /// </summary>
    /// <param name="data">The array to be denoised</param>
    /// <param name="order">The order of the denoising filter</param>
    /// <returns>The denoised array</returns>
    public static double[] Denoise(this double[] data, int order)
    {
        // Verify that the cutoff frequency is valid
        if (order <= 0 || data == null)
        {
            throw new InvalidParameterException();
        }
        // Create the filter
        return OnlineFilter.CreateDenoise(order).ProcessSamples(data);
    }

    /// <summary>
    /// Gets the real FFT of a given double array of data
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>double array of FFT'd data</returns>
    public static double[] GetRealFFT(this double[] data)
    {
        // Verify that the data is valid
        if (data == null)
        {
            throw new InvalidParameterException();
        }
        // Get the FFT
        double[] fft = new double[data.Length];
        Fourier.Forward(data, fft, FourierOptions.NoScaling);
        return fft;
    }

    /// <summary>
    /// Gets the real FFTs of the matrix data
    /// </summary>
    /// <param name="data">The data to get the FFTs of</param>
    /// <returns>The real FFTs of the data</returns>
    public static double[][] GetRealFFTs(this double[][] data)
    {

        for (int i = 0; i < data.Length; ++i)
        {
            // Window the individual channel
            double[] window = Window.Hann(data[i].Length);
            for (int j = 0; j < data[i].Length; ++j)
            {
                data[i][j] = data[i][j] * window[j];
            }
            // Apply the FFT to the individual channel
            Fourier.ForwardReal(data[i], data[i].Length, FourierOptions.Matlab);
        }
        return data;
    }

    /// <summary>
    /// Gets the autocorrelation of the data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>The autocorrelation of the data.</returns>
    public static double[] GetAutoCorrelation(this double[] data)
    {
        return Correlation.Auto(data);
    }

    /// <summary>
    /// Gets the power spectral density of a signal.
    /// </summary>
    /// <param name="data">The signal to be analyzed.</param>
    /// <param name="isTimeSeries">Whether the signal is a time series or not.</param>
    /// <param name="Fs">The sampling rate of the signal.</param>
    /// <returns>The power spectral density of the signal.</returns>
    public static double[] GetPSD(this double[] data, bool isTimeSeries, int Fs = brainbitSamplingRate)
    {
        double[] psdx = data.SquareArray().MultArrayByConsts((1 / (Fs * data.Length)));
        return psdx;
    }

    /// <summary>
    /// Calculates the signal to noise ratio (SNR) of a signal.
    /// </summary>
    /// <param name="signal">The signal to calculate the SNR of.</param>
    /// <returns>The SNR of the signal.</returns>
    public static double GetSNR(this double[] signal)
    {
        return (Statistics.Mean(signal) / Statistics.StandardDeviation(signal));
    }

    /// <summary>
    /// Gets the coefficient of variation of a list of doubles.
    /// </summary>
    /// <param name="signal">The signal.</param>
    /// <returns>The coefficient of variation of the signal.</returns>
    public static double GetCV(this double[] signal)
    {
        return (100.0 * (Statistics.StandardDeviation(signal) / Statistics.Mean(signal)));
    }

    #endregion

    #region Simple Array Mathematics

    /// <summary>
    /// Squares the array.
    /// </summary>
    /// <param name="data">Data.</param>
    /// <returns>The array.</returns>
    public static double[] SquareArray(this double[] data)
    {
        for (int i = 0; i < data.Length; ++i)
        {
            data[i] = Math.Pow(data[i], 2);
        }
        return data;
    }

    /// <summary>
    /// This function is used to multiply a double array by a constant.
    /// </summary>
    /// <param name="data">The double array that is to be multiplied by a constant.</param>
    /// <param name="C">The constant that is to be multiplied to the double array.</param>
    /// <returns>The double array multiplied by the constant.</returns>
    public static double[] MultArrayByConsts(this double[] data, double C)
    {
        for (int i = 0; i < data.Length; ++i)
        {
            data[i] *= C;
        }
        return data;

    }

    #endregion


    /// <summary>
    /// Returns the shortest channel length from a dictionary of channels
    /// </summary>
    /// <typeparam name="T">Type of data in the channel</typeparam>
    /// <param name="internalChannelsData">Dictionary of channels to search</param>
    /// <returns>The shortest channel length</returns>
    public static int FindShortestChannel<T>(this Dictionary<string, List<T>> internalChannelsData)
    {
        int min = -int.MaxValue;

        // Loop through the dictionary and add the data to the matrix
        foreach (var channel in internalChannelsData)
        {
            int curr = channel.Value.ToArray().Length;
            if (curr < Math.Abs(min))
            {
                min = curr;
            }
        }
        return min;
    }
}