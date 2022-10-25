using System;
using System.Runtime.InteropServices;
using UnityEngine;

class TRosenstein : VRosenstein
{

    #region Algorithm -> Test Algorithm Extensions

    #region Additional Members + Properties

    private FileIO fileIO_;
    // static (class private) variable
    private int last = 100;

    #endregion

    public TRosenstein(int numTests, int numChannels = 4, int m = 5, int J = 9, int W = 300, int divergeT = 500)
    {
        fileIO_ = new FileIO(numTests);

        Params_ = new Parameters
        {
            startIndex = 0,
            stopIndex = 7, // should be 5 or 7
            m = m,
            J = J,
            W = W,
            divergeT = divergeT

        };
    }

    /// <summary>
    /// Make the TS data from the .dat available to other classes.
    /// </summary>
    /// <returns></returns>
    public double[] ExposeTSData()
    {
        return GData;
    }

    /// <summary>
    /// Override the params with test[i] config
    /// </summary>
    /// <param name="i"></param>
    private void OverrideParamsWithTestConfig(int i)
    {
        Parameters overrideParameters = new Parameters
        {
            startIndex = fileIO_.StartIndex(i),
            stopIndex = fileIO_.StopIndex(i),
            seriesN = fileIO_.SeriesN(i),
            J = fileIO_.J(i),
            m = fileIO_.M(i),
            divergeT = fileIO_.DivergenceT(i),
            W = fileIO_.W(i),
        };

        Params_ = overrideParameters;
    }

    /// <summary>
    /// Calculate percent of test-completion
    /// </summary>
    /// <param name="percentDone"></param>
    public void PercentDone(int percentDone)
    {
        if (percentDone < last)
        {
            last = 0;
            Console.WriteLine("% Complete _________________________");
            Console.Write(" 0" + " %  .d");
        }
        else if (percentDone > last && percentDone % 2 == 0)
        {
            last = percentDone;
            if (percentDone % 10 == 0)
                Console.Write("\n{0} % ", percentDone);
            for(int i = 0; i < percentDone / 10; ++i)
            {
                Console.Write(".");
            }
        };
    }

    /// <summary>
    /// Main file-test frame
    /// </summary>
    /// <returns></returns>
    public unsafe int MainTest()
    {
        Console.WriteLine("**************** AURORA-2022 L1D2 FUNCTIONAL Test*************************************************");
        Console.WriteLine("**************************************************************************************************");
        Console.WriteLine("** Algorithm Description: \n Generates scaling information for L1 and D2 ***");
        Console.WriteLine("\n    * v1.0 Copyright (c) 1999 M.T. Rosenstein");
        Console.WriteLine("             - reference: M.T. Rosenstein, J.J. Collins, C.J. De Luca,");
        Console.WriteLine("             - A practical method for calculating largest");
        Console.WriteLine("                 Lyapunov exponents from small data sets,");
        Console.WriteLine("                 Physica D 65:117-134, 1993.");
        Console.WriteLine(" \n ** Contact information:\n \t Blake Martin\n\t (mbmartin44@tntech.edu)");
        Console.WriteLine("****************************************************************************************************");
        Console.WriteLine("****************************************************************************************************\n\n");

        FileIO.GenerateTemplateFile();

        // *************** TEST FILE IN *****************************************************
        Console.WriteLine("Enter test file name: ");
        String fName = @"..\..\TSData\lorenz.l1d2";// + Console.ReadLine();
        fileIO_.ReadTestFile(fName);

        // ******************* OUT FILE **************************************************
        Console.WriteLine("\nEnter output file root (no extension): ");
        String fileOutName = Console.ReadLine();

        /* allocate the divergence and correlation sum arrays */
        for (int i = 0; i < fileIO_.NumTests(); i++)
        {
            if (fileIO_.DivergenceT(i) > gMaxDivergeT)
            {
                gMaxDivergeT = 1 + fileIO_.DivergenceT(i);
            }
        }
        GNDivergence = new double[gMaxDivergeT];
        GDivergence = new double[gMaxDivergeT, 2 * fileIO_.NumTests()];
        GCSum = new double[Constants.N_LN_R, 2 * fileIO_.NumTests()];

        for (int i = 0; i < fileIO_.NumTests(); i++)
        {
            OverrideParamsWithTestConfig(i);
            ProcessData(i);
        }

        ComputeSlopes();

        fileIO_.SaveL1Results(fileOutName, GDivergence, gMaxDivergeT); // Need to add new params from Algorithm class.
        fileIO_.SaveD2Results(fileOutName, GCSum); // Need to add new params from Algorithm class.

        Console.WriteLine("\nSuccess!\n");
        return 0;
    }

