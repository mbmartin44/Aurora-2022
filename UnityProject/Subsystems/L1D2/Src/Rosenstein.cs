// Description of the Output:
// First column: Number of the iteration
// Second column: Logarithm of the stretching factor
// https://www.pks.mpg.de/tisean/Tisean_3.0.1/index.html

using System;
using System.Linq;
using UnityEngine;
public class Rosenstein
{
    private const string WID_STR = "Estimates the maximal Lyapunov exponent; Rosenstein et al.";
    private const int NMAX = 256;
    private const ulong ULONG_MAX = 0xffffffffUL;
    private const int VER_USR1 = 0x2;
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
    public Rosenstein()
    {
        box = new long[NMAX, NMAX];
        exit = false;
    }

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

    public struct Output
    {
        public double LLE;
        public double SNR;
        public string txt;
        public Color color;
        public bool detection;
    }

    public void SetData1D(double[] seriesI)
    {
        series = seriesI;
        length_ = (ulong)seriesI.Length;
    }

    public void SetData(double[] ts1, double[] ts2, double[] ts3, double[] ts4)
    {
        length_ = (ulong)(ts1.Length + ts2.Length + ts3.Length + ts4.Length);
        series = new double[length_];
        ts1.CopyTo(series, 0);
        ts2.CopyTo(series, ts1.Length);
        ts3.CopyTo(series, ts2.Length);
        ts4.CopyTo(series, ts3.Length);
    }

    private bool Put_in_boxes()
    {
        // Test for correct conditionsfirst
        int del = (int)(delay_ * (dims_ - 1));
        if ((int)length_ - del - steps_ <= 0)
        {
            return false;
        }
        int x, y;

        for (int i = 0; i < NMAX; i++)
            for (int j = 0; j < NMAX; j++)
            {
                {
                    box[i, j] = -1;
                }
            }
        list = new long[(int)length_ - del - steps_];
        for (int i = 0; i < (int)length_ - del - steps_; i++)
        {
            x = (int)((int)(series[i] * epsinv) & nmax_);
            y = (int)((int)(series[i + del] * epsinv) & nmax_);
            list[i] = box[x, y];
            box[x, y] = i;
        }
        return true;
    }

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
    /// ***  You must AddData() before calling this function!
    /// </summary>
    /// <returns></returns>
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
