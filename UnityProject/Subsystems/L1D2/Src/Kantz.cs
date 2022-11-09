/*
 * Reference Link: https://www.pks.mpg.de/tisean/Tisean_3.0.1/index.html
 *
 *                 Description of the Output:
 *
 *    For each embedding dimension and each length scale the file contains a block of data consisting of 3 columns
 *    The number of the iteration
 *    The logarithm of the stretching factor (the slope is the Lyapunov exponent if it is a straight line)
 *    The number of points for which a neighborhood with enough points was found
 */


using System;

public class Kantz
{
    private ulong length = 5000;
    private ulong reference = Constants.ULONG_MAX;
    private uint maxdim = 2;
    private uint mindim = 2;
    private uint ibox = Constants.BOX - 1;
    private uint delay = 1;
    private uint epscount = 5;
    private uint maxiter = 50;
    private uint window = 0;
    private uint verbosity = 0xff;
    private double epsmin = 1e-3, epsmax = 1e-2;
    private bool eps0set = false, eps1set = false;

    private long[,] box, lfound, count;
    private long[] liste, found;

    private double[,] lyap;
    private double[] series;
    private double max, min;

    public void SetData1D(double[] ts)
    {
        series = ts;
    }

    public void SetData(double[] ts1, double[] ts2, double[] ts3, double[] ts4)
    {
        series = new double[ts1.Length + ts2.Length + ts3.Length + ts4.Length];
        ts1.CopyTo(series, 0);
        ts2.CopyTo(series, ts1.Length);
        ts3.CopyTo(series, ts2.Length);
        ts4.CopyTo(series, ts3.Length);
    }

    public Kantz()
    {
        box = new long[Constants.BOX, Constants.BOX];
    }

    public Kantz(double[] seriesI)
    {
        SetData1D(seriesI);
    }

    public Kantz(double[] ts1, double[] ts2, double[] ts3, double[] ts4)
    {
        SetData(ts1, ts2, ts3, ts4);
    }

    private ulong blength;
    private void Put_in_boxes(double eps)
    {
        ulong i;
        long j, k;

        blength = length - (maxdim - 1) * delay - maxiter;

        for (i = 0; i < Constants.BOX; i++)
            for (j = 0; j < Constants.BOX; j++)
                box[i, j] = -1;

        for (i = 0; i < blength; i++)
        {
            j = (long)(series[i] / eps) & ibox;
            k = (long)(series[i + delay] / eps) & ibox;
            liste[i] = box[j, k];
            box[j, k] = (long)i;
        }
    }

    private long lwindow;
    private void Lfind_neighbors(long act, double eps)
    {
        uint hi, k, k1;
        long i, j, i1, i2, j1, element;

        double dx, eps2 = Math.Sqrt(eps);

        lwindow = (long)window;
        for (hi = 0; hi < maxdim - 1; hi++)
            found[hi] = 0;
        i = (long)(series[act] / eps) & ibox;
        j = (long)(series[act + delay] / eps) & ibox;
        for (i1 = i - 1; i1 <= i + 1; i1++)
        {
            i2 = i1 & ibox;
            for (j1 = j - 1; j1 <= j + 1; j1++)
            {
                element = box[i2, j1 & ibox];
                while (element != -1)
                {
                    if ((element < (act - lwindow)) || (element > (act + lwindow)))
                    {
                        dx = Math.Sqrt(series[act] - series[element]);
                        if (dx <= eps2)
                        {
                            for (k = 1; k < maxdim; k++)
                            {
                                k1 = k * delay;
                                dx += Math.Sqrt(series[act + k1] - series[element + k1]);
                                if (dx <= eps2)
                                {
                                    k1 = k - 1;
                                    lfound[k1, found[k1]] = element;
                                    found[k1]++;
                                }
                                else
                                    break;
                            }
                        }
                    }
                    element = liste[element];
                }
            }
        }
    }

