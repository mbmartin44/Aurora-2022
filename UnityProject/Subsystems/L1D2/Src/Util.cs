using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Util
{

    public static void Rescale_data(double[] x, ulong l, ref double min, ref double interval)
    {
        int i;

        min = interval = x[0];

        for (i = 1; i < (int)l; i++)
        {
            if (x[i] < min) min = x[i];
            if (x[i] > interval) interval = x[i];
        }
        interval -= min;

        if (interval != 0.0)
        {
            for (i = 0; i < (int)l; i++)
                x[i] = (x[i] - min) / interval;
        }
        else
        {
            Console.WriteLine("rescale_data: data ranges from %e to %e. It makes\n"
                + "\t\tno sense to continue. Exiting!\n\n", min, min + (interval));
        }
    }

    public static void Rescale_data(int j, double[,] x, ulong l, ref double min, ref double interval)
    {
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
        else
        {
            Console.WriteLine("rescale_data: data ranges from %e to %e. It makes\n"
                    + "\t\tno sense to continue. Exiting!\n\n", min, min + (interval));
            return;
        }
    }



    public static void Variance(int j, double[,] s, ulong l, ref double av, ref double var)
    {
        ulong i;
        double h;

        av = var = 0.0;

        for (i = 0; i < l; i++)
        {
            h = s[i, j];
            av += h;
            var += h * h;
        }
        av /= (double)l;
        var = Math.Sqrt(Math.Abs((var) / (double)l - (av) * (av)));
        if (var == 0.0)
        {
            Console.WriteLine("Variance of the data is zero. Exiting!\n\n");
            return;
        }
    }
}

