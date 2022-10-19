using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace L1D2
{
    /// <summary>
    /// This is an example of running the file-test for L1D2 and will be 
    /// updated as the project development continues. 
    /// This file should not be included when the library is compiled 
    /// (Should never be derived from Monobehavior)
    /// </summary>
    class Main_L1D2
    {
        unsafe static int Main(string[] args)
        {
            L1D2Algorithm alg = new L1D2Algorithm();

            // Run the test for L1D2 Algorithm.
            int retInt = alg.MainTest();

            return 0;
        }
    }
}