    #endregion

    #region Overridden Abstract Methods (Class-specific Method Implementations)

    /// <summary>
    /// Process the test data??
    /// </summary>
    /// <param name="testN"></param>
    public unsafe override void ProcessData(int testN)
    {
        long m = 0, J = 0, W = 0, divergeT = 0, neighborIndex = 0, maxIndex = 0;
        long i, j, k, T, CSumIndex, percentDone;
        long nPts, nCompletedPairs = 0, nVectors;
        char* isNeighbor;
        double distance, d;
        double k1, k2, temp, kNorm;

        m = fileIO_.M(testN);
        J = fileIO_.J(testN);
        W = fileIO_.W(testN);
        divergeT = fileIO_.DivergenceT(testN);
        double[] allocArr = new double[0];
        nPts = fileIO_.GetData(fileIO_.FileName(testN), fileIO_.SeriesN(testN),
            fileIO_.StartIndex(testN), fileIO_.StopIndex(testN), ref allocArr);

        GData = allocArr;

        k1 = (double)Constants.N_LN_R / (Constants.MAX_LN_R - Constants.MIN_LN_R);
        k1 *= 0.5; // accounts for the SQUARED distances later on
        k2 = Constants.N_LN_R / 2;

        nVectors = nPts - J * (m - 1);

        isNeighbor = (char*)Marshal.AllocHGlobal((int)(nVectors * sizeof(char)));
        if (isNeighbor == null)
        {
            Debug.Log("\nOUT OF MEMORY: ProcessTest\n\n");
            throw new OutOfMemoryException();
        };

        // clear the divergence arrays
        for (i = 0; i < gMaxDivergeT; i++)
            GNDivergence[i] = GDivergence[i, testN] = 0;

        // loop through vectors
        i = 0;
        while (i < nVectors)
        {
            percentDone = (long)(100.0 * nCompletedPairs / nVectors * 2 + 0.5);
            percentDone = (long)(100.0 * i / nVectors + 0.5);
            PercentDone((int)percentDone);

            if (isNeighbor[i] == 0)
            {
                distance = 10e10;

                // find the nearest neighbor for the vector at i
                // ignore temporally close neighbors using W
                if (i > W)
                    for (j = 0; j < i - W; j++)
                    {
                        // calculate distance squared
                        d = 0;
                        for (k = 0; k < m; k++)
                        {
                            T = k * J;
                            temp = GData[i + T] - GData[j + T];
                            temp *= temp;
                            d += temp;
                        };
                        d += Constants.IOTA;

                        // map the squared distance to array position
                        CSumIndex = (long)(k1 * Math.Log(d) + k2);
                        if (CSumIndex < 0)
                            CSumIndex = 0;
                        if (CSumIndex >= Constants.N_LN_R)
                            CSumIndex = Constants.N_LN_R - 1;

                        // increment the correlation sum array
                        GCSum[CSumIndex, testN]++;

                        // now compare to current nearest neighbor
                        // use IOTA above to ignore nbrs that are too close
                        if (d < distance)
                        {
                            distance = d;
                            neighborIndex = j;
                        };
                    };

                if (i < nVectors - W)
                    for (j = i + W; j < nVectors; j++)
                    {
                        d = 0;
                        for (k = 0; k < m; k++)
                        {
                            T = k * J;
                            temp = GData[i + T] - GData[j + T];
                            temp *= temp;
                            d += temp;
                        };
                        d += Constants.IOTA;

                        CSumIndex = (long)(k1 * Math.Log(d) + k2);
                        if (CSumIndex < 0)
                            CSumIndex = 0;
                        if (CSumIndex >= Constants.N_LN_R)
                            CSumIndex = Constants.N_LN_R - 1;

                        GCSum[CSumIndex, testN]++;

                        if (d < distance)
                        {
                            distance = d;
                            neighborIndex = j;
                        };
                    };

                isNeighbor[neighborIndex] = (char)1;

                // track divergence
                for (j = 0; j <= divergeT; j++)
                {
                    maxIndex = nPts - m * J - j - 1;
                    if (i < maxIndex && neighborIndex < maxIndex)
                    {
                        d = 0;
                        for (k = 0; k < m; k++)
                        {
                            T = k * J + j;
                            temp = GData[i + T] - GData[neighborIndex + T];
                            temp *= temp;
                            d += temp;
                        };
                        d += Constants.IOTA;
                        GNDivergence[j]++;
                        temp = 0.5 * Math.Log(d);
                        GDivergence[j, testN] += temp;
                    };
                };
                nCompletedPairs++;
            };
            i++;
        };

        // integrate the correlation sum array to get the correlation sum curve
        for (i = 1; i < Constants.N_LN_R; i++)
            GCSum[i, testN] += GCSum[i - 1, testN];

        // next normalize values
        kNorm = 1.0 / GCSum[Constants.N_LN_R - 1, testN];
        for (i = 0; i < Constants.N_LN_R; i++)
            GCSum[i, testN] *= kNorm;

        // now take natural log of the values
        for (i = 0; i < Constants.N_LN_R; i++)
        {
            temp = GCSum[i, testN];
            if ((temp < 0.000045) || (temp > 0.990050))
                GCSum[i, testN] = 0;
            else
                GCSum[i, testN] = Math.Log(temp);
        };

        // now take care of Lyapunovv average
        for (i = 0; i <= divergeT; i++)
            if (GNDivergence[i] > 0)
                GDivergence[i, testN] /= GNDivergence[i];

        Marshal.FreeHGlobal((IntPtr)isNeighbor);
    }

