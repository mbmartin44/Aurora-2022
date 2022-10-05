using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class SpectrumGraph : Graph
{
    double[] spectrumSamples;

    public override void InitGraph(int samplesCount)
    {
        base.InitGraph(samplesCount);

        spectrumSamples = new double[samplesCount];

        ShowGraph(spectrumSamples);
    }

    protected override void ShowGraph(double[] samples)
    {
        base.ShowGraph(samples);

        float graphHeight = container.sizeDelta.y;
        float yMax = 10000f;
        float xSize = container.sizeDelta.x / samplesCount;

        float lastX = -1;
        float lastY = -1;
        for (int i = 0; i < samples.Length; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = 0;
            yPosition = (float)samples[i] * graphHeight / yMax;
            yPosition = Mathf.Clamp(yPosition, 0, graphHeight);

            if (lastX > -1 && lastY > -1)
            {
                connections.Add(CreateConnection(new Vector2(lastX, lastY),
                    new Vector2(xPosition, yPosition)));
            }
            lastX = xPosition;
            lastY = yPosition;
        }
    }

    private void FixedUpdate()
    {
        float graphHeight = container.sizeDelta.y;
        float yMax = 10000f;
        float xSize = container.sizeDelta.x / samplesCount;

        float lastX = -1;
        float lastY = -1;
        for (int i = 0; i < spectrumSamples.Length - 1; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = (float)spectrumSamples[i] * graphHeight / yMax;
            yPosition = Mathf.Clamp(yPosition, 0, graphHeight);

            if (lastX > -1 && lastY > -1 && i < connections.Count)
            {
                UpdateConnection(new Vector2(lastX, lastY), new Vector2(xPosition, yPosition), i);
            }
            lastX = xPosition;
            lastY = yPosition;
        }
    }

    public override void UpdateGraph(double[] newSamples)
    {
        spectrumSamples = Array.ConvertAll(newSamples, (sample) => { return sample * 1e6; });
    }
}
