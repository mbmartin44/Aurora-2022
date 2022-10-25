using System;
using System.IO;
using System.Text;

/// <summary>
/// Class for fileIO testing related functionality.
/// </summary>
class FileIO
{
    #region File Directory Information
    // **********************************************************************************************************************
    private const string testFolderPath = @"..\..\TSData\";
    private const string testOutFolder = @"TestOutputs\";
    // **********************************************************************************************************************
    #endregion

    private unsafe FileTestData fileTest_;

    public FileIO(int numTests)
    {
        fileTest_ = new FileTestData(numTests);
    }

    public FileIO()
    {
        fileTest_ = new FileTestData(1);
    }

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

    #region File Input Methods
    //*******************************************************************************************************************************

    /// <summary>
    /// Import data from a file
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="tsN"></param>
    /// <param name="start"></param>
    /// <param name="stop"></param>
    /// <returns></returns>
    public unsafe long GetData(string fileName, int tsN, long start, long stop, ref double[] gData)
    {
        using (var stream = File.OpenRead(testFolderPath + fileName))
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string temp = reader.ReadLine();
                var colCheck = temp.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                int tempIndex = 0;
                int numFileRows = 0, numFileCols = 0;
                numFileCols = colCheck.Length;
                // Determine the number of rows in the data file.
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
                gData = new double[tempIndex];// (double*)Marshal.AllocHGlobal(tempIndex * sizeof(double));
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
                return numFileRows;
            }
        }
    }

    /// <summary>
    /// Read test data from a file
    /// </summary>
    /// <param name="fileName"></param>
    public unsafe void ReadTestFile(string fileName /*, int numTests*/)
    {
        Console.WriteLine("Reading Test File...");

        using (var stream = File.OpenRead(fileName))
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                // Allocate buffer for reading max of 1024 chars from file
                char[] buffer = new char[1024];

                // Skip the header
                string line = reader.ReadLine();

                int numTests = 2;

                // Allocate a new set of tests
                fileTest_ = new FileTestData(numTests);

                // Configure tests from .l1d2 test files ***************************************************************
                // 1. file_name
                // 2. series number
                // 3. startIndex
                // 4. stopIndex
                // 5. m
                // 6. J
                // 8. W -
                // 9. divergeT - Total divergence
                for (int i = 0; i < fileTest_.NumTests(); i++)
                {
                    var parts = reader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    fileTest_.SetTestParams(i, parts);
                }

                //***************************************************************************************************
            } // !StreamReader
        } // !File
    }
    //*******************************************************************************************************************************
    #endregion

    #region File Output Methods
    //*******************************************************************************************************************************
    /// <summary>
    /// Save D2 results to file (GCSum)
    /// </summary>
    /// <param name="fileRoot"></param>
    public unsafe void SaveD2Results(string fileRoot, double[,] gCSum)
    {
        int i = 0, i1 = 0, i2 = 0, testN = 0, keepGoing = 0;
        double k1 = 0.0, k2 = 0.0;

        Console.WriteLine("Saving data for correlation dimension...");

        // Open the file for writing
        using (StreamWriter writetext = new StreamWriter(testFolderPath + testOutFolder + fileRoot + ".d2"))
        {
            k1 = (double)(Constants.MAX_LN_R - Constants.MIN_LN_R) / Constants.N_LN_R;
            k2 = Constants.MIN_LN_R;
            // Don't save rows of just zeros
            keepGoing = 1;
            i1 = 0;
            while (keepGoing != 0)
            {
                for (testN = 0; testN < fileTest_.gNTests; testN++)
                {
                    if (gCSum[i1, testN] != 0)
                    {
                        keepGoing = 0;
                        break;
                    };
                }
                i1 += keepGoing;
            }
            i1--;
            if (i1 < 0 || i1 >= Constants.N_LN_R)
            {
                i1 = 0;
            }
            keepGoing = 1;
            i2 = Constants.N_LN_R - 1;
            while (keepGoing != 0)
            {
                for (testN = 0; testN < fileTest_.gNTests; testN++)
                {
                    if (gCSum[i2, testN] != 0)
                    {
                        keepGoing = 0;
                        break;
                    }

                    i2 -= keepGoing;
                }
            }

            i2++;
            if (i2 < 0 || i2 >= Constants.N_LN_R)
            {
                i2 = Constants.N_LN_R - 1;
            }

            // write the data
            for (i = i1; i < i2 + 1; i++)
            {
                writetext.Write(String.Format("{0:00.00000}", (k1 * i + k2)) + "\t");

                for (testN = 0; testN < fileTest_.gNTests; testN++)
                {
                    writetext.Write(String.Format("{0:00.00000}", gCSum[i, testN]) + "\t");
                }

                // write slope data
                writetext.Write(String.Format("{0:00.00000}", k1 * i + k2) + "\t");

                for (; testN < 2 * fileTest_.gNTests - 1; testN++)
                {
                    writetext.Write(String.Format("{0:00.00000}", gCSum[i, testN]));
                }
                writetext.WriteLine("\t" + String.Format("{0:00.00000}", gCSum[i, testN]));
            }
        }
    }

    /// <summary>
    /// Save L1 results to file
    /// </summary>
    /// <param name="fileRoot"></param>
    public unsafe void SaveL1Results(string fileRoot, double[,] gDivergence, int gMaxDivergeT)
    {

        // Open the file for writing
        using (StreamWriter writetext = new StreamWriter(testFolderPath + testOutFolder + fileRoot + ".l1"))
        {
            Console.WriteLine("\nSaving data for largest Lyapunov exponent");
            for (int i = 0; i < gMaxDivergeT; i++)
            {
                writetext.Write(i + "\t");
                int testN;
                for (testN = 0; testN < fileTest_.gNTests; testN++)
                {
                    if (i <= fileTest_.gTest[testN].divergeT)
                    {
                        writetext.Write(String.Format("{0:00.00000}", gDivergence[i, testN]) + "\t");
                    }
                    else
                    {
                        writetext.Write("\t");
                    }
                }
                // write slope data
                writetext.Write("\t\t" + i + "\t");

                for (; testN < 2 * fileTest_.gNTests - 1; testN++)
                {
                    if (i <= fileTest_.gTest[testN - fileTest_.gNTests].divergeT)
                    {
                        writetext.Write(String.Format("{0:00.00000}", gDivergence[i, testN]) + "\t");
                    }
                    else
                    {
                        writetext.Write("\t");
                    }
                }
                if (i <= fileTest_.gTest[testN - fileTest_.gNTests].divergeT)
                {
                    writetext.WriteLine(String.Format("{0:00.00000}", gDivergence[i, testN]));
                }
                else
                {
                    writetext.WriteLine();
                }
            }
        }
    }
    //*******************************************************************************************************************************
    #endregion


    #region Accessor Methods for interfacing between classes
    // Get the number of tests
    public int NumTests()
    {
        return fileTest_.NumTests();
    }

    public string FileName(int i)
    {
        return fileTest_.FileName(i);
    }

    public long StartIndex(int i)
    {
        return fileTest_.StartIndex(i);
    }

    public long StopIndex(int i)
    {
        return fileTest_.StopIndex(i);
    }


    public int DivergenceT(int i)
    {
        return fileTest_.DivergenceT(i);
    }

    public int M(int i)
    {
        return fileTest_.M(i);
    }

    public int J(int i)
    {
        return fileTest_.J(i);
    }

    public int W(int i)
    {
        return fileTest_.W(i);
    }

    public int SeriesN(int i)
    {
        return fileTest_.SeriesN(i);
    }
    #endregion
}

