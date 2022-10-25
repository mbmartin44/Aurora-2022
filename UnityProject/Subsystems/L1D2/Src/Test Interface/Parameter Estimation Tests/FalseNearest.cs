using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Determines the draction of false nearest neighbors.
/// Should be below 30% - 10% depending on the application
/// include "routines/tsa.h"
/// </summary>

public unsafe class FalseNearest
{

    private VRosenstein.AuroraConfig auroraConfig_;
    private bool verbosity = true;
    private const ulong ULONG_MAX = 0xffffffffUL;
    private const int VER_USR1 = 0x2;
    private const string WhatIDO = "Determines the fraction of false nearest neighbors.";
    private ulong length = ULONG_MAX, theiler = 0;
    private uint delay = 1, maxdim = 6, minemb = 1;
    private uint comp = 1, maxemb = 5;
    private double rt = 2.0;
    private double eps0 = 1.0e-5;
    private double aveps, vareps;
    private double varianz;
    private const int BOX = 1024;
    private int ibox = BOX - 1;
    private long[,] box;
    private long[] list;
    private uint[] vcomp, vemb;
    private ulong toolarge;

    // IMPORTED TIME-SERIES MATRIX
    private double[,] series;

    public FalseNearest(double[] exposedTS, int numCols, uint delaySet = 1, bool verbose = true)
    {
        auroraConfig_ = new VRosenstein.AuroraConfig();

        auroraConfig_.numEEGChannels = numCols; // For this test!

        int numRows = exposedTS.Length / auroraConfig_.numEEGChannels;
        length = (ulong)numRows;
        auroraConfig_.numSamples = numRows;
        auroraConfig_.sampFreq = 0; // doesnt matter

        // Fill the multi-dimensional matrix with flattened TS data from file.
        series = new double[numRows, numCols];
        for (int i = 0; i < numRows; ++i)
        {
            for (int j = 0; j < numCols; ++j)
            {
                series[i, j] = exposedTS[i * numCols + j];
            }
        }
        delay = delaySet;
        verbosity = verbose;
    }

    private void ExitText()
    {
        for (int i = 0; i < 4; ++i)
        {
            Console.WriteLine("*****************************************************************************************************");
        }
        Console.WriteLine("\t Press any key to terminate test.");
    }

    void What_i_do()
    {
        for (int i = 0; i < 4; ++i)
        {
            Console.WriteLine("*************************************************************************************************");
        }
        Console.WriteLine("\n\nTEST INTIALIZED...\n\n");
    }

    private void Mmb(uint hdim, uint hemb, double eps)
    {
        ulong i;
        long x, y;
        box = new long[BOX, BOX];
        list = new long[length - (maxemb + 1) * delay];
        for (x = 0; x < BOX; x++)
            for (y = 0; y < BOX; y++)
                box[x, y] = -1;

        for (i = 0; i < length - (maxemb + 1) * delay; i++)
        {
            x = (long)(series[i, 0] / eps) & ibox;
            y = (long)(series[i + hemb, hdim] / eps) & ibox;
            list[i] = box[x, y];
            box[x, y] = (long)i;
        }
    }

    private bool Find_nearest(long n, uint dim, double eps)
    {
        long x, y, x1, x2, y1, i, i1, ic, ie;
        long element, which = -1;
        double dx, maxdx, mindx = 1.1, hfactor, factor;

        ic = vcomp[dim];
        ie = vemb[dim];
        x = (long)(series[n, 0] / eps) & ibox;
        y = (long)(series[n + ie, ic] / eps) & ibox;

        for (x1 = x - 1; x1 <= x + 1; x1++)
        {
            x2 = x1 & ibox;
            for (y1 = y - 1; y1 <= y + 1; y1++)
            {
                element = box[x2, y1 & ibox];
                while (element != -1)
                {
                    if (Math.Abs(element - n) > (long)theiler)
                    {
                        maxdx = Math.Abs(series[n, 0] - series[element, 0]);
                        for (i = 1; i <= dim; i++)
                        {
                            ic = vcomp[i];
                            i1 = vemb[i];
                            dx = Math.Abs(series[n + i1, ic] - series[element + i1, ic]);
                            if (dx > maxdx)
                                maxdx = dx;
                        }
                        if ((maxdx < mindx) && (maxdx > 0.0))
                        {
                            which = element;
                            mindx = maxdx;
                        }
                    }
                    element = (long)list[element];
                }
            }
        }

        if ((which != -1) && (mindx <= eps) && (mindx <= varianz / rt))
        {
            aveps += mindx;
            vareps += mindx * mindx;
            factor = 0.0;
            for (i = 1; i <= comp; i++)
            {
                ic = vcomp[dim + i];
                ie = vemb[dim + i];
                hfactor = Math.Abs(series[n + ie, ic] - series[which + ie, ic]) / mindx;
                if (hfactor > factor)
                    factor = hfactor;
            }
            if (factor > rt)
                toolarge++;
            return true;
        }
        return false;
    }

