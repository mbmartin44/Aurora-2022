///--------------------------------------------------------------------------------------
/// <file>    Rosenstein.cs                                      </file>
/// <author>  Blake Martin                                       </author>
/// <date>    Last Edited: 12/03/2022                            </date>
///--------------------------------------------------------------------------------------
/// <summary>
///     A C# implementation of the Rosenstein algorithm for calculating the
///     local Lyapunov exponent (LLE) of a time series data set.
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
using System.Linq;
using UnityEngine;

/// <summary>
/// A C# implementation of the Rosenstein algorithm for calculating the
/// local Lyapunov exponent (LLE) of a time series data set.
/// </summary>
public class Rosenstein
{
    // Private members for the Rosenstein algorithm
    private const int NMAX = 256;
    private const uint dims_ = 2;
    private const uint delay_ = 1;
    private const uint steps_ = 10;
    private double[] series, lyap;
    private long[,] box;
    private long[] list;
    private uint mindist_ = 0;
    private const uint nmax_ = NMAX - 1;
    private ulong length_ = 5000;
    private long[] found_;
    private const double eps0 = 1e-3;
    private double eps, epsinv;
    private bool exit = false;

    /// <summary>
    /// Constructor for the Rosenstein class.
    /// </summary>
    public Rosenstein()
    {
        box = new long[NMAX, NMAX];
        exit = false;
    }

    /// <summary>
    /// This function is used to clear the data stored in the class.
    /// </summary>
    public void ClearData()
    {
        series = new double[0];
        length_ = 0;
        box = new long[NMAX, NMAX];
        lyap = new double[0];
        list = new long[0];
        found_ = new long[0];
        eps = 0;
        epsinv = 0;
    }

    /// <summary>
    /// A packaged output of the Rosenstein algorithm.
    /// </summary>
    public struct Output
    {
        public double LLE;
        public double SNR;
        public string txt;
        public Color color;
        public bool detection;
    }

    /// <summary>
    /// This function sets the data for the 1D series.
    /// </summary>
    /// <param name="seriesI">Data to be set.</param>
    public void SetData1D(double[] seriesI)
    {
        series = seriesI;
        length_ = (ulong)seriesI.Length;
    }

    /// <summary>
    /// This method computes the nearest neighbors of each point in the series.
    /// This is done by creating a box around each point and storing the index of the nearest
    /// point in that box. The index of the box is computed by multiplying the value of the
    /// time series by the number of boxes and taking the floor of the result. The nearest neighbor
    /// is then computed by searching the box for the nearest point.
    /// </summary>
    /// <returns>
    /// Returns true if the nearest neighbor could be computed for each point.
    /// The method returns false if the number of points in the series is less than
    /// the length of the box plus the number of steps.
    /// </returns>
    private bool Put_in_boxes()
    {

        // Compute the length of the input time series and the size of the box
        int del = (int)(delay_ * (dims_ - 1));
        if ((int)length_ - del - steps_ <= 0)
        {
            return false;
        }

        int x, y;

        // Setup the box
        for (int i = 0; i < NMAX; i++)
            for (int j = 0; j < NMAX; j++)
            {
                {
                    box[i, j] = -1;
                }
            }
        // Create a list to store the results
        list = new long[(int)length_ - del - steps_];
        // Compute the list of nearest neighbors
        for (int i = 0; i < (int)length_ - del - steps_; i++)
        {
            // Compute the index of the box for the current point
            x = (int)((int)(series[i] * epsinv) & nmax_);
            // Compute the index of the box for the point del time steps away
            y = (int)((int)(series[i + del] * epsinv) & nmax_);
            // Store the nearest neighbor for the current points
            list[i] = box[x, y];
            // Update the box with the current point
            box[x, y] = i;
        }
        return true;
    }

