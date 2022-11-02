using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Filtering;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.Statistics;
//using Accord.MachineLearning;
//using Accord.Statistics.Analysis;
public static class ExtensionMethods
{
    #region Subcollection Extension Methods *********************************************************************************


    public static T[] SubArray<T>(this T[] data, int minIndex, int maxIndex)
    {
        int subArrLength = maxIndex - minIndex;
        T[] result = new T[subArrLength];
        Array.Copy(data, minIndex, result, 0, maxIndex);
        return result;
    }


    public static T[] RestOfArray<T>(this T[] data, int startIndex)
    {
        int subArrLength = data.Length - startIndex;
        T[] result = new T[subArrLength];
        Array.Copy(data, startIndex, result, 0, subArrLength);
        return result;
    }


    /// <summary>
    /// Get the subarray of elements 0 - index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="index"></param>
    /// <returns></returns>
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
    /// Get the subarray of elements 0 - index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="index"></param>
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
    /// Apply a lowpass filter to the data.
    /// Custom sampling rate and cutoff are optional.
    /// true - FIR filter
    /// false - IIR filter
    /// </summary>
    /// <param name="data"></param>
    /// <param name="samplingRate"></param>
    /// <param name="cutoff"></param>
    /// <param name="FIR"></param>
    /// <returns></returns>
    public static double[] ApplyLPF(this double[] data, bool FIR = true, int samplingRate = brainbitSamplingRate, double cutoff = defaultCutoffFreq)
    {
        if (samplingRate <= 0 || cutoff <= 0 || data == null)
        {
            throw new InvalidParameterException();
        }

        return FIR ? OnlineFilter.CreateLowpass(ImpulseResponse.Finite, samplingRate, cutoff).ProcessSamples(data)
            : OnlineFilter.CreateLowpass(ImpulseResponse.Infinite, samplingRate, cutoff).ProcessSamples(data);
    }


    public static double[] Denoise(this double[] data, int order)
    {

        return OnlineFilter.CreateDenoise(order).ProcessSamples(data);
    }

    /// <summary>
    /// Apply FFT to 1-D time-series.
    /// ** Currently uses Hanning Window
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static double[] GetRealFFT(this double[] data)
    {
        // Window the individual channel
        double[] window = Window.Hann(data.Length);
        for (int i = 0; i < data.Length; ++i)
        {
            data[i] = data[i] * window[i];
        }
        Fourier.ForwardReal(data, data.Length, FourierOptions.Matlab);
        return data;
    }

    /// <summary>
    /// Apply FFT to multi-dimensional time-series.
    /// ** Currently uses Hanning Window
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
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
    /// Return the autocorrelation of the array
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static double[] GetAutoCorrelation(this double[] data)
    {
        return Correlation.Auto(data);
    }

    public static double[] GetPSD(this double[] data, bool isTimeSeries, int Fs = brainbitSamplingRate)
    {
        double[] psdx = data.SquareArray().MultArrayByConsts((1 / (Fs * data.Length)));
        return psdx;
    }

    /// <summary>
    /// Returns the estimated to ratio of the signal
    /// </summary>
    /// <param name="signal"></param>
    /// <returns></returns>
    public static double GetSNR(this double[] signal)
    {
        return (Statistics.Mean(signal) / Statistics.StandardDeviation(signal));
    }

    /// <summary>
    /// Returns the Coefficient of Variation as a %.
    /// ** Lower value means better data. **
    /// </summary>
    /// <param name="signal"></param>
    /// <returns></returns>
    public static double GetCV(this double[] signal)
    {
        return (100.0 * (Statistics.StandardDeviation(signal) / Statistics.Mean(signal)));
    }

    //public static double[][] PerformICA(this double[][] data)
    //{
    //    Accord.Statistics.Analysis.IndependentComponentAnalysis ica;
    //    double[,] icaDataMat = new double[4, ];
    //    ica.Separate(data);
    //}

    #endregion

    #region Simple Array Mathematics

    public static double[] SquareArray(this double[] data)
    {
        for (int i = 0; i < data.Length; ++i)
        {
            data[i] = Math.Pow(data[i], 2);
        }
        return data;
    }

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
    /// Find the shortest channel length among the 4 channels contained in the dictionary
    /// </summary>
    /// <returns></returns>
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

    //public static double[][] PerformICA(this Dictionary<string, List<double>> data)
    //{
    //    int shortestChannel = data.FindShortestChannel();
    //    Accord.Statistics.Analysis.IndependentComponentAnalysis ica;
    //    double[,] icaDataMat = new double[4, shortestChannel];
    //
    //    for(int i = 0; i < 4; ++i)
    //    {
    //        for(int j = 0; j < shortestChannel)
    //        {
    //
    //        }
    //    }
    //
    //    ica.Separate(data);
    //}

}