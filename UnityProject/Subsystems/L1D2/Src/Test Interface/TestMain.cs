
/// <summary>
/// This is an example of running the file-test for L1D2 and will be
/// updated as the project development continues.
/// This file should not be included when the library is compiled
/// (Should never be derived from Monobehavior)
/// </summary>
using System;
class TestMain
{

    unsafe static int Main(string[] args)
    {
        int numChannels = 3;
        bool runEmbeddingTests = false;
        bool verbosity = false;
        bool runKantzTest = true;
        bool runRosensteinV2 = true;

        for (int i = 0; i < 5; ++i)
        {
            Console.WriteLine("***************************************************************************");
        }
        Console.WriteLine("Run Embedded Testing? [1] Yes    [0] No ");
        string resp = Console.ReadLine();
        if(resp == "1")
        {
            runEmbeddingTests = true;
            Console.WriteLine("Embedded Tests Enabled.");
        }
        else
        {
            runEmbeddingTests = false;
            Console.WriteLine("Embedded Tests Disabled.");
        }

        Console.WriteLine("Run Kantz Testing? [1] Yes    [0] No ");
        resp = Console.ReadLine();
        if (resp == "1")
        {
            runKantzTest = true;
            Console.WriteLine("Kantz Tests Enabled.");
        }
        else
        {
            runKantzTest = false;
            Console.WriteLine("Kantz Tests Disabled.");
        }
        Console.WriteLine("Run RosensteinV2 Testing? [1] Yes    [0] No ");
        resp = Console.ReadLine();
        if (resp == "1")
        {
            runRosensteinV2 = true;
            Console.WriteLine("RosensteinV2 Tests Enabled.");
        }
        else
        {
            runRosensteinV2 = false;
            Console.WriteLine("RosensteinV2 Tests Disabled.");
        }

        TRosenstein testRosenstein = new TRosenstein(2);
        // Run L1D2 Algorithm Test.
        int retInt = testRosenstein.MainTest();

        double[] exposedTS = testRosenstein.ExposeTSData();

        if (runEmbeddingTests)
        {
            for (uint i = 1; i < 5; ++i)
            {
                Console.WriteLine("Running Embedded Dimensions Test With Reconstruction Delay of {0} samples.", i);
                FalseNearest falseNearest = new FalseNearest(exposedTS, numChannels, i, verbosity);
                falseNearest.RunFalseNeighborTest();
            }
        }

        if (runKantzTest)
        {
            Console.WriteLine("Running Kantz Algorithm **************************************************");
            Kantz kantz = new Kantz();
            kantz.SetData1D(exposedTS);
            kantz.RunAlgorithm();
            Console.WriteLine("*************************************************************************");
            Console.WriteLine();
        }

        if (runRosensteinV2)
        {
            Console.WriteLine("Running Rosenstein V2 Algorithm *****************************************");
            Rosenstein rosensteinV2 = new Rosenstein();
            rosensteinV2.SetData1D(exposedTS);
            rosensteinV2.RunAlgorithm();
            Console.WriteLine("*************************************************************************");

        }

        Console.ReadLine();

        return 0;
    }
}

