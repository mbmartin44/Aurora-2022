using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class SignalGraph : Graph
{
    private Queue<double> samplesQueue = new Queue<double>(50);

    public override void InitGraph(int samplesCount)
    {
        base.InitGraph(samplesCount);

        for (int i = 0; i < samplesCount; i++)
        {
            samplesQueue.Enqueue(0);
        }
        ShowGraph(samplesQueue.ToArray());
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
            yPosition = graphHeight / (yMax * 2.0f) * ((float)samples[i] + yMax);
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
        double[] samples = samplesQueue.ToArray();

        float graphHeight = container.sizeDelta.y;
        float yMax = 10000f;
        float xSize = container.sizeDelta.x / samplesCount;

        float lastX = -1;
        float lastY = -1;
        for (int i = 0; i < samples.Length - 1; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = graphHeight / (yMax * 2.0f) * ((float)samples[i] + yMax);
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
        for (int i = 0; i < newSamples.Length; i++)
        {
            if (samplesQueue.Count < samplesCount)
            {
                samplesQueue.Enqueue(newSamples[i] * 1e6);
            }
            else
            {
                samplesQueue.Enqueue(newSamples[i] * 1e6);
                samplesQueue.Dequeue();
            }
        }
    }

    public override void Close()
    {
        samplesQueue.Clear();
        base.Close();
    }
}
