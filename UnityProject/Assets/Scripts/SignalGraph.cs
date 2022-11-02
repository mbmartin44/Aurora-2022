using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


class SignalGraph : Graph
{
    private Queue<double> samplesQueue = new Queue<double>();
    RectTransform labelX;
    RectTransform labelY;
    bool labelsInit = false;
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


        if (!labelsInit)
        {
            int seperatorCount = 10;
            for (int i = -4; i < seperatorCount; i++)
            {
                labelY = Instantiate(labelTemplateY);
                labelY.SetParent(container);
                labelY.gameObject.SetActive(true);
                float normalizedValue = i * 1f / seperatorCount;

                if (i < 5)
                {
                    labelY.anchoredPosition = new Vector2(-30f, normalizedValue * graphHeight);
                    labelY.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValue * yMax * .001f).ToString();
                }

            }

            int sepCountX = 21;
            for (int i = 0; i < sepCountX; i += 2)
            {
                labelX = Instantiate(labelTemplateX);
                labelX.SetParent(container);
                labelX.gameObject.SetActive(true);
                float test = (container.sizeDelta.x - 120) / sepCountX;

                if (i < 21)
                {
                    labelX.anchoredPosition = new Vector2(i * test, -30f);
                    labelX.GetComponent<Text>().text = (i * 0.1f).ToString();

                }

            }
            labelsInit = true;
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
