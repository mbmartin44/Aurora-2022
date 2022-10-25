using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public abstract class VAlgorithm
{

    // Total number of divergence sums and correlation sums in the arrays
    public int gMaxDivergeT = 0;

    // System Parameters
    public struct AuroraConfig
    {
        public int numEEGChannels; // = 4;
        public int numSamples;     // = 2000;
        public int sampFreq;
    }

    private const string outOfMemoryMsg = "\nOUT OF MEMORY: ProcessTest\n\n";
    public string OutOfMemoryMsg
    {
        get => outOfMemoryMsg;
    }

    /// <summary>
    /// L1D2 (Rosenstein) Algorithm Configurables
    /// </summary>
    public struct Parameters
    {
        public int seriesN;    // Only for testing
        public long startIndex; // Only for testing
        public long stopIndex;  // Only for testing
        public int m;          // embedded dimensions;
        public int J;          // Reconstruction (lag) delay;
        public int W;          // UNKNOWN (Need to determine meaning);
        public int divergeT;   // Total number of divergences taken;
    }

    // Algorithm parameters used internally
    private Parameters params_;
    public Parameters Params_
    {
        get => params_;
        set => params_ = value;
    }

    private double[] gData;
    public double[] GData
    {
        get => gData;
        set { gData = value; }
    }

    /// <summary>
    /// Divergence sum array
    /// </summary>
    private double[] gNDivergence; // C# version (maybe use this)
    public double[] GNDivergence
    {
        get => gNDivergence;
        set { gNDivergence = value; }
    }

    /// <summary>
    /// Divergence sum matrix
    /// </summary>
    private double[,] gDivergence;
    public double[,] GDivergence
    {
        get => gDivergence;
        set { gDivergence = value; }
    }

    /// <summary>
    /// Correlation sum matrix
    /// </summary>
    private double[,] gCSum; // C# version (Maybe use this)
    public double[,] GCSum
    {
        get => gCSum;
        set { gCSum = value; }
    }

}