    /// <summary>
    /// 1. We are looking for the points in the series that are the closest to the point act ('actual'). We are looking in the box (x, y) and its adjacent boxes.
    /// 2. We also search for the point that is the closest to the point that is act-delay_*(dims_-1) points earlier in the series.
    /// 3. If we find such a point, we calculate the distance to the point act-delay_*(dims_-1) points earlier in the series.
    /// 4. If the distance is less than eps, we remember in the found_ array the number of points that are closer than eps to each point in the series.
    /// 5. We calculate the sum of the logarithms of the distances of the points in the series to the point that is act-delay_*(dims_-1) points earlier in the series.
    /// </summary>
    private bool Make_iterate(long act)
    {
        bool ok = false;
        int x, y, i, j, i1, k, del1 = (int)(dims_ * delay_);
        long element, minelement = -1;
        double dx, mindx = 1.0;
        if (series.Length <= act || series.Length <= act + delay_ * (dims_ - 1))
        {
            exit = true;
            return false;
        }
        x = (int)((int)(series[act] * epsinv) & nmax_);
        y = (int)((int)(series[act + delay_ * (dims_ - 1)] * epsinv) & nmax_);
        for (i = x - 1; i <= x + 1; i++)
        {
            i1 = (int)(i & nmax_);
            for (j = y - 1; j <= y + 1; j++)
            {
                element = box[i1, j & nmax_];
                while (element != -1)
                {
                    if (Math.Abs(act - element) > mindist_)
                    {
                        dx = 0.0;
                        for (k = 0; k < del1; k += (int)delay_)
                        {
                            dx += (series[act + k] - series[element + k]) *
                              (series[act + k] - series[element + k]);
                            if (dx > eps * eps)
                                break;
                        }
                        if (k == del1)
                        {
                            if (dx < mindx)
                            {
                                ok = true;
                                if (dx > 0.0)
                                {
                                    mindx = dx;
                                    minelement = element;
                                }
                            }
                        }
                    }
                    element = list[element];
                }
            }
        }
        if ((minelement != -1))
        {
            act--;
            minelement--;
            for (i = 0; i <= steps_; i++)
            {
                act++;
                minelement++;
                dx = 0.0;
                for (j = 0; j < del1; j += (int)delay_)
                {
                    dx += (series[act + j] - series[minelement + j]) *
                      (series[act + j] - series[minelement + j]);
                }
                if (dx > 0.0)
                {
                    found_[i]++;
                    lyap[i] += Math.Log(dx);
                }
            }
        }
        return ok;
    }

    /// <summary>
    /// Run the Rosenstein Algorithm.
    /// Calculates the local Lyapunov Exponent for the time series.
    /// </summary>
    /// <returns>
    /// Returns the output packet.
    /// </returns>
    /// <remarks>
    /// ***  You must AddData() before calling this function!
    /// </remarks>
    public Output? RunAlgorithm()
    {

        bool[] done;
        bool alldone = false;
        double min = 0, max = 0;

        if (series == null)
        {
            return null;
        }

        Util.Rescale_data(ref series, length_, ref min, ref max);

        lyap = new double[steps_ + 1];
        found_ = new long[steps_ + 1];
        for (int i = 0; i <= steps_; i++)
        {
            lyap[i] = 0.0;
            found_[i] = 0;
        }
        done = new bool[length_];

        for (int i = 0; i < (int)length_; i++)
        {
            done[i] = false;
        }

        long maxlength = (long)(length_ - delay_ * (dims_ - 1) - steps_ - 1 - mindist_);

        for (eps = eps0; !alldone; eps *= 1.1)
        {
            epsinv = 1.0 / eps;
            if (!Put_in_boxes())
            {
                return null;
            }
            alldone = true;
            long n;
            for (n = 0; n <= maxlength; n++)
            {
                if (!done[n])
                {
                    done[n] = Make_iterate(n);
                    if (exit)
                    {
                        exit &= false;
                        return null;
                    }
                }
                alldone &= done[n];
            }
        }
        double[] scaledLLE = new double[steps_ + 1];
        for (int i = 0; i <= steps_; i++)
        {
            scaledLLE[i] = lyap[i] / found_[i] / 2.0;
        }
        Output output = new Output();
        output.LLE = scaledLLE.Max() == double.NaN ? 0 : -scaledLLE.Max();
        output.SNR = series.GetSNR();
        output.detection = output.LLE > 4;
        output.txt = output.detection ? "Seizure Detected!" : "Seizure Not Detected.";
        output.color = output.LLE > 4 ? Color.red : Color.green;
        return output;
    }
}
