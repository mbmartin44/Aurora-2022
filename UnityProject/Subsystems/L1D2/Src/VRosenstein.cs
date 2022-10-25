using System;
using UnityEngine;

public abstract class VRosenstein : VAlgorithm
{

    /// <summary>
    /// Allocate new matrix[rows, cols]
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="nRows"></param>
    /// <param name="nCols"></param>
    public unsafe void AllocateDMatrix(ref double[,] matrix, long nRows, long nCols)
    {
        matrix = new double[nRows, nCols];

        // Verify sufficient memory.
        if (matrix == null)
        {
            Debug.Log(OutOfMemoryMsg);
            throw new InsufficientMemoryException();
        }
    }

    /// <summary>
    /// Linearly fit the data
    /// </summary>
    /// <param name="data"></param>
    /// <param name="n"></param>
    /// <param name="m"></param>
    /// <param name="b"></param>
    /// <param name="rr"></param>
    public unsafe void LineFit(double[] data, int offset, double n, double* m, double* b, double* rr)
    {
        int i;
        double sx, sy, sxy, sx2, sy2;
        double x, y, k, mTemp, bTemp, rrTemp;

        sx = sy = sxy = sx2 = sy2 = 0;
        for (i = 0; i < n; i++)
        {
            x = i;
            y = data[i + offset];
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


    #region Abstract methods (MUST OVERRIDE)

    /// <summary>
    /// Compute the slope of the linear-fitting line
    /// </summary>
    public abstract unsafe void ComputeSlopes();

    /// <summary>
    /// Compute C2 and L1 Coefficients of the data
    /// </summary>
    /// <param name="testN"></param>
    public abstract unsafe void ProcessData(int testN);

    #endregion
}