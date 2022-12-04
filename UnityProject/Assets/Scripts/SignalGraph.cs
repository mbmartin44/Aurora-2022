///--------------------------------------------------------------------------------------
/// <file>    DeviceController.cs                                </file>
/// <date>    Last Edited: 12/03/2022                              </date>
///--------------------------------------------------------------------------------------
/// <summary>
/// This class is used to draw the signal graph.
/// </summary>
/// -------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is used to draw the signal graph.
/// </summary>
class SignalGraph : Graph
{
    // The samples to be drawn on the graph.
    private Queue<double> samplesQueue = new Queue<double>();
    // The labelX is used to display the sampleCount.
    RectTransform labelX;
    // The labelY is used to display the sample value.
    RectTransform labelY;
    bool labelsInit = false;

    /// <summary>
    /// This function initializes the graph, setting the number of samples to be displayed
    /// to the specified value. This function should be called before any other function
    /// in this class.
    /// </summary>
    public override void InitGraph(int samplesCount)
    {
        base.InitGraph(samplesCount);

        for (int i = 0; i < samplesCount; i++)
        {
            samplesQueue.Enqueue(0);
        }
        ShowGraph(samplesQueue.ToArray());
    }

    /// <summary>
    /// This function is called when the graph should be drawn. It is called every time the graph is updated.
    /// It takes in a double array of samples and draws the graph based on the samples.
    /// This function draws the graph by setting the size of the container to match the size of the background.
    /// Then, the container's position is set to be the center of the background.
    /// </summary>
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

    /// <summary>
    /// This is called on fixed intervals to update the graph.
    /// </summary>
    private void FixedUpdate()
    {
        double[] samples = samplesQueue.ToArray();

        // Set the graph height to be the same as the background.
        float graphHeight = container.sizeDelta.y;
        float yMax = 10000f;
        // Set the xSize to be the width of the background divided by the number of samples.
        float xSize = container.sizeDelta.x / samplesCount;

        float lastX = -1;
        float lastY = -1;
        // Loop through the samples.
        for (int i = 0; i < samples.Length - 1; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = graphHeight / (yMax * 2.0f) * ((float)samples[i] + yMax);
            yPosition = Mathf.Clamp(yPosition, 0, graphHeight);
            // If the lastX and lastY are not -1, then create a connection between the last point and the current point.
            if (lastX > -1 && lastY > -1 && i < connections.Count)
            {
                UpdateConnection(new Vector2(lastX, lastY), new Vector2(xPosition, yPosition), i);
            }
            lastX = xPosition;
            lastY = yPosition;

        }
        // If the labels have not been initialized, initialize them.
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
            // Loop through the number of seperators.
            for (int i = 0; i < sepCountX; i += 2)
            {
                // Instantiate the label.
                labelX = Instantiate(labelTemplateX);
                // Set the parent of the label to be the container.
                labelX.SetParent(container);
                // Set the label to be active.
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

    /// <summary>
    /// This function is used to update the graph. The function takes in an array of new samples and adds them to the queue.
    /// If the queue is not full, it adds the samples to the queue. If the queue is full, it removes the oldest sample in the queue
    /// and adds the new sample.
    /// </summary>
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

    /// <summary>
    /// This function is used to clear the graph. It clears the queue and sets the graph to be empty.
    /// </summary>
    public override void Close()
    {
        samplesQueue.Clear();
        base.Close();
    }
}
