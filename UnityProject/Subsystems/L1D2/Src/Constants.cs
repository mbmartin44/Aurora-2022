using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Constants
{
    // Constants used in the direct Rosenstein (V1)
    public const double IOTA = 10e-15;
    public const int MAX_LN_R = 12;
    public const int MIN_LN_R = -12;
    public const int N_LN_R = 600;

    // Constants used in Tisean Algorithms
    public const ulong ULONG_MAX = 0xffffffffUL;
    public const int BOX = 128;

    public enum AlgResult { PositiveResult, NegativeResult, NeedMoreData };
    public class DiagnosticReport
    {
        public double LLE;
        public double SNR;
        public double CV;
        public bool detection;
    }

}