    /// <summary>
    /// Compute the slope of the linear-fitting line
    /// </summary>
    public override unsafe void ComputeSlopes()
    {
        int i, i2, j;
        double k, m, b, rr;
        double[] data;
        data = Constants.N_LN_R > gMaxDivergeT ? new double[Constants.N_LN_R] : new double[gMaxDivergeT];

        if (data == null)
        {
            Debug.Log("OUT OF MEMORY: ComputeSlopes\n\n");
            throw new InsufficientMemoryException();
        };

        for (i = 0; i < fileIO_.NumTests(); i++)
        {
            i2 = i + fileIO_.NumTests();

            // *** work on correlation dimension first

            k = (double)Constants.N_LN_R / (Constants.MAX_LN_R - Constants.MIN_LN_R);

            // pack the array column into the local data array
            for (j = 0; j < Constants.N_LN_R; j++)
            {
                data[j] = GCSum[j, i];
            }
            // compute the 7-point slopes
            for (j = 3; j < Constants.N_LN_R - 3; j++)
            {
                LineFit(data, j - 3, 7, &m, &b, &rr);
                GCSum[j, i2] = k * m;
            };
            // handle the edges
            LineFit(data, 0, 5, &m, &b, &rr);
            GCSum[2, i2] = k * m;
            LineFit(data, Constants.N_LN_R - 5, 5, &m, &b, &rr);
            GCSum[Constants.N_LN_R - 3, i2] = k * m;
            LineFit(data, 0, 3, &m, &b, &rr);
            GCSum[1, i2] = k * m;
            LineFit(data, Constants.N_LN_R - 3, 3, &m, &b, &rr);
            GCSum[Constants.N_LN_R - 2, i2] = k * m;
            GCSum[0, i2] = k * (data[1] - data[0]);
            GCSum[Constants.N_LN_R - 1, i2] = k * (data[Constants.N_LN_R - 1] - data[Constants.N_LN_R - 2]);

            // ******************** now work on divergence data ***************

            // pack the array column into the local data array.
            for (j = 0; j < gMaxDivergeT; j++)
            {
                data[j] = GDivergence[j, i];
            }

            // compute the 7-point slopes
            for (j = 3; j < gMaxDivergeT - 3; j++)
            {
                LineFit(data, j - 3, 7, &m, &b, &rr);
                GDivergence[j, i2] = m;
            };
            // handle the edges
            LineFit(data, 0, 5, &m, &b, &rr); GDivergence[2, i2] = m;
            LineFit(data, gMaxDivergeT - 5, 5, &m, &b, &rr);
            GDivergence[gMaxDivergeT - 3, i2] = m;
            LineFit(data, 0, 3, &m, &b, &rr);
            GDivergence[1, i2] = m;
            LineFit(data, gMaxDivergeT - 3, 3, &m, &b, &rr);
            GDivergence[gMaxDivergeT - 2, i2] = m;
            GDivergence[0, i2] = data[1] - data[0];
            GDivergence[gMaxDivergeT - 1, i2] = data[gMaxDivergeT - 1] -
                data[gMaxDivergeT - 2];
        };
    }

    #endregion

}