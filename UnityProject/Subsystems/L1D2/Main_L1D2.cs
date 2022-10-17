using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace L1D2
{

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
