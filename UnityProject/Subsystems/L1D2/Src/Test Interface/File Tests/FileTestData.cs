using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class FileTestData
{

    // Number of tests of run
    public int gNTests = 0;

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

    /// <summary>
    /// Array of the params for each test to be run
    /// </summary>
    public Test[] gTest;

    #region Test Configuration Methods
    //*******************************************************************************************************************************
    /// <summary>
    /// Constructor that initializes the test of test configurations
    /// </summary>
    /// <param name="numTests"></param>
    public FileTestData(int numTests)
    {
        gNTests = numTests;
        gTest = new Test[numTests];

        // Verify successful memory alloc.
        if (gTest == null)
        {
            Debug.Log("OUT OF MEMORY: ReadTestFile(" + gNTests.ToString() + ")");
            throw new InsufficientMemoryException();
        }

        Console.WriteLine("detected {0} {1}\n", gNTests, gNTests == 1 ? "test" : "tests");
    }

    public void SetTestParams(int testNum, string[] parts)
    {
        gTest[testNum].fileName = parts[0];
        gTest[testNum].seriesN = Int32.Parse(parts[1]);
        gTest[testNum].startIndex = long.Parse(parts[2]);
        gTest[testNum].stopIndex = long.Parse(parts[3]);
        gTest[testNum].m = Int32.Parse(parts[4]);
        gTest[testNum].J = Int32.Parse(parts[5]);
        gTest[testNum].W = Int32.Parse(parts[6]);
        gTest[testNum].divergeT = Int32.Parse(parts[7]);
    }
    //*******************************************************************************************************************************
    #endregion


    #region Accessor Functions
    //*******************************************************************************************************************************
    /// <summary>
    /// Get the current number of tests allocated
    /// </summary>
    /// <returns></returns>
    public int NumTests()
    {
        return gNTests;
    }

    public string FileName(int i)
    {
        return gTest[i].fileName;
    }

    public long StartIndex(int i)
    {
        return gTest[i].startIndex;
    }

    public long StopIndex(int i)
    {
        return gTest[i].startIndex;
    }

    public int DivergenceT(int i)
    {
        return gTest[i].divergeT;
    }

    public int M(int i)
    {
        return gTest[i].m;
    }

    public int J(int i)
    {
        return gTest[i].J;
    }

    public int W(int i)
    {
        return gTest[i].W;
    }

    public int SeriesN(int i)
    {
        return gTest[i].seriesN;
    }


    //*******************************************************************************************************************************
    #endregion
}
