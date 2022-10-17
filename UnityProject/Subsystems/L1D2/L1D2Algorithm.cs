using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace L1D2
{
    public class L1D2Algorithm : MonoBehaviour
    {
        // Private constants
        private const double IOTA = 10e-15;
        private const int MAX_LN_R = 12;
        private const int MIN_LN_R = -12;
        private const int N_LN_R = 600;
        private const string testFolderPath = @"..\..\TSData\";
        private const string testOutFolder = @"TestOutputs\";
        // Private variables
        private int gNTests = 0, gMaxDivergeT = 0;
        private unsafe double* gData, gNDivergence;
        private unsafe double** gDivergence, gCSum;

        // Test & File Parameters - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        public unsafe Test[] gTest;

        /// <summary>
        /// Test parameters
        /// filename
        /// startIndex, stopIndex
        /// seriesN, m, J, W, divergeT
        /// </summary>
        public unsafe struct Test
        {
            public string fileName;
            public long startIndex, stopIndex;
            public int seriesN, m, J, W, divergeT;
        }
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        #region ************************* Memory Allocation / Deallocation ***************************************************
        /// <summary>
        /// Allocate memory for an nRows x nCols matrix
        /// </summary>
        /// <param name="nRows"></param>
        /// <param name="nCols"></param>
        /// <returns></returns>
        public unsafe double** AllocateDMatrix(long nRows, long nCols)
        {
            long i;
            double** mat;

            // Allocate space for the row pointers.
            mat = (double**)Marshal.AllocHGlobal((int)(nRows * sizeof(double*)));
            // Verify sufficient memory.
            if (mat == null)
            {
                Debug.Log("OUT OF MEMORY: AllocateDMatrix(%ld, %ld)\n\n");
                throw new InsufficientMemoryException();
            }

            // allocate space for each individual row pointer.
            for (i = 0; i < nRows; i++)
            {
                mat[i] = (double*)Marshal.AllocHGlobal((IntPtr)(nCols * sizeof(double)));
            }
            // Verify sufficient memory for each row.
            if (mat[i - 1] == null)
            {
                Debug.Log("OUT OF MEMORY: AllocateDMatrix(%ld, %ld)\n\n");
                throw new InsufficientMemoryException();
            };

            return mat;

        }

        /// <summary>
        /// Free up the D-dimensional matrix from memory
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="nRows"></param>
        public unsafe void FreeDMatrix(double** mat, long nRows)
        {
            long i;

            // free space for each row pointer
            for (i = nRows - 1; i >= 0; i--)
                Marshal.FreeHGlobal((IntPtr)mat[i]);

            // free space for the row pointers
            Marshal.FreeHGlobal((IntPtr)mat);
        }

        #endregion

        #region ************************* Linear Fitting Methods *****************************************************

        /// <summary>
        /// Compute the slope of the linear-fitting line
        /// </summary>
        public unsafe void ComputeSlopes()
        {
            int i, i2, j;
            double k, m, b, rr;
            double* data;

            data = N_LN_R > gMaxDivergeT ? (double*)Marshal.AllocHGlobal(N_LN_R * sizeof(double))
                    : (double*)Marshal.AllocHGlobal(gMaxDivergeT * sizeof(double));

            if (data == null)
            {
                Debug.Log("OUT OF MEMORY: ComputeSlopes\n\n");
                throw new InsufficientMemoryException();
            };

            for (i = 0; i < gNTests; i++)
            {
                i2 = i + gNTests;

                // *** work on correlation dimension first

                k = (double)N_LN_R / (MAX_LN_R - MIN_LN_R);

                // pack the array column into the local data array
                for (j = 0; j < N_LN_R; j++)
                {
                    data[j] = gCSum[j][i];
                }
                // compute the 7-point slopes
                for (j = 3; j < N_LN_R - 3; j++)
                {
                    LineFit(data + j - 3, 7, &m, &b, &rr);
                    gCSum[j][i2] = k * m;
                };
                // handle the edges
                LineFit(data, 5, &m, &b, &rr); gCSum[2][i2] = k * m;
                LineFit(data + N_LN_R - 5, 5, &m, &b, &rr); gCSum[N_LN_R - 3][i2] = k * m;
                LineFit(data, 3, &m, &b, &rr); gCSum[1][i2] = k * m;
                LineFit(data + N_LN_R - 3, 3, &m, &b, &rr); gCSum[N_LN_R - 2][i2] = k * m;
                gCSum[0][i2] = k * (data[1] - data[0]);
                gCSum[N_LN_R - 1][i2] = k * (data[N_LN_R - 1] - data[N_LN_R - 2]);

                // ******************** now work on divergence data ***************

                // pack the array column into the local data array.
                for (j = 0; j < gMaxDivergeT; j++)
                {
                    data[j] = gDivergence[j][i];
                }

                // compute the 7-point slopes
                for (j = 3; j < gMaxDivergeT - 3; j++)
                {
                    LineFit(data + j - 3, 7, &m, &b, &rr);
                    gDivergence[j][i2] = m;
                };
                // handle the edges
                LineFit(data, 5, &m, &b, &rr); gDivergence[2][i2] = m;
                LineFit(data + gMaxDivergeT - 5, 5, &m, &b, &rr);
                gDivergence[gMaxDivergeT - 3][i2] = m;
                LineFit(data, 3, &m, &b, &rr);
                gDivergence[1][i2] = m;
                LineFit(data + gMaxDivergeT - 3, 3, &m, &b, &rr);
                gDivergence[gMaxDivergeT - 2][i2] = m;
                gDivergence[0][i2] = data[1] - data[0];
                gDivergence[gMaxDivergeT - 1][i2] = data[gMaxDivergeT - 1] -
                    data[gMaxDivergeT - 2];
            };
        }

        /// <summary>
        /// Linearly fit the data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <param name="b"></param>
        /// <param name="rr"></param>
        public unsafe void LineFit(double* data, double n, double* m, double* b, double* rr)
        {
            int i;
            double sx, sy, sxy, sx2, sy2;
            double x, y, k, mTemp, bTemp, rrTemp;

            sx = sy = sxy = sx2 = sy2 = 0;
            for (i = 0; i < n; i++)
            {
                x = i;
                y = data[i];
                sx += x; sy += y;
                sx2 += x * x; sy2 += y * y;
                sxy += x * y;
            };
            k = n * sx2 - sx * sx;
            mTemp = (n * sxy - sx * sy) / k;
            bTemp = (sx2 * sy - sx * sxy) / k;
            k = sy * sy / n;
            if (k == sy2)
                rrTemp = 1.0;
            else
            {
                rrTemp = (bTemp * sy + mTemp * sxy - k) / (sy2 - k);
                rrTemp = 1.0 - (1.0 - rrTemp) * (n - 1.0) / (n - 2.0);
            };
            *m = mTemp;
            *b = bTemp;
            *rr = rrTemp;
        }

        #endregion

        #region ************************* Testing Methods *******************************************************************
        public unsafe int MainTest()
        {
            int i;
            char[] str = new char[256];
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

            GenerateTemplateFile();

            // *************** TEST FILE IN *****************************************************
            Console.WriteLine("Enter test file name: ");
            String fName = @"..\..\TSData\lorenz.l1d2";// + Console.ReadLine();
            char* fNameIn = (char*)Marshal.StringToHGlobalAnsi(fName);
            ReadTestFile(fName);

            // ******************* OUT FILE **************************************************
            Console.WriteLine("\nEnter output file root (no extension): ");
            String fileOutName = Console.ReadLine();
            char* fNameOut = (char*)Marshal.StringToHGlobalAnsi(fileOutName);

            /* allocate the divergence and correlation sum arrays */
            for (i = 0; i < gNTests; i++)
            {
                if (gTest[i].divergeT > gMaxDivergeT)
                {
                    gMaxDivergeT = 1 + gTest[i].divergeT;
                }
            }
            gNDivergence = (double*)Marshal.AllocHGlobal(gMaxDivergeT * sizeof(double));
            gDivergence = AllocateDMatrix(gMaxDivergeT, 2 * gNTests);
            gCSum = AllocateDMatrix(N_LN_R, 2 * gNTests);

            for (i = 0; i < gNTests; i++)
            {
                ProcessTest(i);
            }

            ComputeSlopes();

            SaveL1Results(fNameOut);
            SaveD2Results(fNameOut);

            Console.WriteLine("\nSuccess!\n");
            return 0;
        }

        // static (class private) variable
        private int last = 100;
        /// <summary>
        /// Calculate percent done???
        /// </summary>
        /// <param name="percentDone"></param>
        public void PercentDone(int percentDone)        // TODO: NEEDS WORK!!!!!!!!!!!!!!!!!!!!!
        {
            if (percentDone < last)
            {
                last = 0;
                Console.WriteLine("0");
            }
            else if (percentDone > last && percentDone % 2 == 0)
            {
                last = percentDone;
                if (percentDone % 10 == 0)
                    Console.WriteLine("{0}", percentDone / 10);
                else
                    Console.WriteLine(".");
            };
        }

        /// <summary>
        /// Process the test data??
        /// </summary>
        /// <param name="testN"></param>
        public unsafe void ProcessTest(int testN)
        {
            long m = 0, J = 0, W = 0, divergeT = 0, neighborIndex = 0, maxIndex = 0;
            long i, j, k, T, CSumIndex, percentDone;
            long nPts, nCompletedPairs = 0, nVectors;
            char* isNeighbor;
            double distance, d;
            double k1, k2, temp, kNorm;

            m = gTest[testN].m;
            J = gTest[testN].J;
            W = gTest[testN].W;
            divergeT = gTest[testN].divergeT;
            nPts = GetData(gTest[testN].fileName, gTest[testN].seriesN,
                gTest[testN].startIndex, gTest[testN].stopIndex);

            k1 = (double)N_LN_R / (MAX_LN_R - MIN_LN_R);
            k1 *= 0.5; // accounts for the SQUARED distances later on
            k2 = N_LN_R / 2;

            nVectors = nPts - J * (m - 1);

            isNeighbor = (char*)Marshal.AllocHGlobal((int)(nVectors * sizeof(char)));
            if (isNeighbor == null)
            {
                Debug.Log("\nOUT OF MEMORY: ProcessTest\n\n");
                throw new OutOfMemoryException();
            };

            // clear the divergence arrays
            for (i = 0; i < gMaxDivergeT; i++)
                gNDivergence[i] = gDivergence[i][testN] = 0;

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
                                temp = gData[i + T] - gData[j + T];
                                temp *= temp;
                                d += temp;
                            };
                            d += IOTA;

                            // map the squared distance to array position
                            CSumIndex = (long)(k1 * Math.Log(d) + k2);
                            if (CSumIndex < 0)
                                CSumIndex = 0;
                            if (CSumIndex >= N_LN_R)
                                CSumIndex = N_LN_R - 1;

                            // increment the correlation sum array
                            gCSum[CSumIndex][testN]++;

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
                                temp = gData[i + T] - gData[j + T];
                                temp *= temp;
                                d += temp;
                            };
                            d += IOTA;

                            CSumIndex = (long)(k1 * Math.Log(d) + k2);
                            if (CSumIndex < 0)
                                CSumIndex = 0;
                            if (CSumIndex >= N_LN_R)
                                CSumIndex = N_LN_R - 1;

                            gCSum[CSumIndex][testN]++;

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
                                temp = gData[i + T] - gData[neighborIndex + T];
                                temp *= temp;
                                d += temp;
                            };
                            d += IOTA;
                            gNDivergence[j]++;
                            temp = 0.5 * Math.Log(d);
                            gDivergence[j][testN] += temp;
                        };
                    };
                    nCompletedPairs++;
                };
                i++;
            };

            // integrate the correlation sum array to get the correlation sum curve
            for (i = 1; i < N_LN_R; i++)
                gCSum[i][testN] += gCSum[i - 1][testN];

            // next normalize values
            kNorm = 1.0 / gCSum[N_LN_R - 1][testN];
            for (i = 0; i < N_LN_R; i++)
                gCSum[i][testN] *= kNorm;

            // now take natural log of the values
            for (i = 0; i < N_LN_R; i++)
            {
                temp = gCSum[i][testN];
                if ((temp < 0.000045) || (temp > 0.990050))
                    gCSum[i][testN] = 0;
                else
                    gCSum[i][testN] = Math.Log(temp);
            };

            // now take care of Lyapunovv average
            for (i = 0; i <= divergeT; i++)
                if (gNDivergence[i] > 0)
                    gDivergence[i][testN] /= gNDivergence[i];

            Marshal.FreeHGlobal((IntPtr)isNeighbor);
            Marshal.FreeHGlobal((IntPtr)gData);
        }

        #endregion

        #region ************************* File Methods (NEEDS REVIEW ***********************************************

        /// <summary>
        /// Generate output file template
        /// </summary>
        static public unsafe void GenerateTemplateFile()
        {
            using (StreamWriter writetext = new StreamWriter("sampleCS.l1d2"))
            {
                writetext.WriteLine("writing in text file");
                writetext.WriteLine("* Header info starts with an asterisk.\n*\n");
                writetext.WriteLine("* Each line of the test file contains the name of a data file followed\n");
                writetext.WriteLine("* by the parameters for the test:\n");
                writetext.WriteLine("*   file_name series# startIndex stopIndex m J W divergeT\n*\n");
                writetext.WriteLine("*      file_name  = name of the data file\n");
                writetext.WriteLine("*      series#    = time series number to use for delay reconstruction\n");
                writetext.WriteLine("*      startIndex = index of first data point to read (usually 1)\n");
                writetext.WriteLine("*      stopIndex  = index of last data point to read\n");
                writetext.WriteLine("*                      (enter 0 for maximum)\n");
                writetext.WriteLine("*      m          = embedding dimension\n");
                writetext.WriteLine("*      J          = delay in samples\n");
                writetext.WriteLine("*      W          = window size for skipping temporally close nearest neighbors\n");
                writetext.WriteLine("*      divergT    = total divergence time in samples\n");
                writetext.WriteLine("*   example: lorenz.dat 1 1 0 5 7 100 300\n");
            }
        }

        /// <summary>
        /// Import data from a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="tsN"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public unsafe long GetData(string fileName, int tsN, long start, long stop)
        {
            long i = 0, len = 0;
            long nCols = 0, nRows = 0, nPts = 0;

            using (var stream = File.OpenRead(testFolderPath + fileName))
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string temp = reader.ReadLine();
                    var colCheck = temp.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    int tempIndex = 0;
                    int numFileRows = 0, numFileCols = 0;
                    numFileCols = colCheck.Length;
                    while (temp != null)
                    {
                        numFileRows++;
                        var dataParts = temp.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int w = 0; w < dataParts.Length; ++w)
                        {
                            tempIndex++;
                        }
                        temp = reader.ReadLine();
                    }
                    gData = (double*)Marshal.AllocHGlobal(tempIndex * sizeof(double));
                    // Reset stream
                    if (stream.CanSeek)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                    }

                    // Now add the data

                    string dataPt = reader.ReadLine();
                    int dataIndex = 0;
                    while (dataPt != null)
                    {
                        var dataParts = dataPt.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int w = 0; w < dataParts.Length; ++w)
                        {
                            gData[dataIndex] = Double.Parse(dataParts[w]);
                            dataIndex++;
                        }
                        dataPt = reader.ReadLine();
                    }
                    return tempIndex;
                }
            }
            return nPts;
        }

        /// <summary>
        /// Read test data from a file
        /// </summary>
        /// <param name="fileName"></param>
        public unsafe void ReadTestFile(string fileName)
        {
            int i = 0;
            int len = 0;
            long endOfHeader = 0;
            int nHead = 0, nRows = 0;
            char[] cStr = new char[1024];
            Console.WriteLine("Reading Test File...");

            using (var stream = File.OpenRead(fileName))
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    // Allocate buffer for reading max of 1024 chars from file
                    char[] buffer = new char[1024];
                    // Skip the header
                    string line = reader.ReadLine();
                    //buffer = line.ToCharArray();
                    //// Determine the numRows of the file, assuming at least one.
                    //nRows = 1;
                    //while (reader.ReadLine() != null)
                    //{
                    //    nRows++;
                    //}
                    //gNTests = nRows;
                    gNTests = 2;
                    // Allocate the test array.
                    gTest = new Test[gNTests];

                    // Verify successful memory alloc.
                    if (gTest == null)
                    {
                        Debug.Log("OUT OF MEMORY: ReadTestFile(" + gNTests.ToString() + ")");
                        throw new InsufficientMemoryException();
                    }
                    Console.WriteLine("detected {0} {1}\n", gNTests, gNTests == 1 ? "test" : "tests");

                    for (i = 0; i < gNTests; i++)
                    {

                        var parts = reader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        gTest[i].fileName = parts[0];
                        gTest[i].seriesN = Int32.Parse(parts[1]);
                        gTest[i].startIndex = long.Parse(parts[2]);
                        gTest[i].stopIndex = long.Parse(parts[3]);
                        gTest[i].m = Int32.Parse(parts[4]);
                        gTest[i].J = Int32.Parse(parts[5]);
                        gTest[i].W = Int32.Parse(parts[6]);
                        gTest[i].divergeT = Int32.Parse(parts[7]);
                    }
                } // !StreamReader
            } // !File
        }

        /// <summary>
        /// Save D2 results to file
        /// </summary>
        /// <param name="fileRoot"></param>
        public unsafe void SaveD2Results(char* fileRoot)
        {
            int i = 0, i1 = 0, i2 = 0, testN = 0, keepGoing = 0;
            char[] str = new char[256];
            double k1 = 0.0, k2 = 0.0;

            Console.WriteLine("Saving data for correlation dimension...");

            string file = Marshal.PtrToStringAnsi((IntPtr)fileRoot);

            // Open the file for writing
            using (StreamWriter writetext = new StreamWriter(testFolderPath + testOutFolder + file + ".d2"))
            {
                k1 = (double)(MAX_LN_R - MIN_LN_R) / N_LN_R;
                k2 = MIN_LN_R;
                // Don't save rows of just zeros
                keepGoing = 1;
                i1 = 0;
                while (keepGoing != 0)
                {
                    for (testN = 0; testN < gNTests; testN++)
                    {
                        if (gCSum[i1][testN] != 0)
                        {
                            keepGoing = 0;
                            break;
                        };
                    }
                    i1 += keepGoing;
                }
                i1--;
                if (i1 < 0 || i1 >= N_LN_R)
                {
                    i1 = 0;
                }
                keepGoing = 1;
                i2 = N_LN_R - 1;
                while (keepGoing != 0)
                {
                    for (testN = 0; testN < gNTests; testN++)
                    {
                        if (gCSum[i2][testN] != 0)
                        {
                            keepGoing = 0;
                            break;
                        }

                        i2 -= keepGoing;
                    }
                }
                i2++;
                if (i2 < 0 || i2 >= N_LN_R)
                {
                    i2 = N_LN_R - 1;
                }
                // write the data
                for (i = i1; i < i2 + 1; i++)
                {
                    writetext.WriteLine(k1 * i + k2);

                    for (testN = 0; testN < gNTests; testN++)
                    {
                        writetext.WriteLine(gCSum[i][testN] + "\t");
                    }

                    // write slope data
                    writetext.WriteLine(k1 * i + k2 + "\t");

                    for (; testN < 2 * gNTests - 1; testN++)
                    {
                        writetext.WriteLine(gCSum[i][testN] + "\t");
                    }
                    writetext.WriteLine(gCSum[i][testN] + "\n");
                }
            }
        }

        /// <summary>
        /// Save L1 results to file
        /// </summary>
        /// <param name="fileRoot"></param>
        public unsafe void SaveL1Results(char* fileRoot)
        {
            int i = 0, testN = 0;
            char[] str = new char[256];
            string file = Marshal.PtrToStringAnsi((IntPtr)fileRoot);

            // Open the file for writing
            using (StreamWriter writetext = new StreamWriter(testFolderPath + testOutFolder + file + ".l1"))
            {
                Console.WriteLine("\nSaving data for largest Lyapunov exponent...\n");
                for (i = 0; i < gMaxDivergeT; i++)
                {
                    writetext.WriteLine(i + "\t");
                    for (testN = 0; testN < gNTests; testN++)
                    {
                        if (i <= gTest[testN].divergeT)
                        {
                            writetext.WriteLine(gDivergence[i][testN] + "\t");
                        }
                        else
                        {
                            writetext.WriteLine("\t");
                        }
                    }
                    // write slope data
                    writetext.WriteLine(i + "\t");

                    for (; testN < 2 * gNTests - 1; testN++)
                    {
                        if (i <= gTest[testN - gNTests].divergeT)
                        {
                            writetext.WriteLine(gDivergence[i][testN] + "\t");
                        }
                        else
                        {
                            writetext.WriteLine("\t");
                        }
                    }
                    if (i <= gTest[testN - gNTests].divergeT)
                    {
                        writetext.WriteLine(gDivergence[i][testN] + "\n");
                    }
                    else
                    {
                        writetext.WriteLine("\n");
                    }
                }
            }
        }
        #endregion //!FILE METHODS REGION*
    }
}