    public int RunFalseNeighborTest()
    {
        What_i_do();

        double min = 0, inter = 0.0, ind_inter = 0, epsilon = 0, av = 0, ind_var = 0;
        bool[] nearest;
        bool alldone = false;
        long i = 0;
        uint dim = 0, emb = 0;
        ulong donesofar = 0;
        nearest = new bool[length];
        if ((maxemb * delay + 1) >= length)
        {
            Console.WriteLine("Not enough points!\n");
            return 0;
        }

        for (i = 0; i < comp; i++)
        {
            Util.Rescale_data((int)i, series, length, ref min, ref ind_inter);
            Util.Variance((int)i, series, length, ref av, ref ind_var);
            if (i == 0)
            {
                varianz = ind_var;
                inter = ind_inter;
            }
            else
            {
                varianz = (varianz > ind_var) ? ind_var : varianz;
                inter = (inter < ind_inter) ? ind_inter : inter;
            }
        }
        vcomp = new uint[maxdim];
        vemb = new uint[maxdim];
        for (i = 0; i < maxdim; i++)
        {
            if (comp == 1)
            {
                vcomp[i] = 0;
                vemb[i] = (uint)(i * delay);
            }
            else
            {
                vcomp[i] = (uint)(i % comp);
                vemb[i] = (uint)(i / comp * delay);
            }
        }
        for (emb = minemb; emb <= maxemb; emb++)
        {
            dim = emb * comp - 1;
            epsilon = eps0;
            toolarge = 0;
            alldone = false;
            donesofar = 0;
            aveps = 0.0;
            vareps = 0.0;

            for (i = 0; i < (long)length; i++)
            {
                nearest[i] = false;
            }
            for (int v = 0; v < 3; v++)
            {
                Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::"); ;
            }
            Console.WriteLine();
            Console.WriteLine(">>>>>>>> Running test for dimensionality = {0}\n", dim + 1);

            while (!alldone && (epsilon < 2.0 * varianz / rt))
            {
                alldone = true;
                Mmb(vcomp[dim], vemb[dim], epsilon);
                for (i = 0; i < (long)length - (maxemb * delay); i++)
                {
                    if (!nearest[i])
                    {
                        nearest[i] = Find_nearest(i, dim, epsilon);
                        alldone &= nearest[i];
                        donesofar = nearest[i] == true ? donesofar + 1 : donesofar;
                    }
                }

                if (verbosity)
                {
                    Console.WriteLine("Found {0} up to epsilon= {1}\n", donesofar, epsilon * inter);
                }

                epsilon *= Math.Sqrt(2.0);
                if (donesofar == 0)
                {
                    eps0 = epsilon;
                }
            }
            if (donesofar == 0)
            {
                Console.WriteLine("Not enough points found!\n");
                return 0;
            }

            aveps *= 1 / (double)donesofar;
            vareps *= 1 / (double)donesofar;
            double percentageFalse = ((double)toolarge / (double)donesofar) * 100;
            string result;
            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Results %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            Console.WriteLine("\n*    EMBEDDED DIMENSIONS: {0}", dim + 1);
            Console.WriteLine("*    RECONSTRUCTION DELAY: {0}", delay);
            Console.WriteLine("*    PERCENTAGE OF FALSE NEIGHBORS: {0} %", ((double)toolarge / (double)donesofar) * 100);
            Console.WriteLine("*    AVERAGE SIZE OF NEIGHBORHOOD {0}", aveps * inter);
            Console.WriteLine("*    AVERAGE OF SQUARED SIZE OF NEIGHBORHOOD: {0}", Math.Sqrt(vareps) * inter);
            result = (percentageFalse < 30.0) ? "VALID EMBEDDING CONFIGURATION. " : "X X X X X X X INVALID EMBEDDING CONFIGURATION!! X X X X X X X X ";
            Console.WriteLine("\n\n* CONCLUSION:\n " + result);
            Console.WriteLine("\n%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
        }
        ExitText();
        Console.ReadLine();
        return 0;
    }
}