    private void Iterate_points(long act)
    {
        double[,] lfactor;
        double[] dx;
        uint i, j, l, l1;
        long k, element;
        long[,] lcount;
        lfactor = new double[maxiter+1, maxdim - 1];
        lcount = new long[maxiter, maxiter];
        dx = new double[maxiter + 1];

        for (i = 0; i < maxiter; i++)
            for (j = 0; j < maxdim - 1; j++)
            {
                lfactor[i, j] = 0.0;
                lcount[i, j] = 0;
            }

        for (j = mindim - 2; j < maxdim - 1; j++)
        {
            for (k = 0; k < found[j]; k++)
            {
                element = lfound[j, k];
                for (i = 0; i <= maxiter; i++)
                    dx[i] = Math.Sqrt(series[act + i] - series[element + i]);
                for (l = 1; l < j + 2; l++)
                {
                    l1 = l * delay;
                    for (i = 0; i < maxiter; i++)
                        dx[i] += Math.Sqrt(series[act + i + l1] - series[element + l1 + i]);
                }
                for (i = 0; i < maxiter; i++)
                    if (dx[i] > 0.0)
                    {
                        lcount[i, j]++;
                        lfactor[i, j] += dx[i];
                    }
            }
        }
        for (i = mindim - 2; i < maxdim - 1; i++)
            for (j = 0; j < maxiter; j++)
                if (lcount[i, j] != 0)
                {
                    count[i, j]++;
                    lyap[i, j] += Math.Log(lfactor[i, j] / lcount[i, j]) / 2.0;
                }
    }

    /// <summary>
    /// Run the Kantz Algorithm.
    /// ***  You must AddData() before calling this function!
    /// </summary>
    /// <returns></returns>
    public int RunAlgorithm()
    {
        double eps_fak = 0;
        double epsilon = 0;
        uint i = 0, j = 0, l = 0;

        //series = Util.Get_series(infile, &length, exclude, column, verbosity);
        Util.Rescale_data(ref series, length, ref min, ref max);

        if (eps0set)
        {
            epsmin /= max;
        }
        if (eps1set)
        {
            epsmax /= max;
        }
        if (epsmin >= epsmax)
        {
            epsmax = epsmin;
            epscount = 1;
        }

        if (reference > (length - maxiter - (maxdim - 1) * delay))
        {
            reference = length - maxiter - (maxdim - 1) * delay;
        }
        if ((maxiter + (maxdim - 1) * delay) >= length)
        {
            Console.WriteLine("Too few points to handle these parameters!\n");
            return 0;
        }

        if (maxdim < 2)
            maxdim = 2;
        if (mindim < 2)
            mindim = 2;
        if (mindim > maxdim)
            maxdim = mindim;


        liste = new long[length];
        found = new long[maxdim];
        lfound = new long[maxdim, length];
        count = new long[maxdim, maxiter + 1];
        lyap = new double[maxdim, maxiter + 1];

        if (epscount == 1)
            eps_fak = 1.0;
        else
            eps_fak = Math.Pow(epsmax / epsmin, 1.0 / (double)(epscount - 1));

        for (l = 0; l < epscount; l++)
        {
            epsilon = epsmin * Math.Pow(eps_fak, (double)l);
            for (i = 0; i < maxdim - 1; i++)
                for (j = 0; j <= maxiter; j++)
                {
                    count[i, j] = 0;
                    lyap[i, j] = 0.0;
                }
            Put_in_boxes(epsilon);
            for (i = 0; i < reference; i++)
            {
                Lfind_neighbors(i, epsilon);
                Iterate_points(i);
            }

            if (verbosity != 0)
            {
                Console.WriteLine("epsilon= {0}\n", epsilon * max);
            }
            for (i = mindim - 2; i < maxdim - 1; i++)
            {
                Console.WriteLine("#epsilon= {0}  dim= {1}\n", epsilon * max, i + 2);
                for (j = 0; j <= maxiter; j++)
                {
                    if (count[i, j] != 0)
                    {
                        Console.WriteLine("{0} {1} {2}\n", j, lyap[i, j] / count[i, j], count[i, j]);
                    }
                }
                Console.WriteLine("\n");
            }
        }
        return 0;
    }
}
