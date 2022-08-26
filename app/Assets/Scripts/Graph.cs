using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    public RectTransform container;
    public RectTransform background;

    protected int samplesCount = 0;
    protected List<RectTransform> connections = new List<RectTransform>();

    

    public virtual void InitGraph(int samplesCount)
    {
        this.samplesCount = samplesCount;
    }

    /*public void InitGraph(int samplesCount)
    {
        this.samplesCount = samplesCount;

        for (int i = 0; i < samplesCount; i++)
        {
            samplesQueue.Enqueue(0);
        }
        ShowGraph(samplesQueue.ToArray());
    }*/


    protected virtual void ShowGraph(double[] samples)
    {
        container.sizeDelta = new Vector2(background.rect.width, background.rect.height);
        
        container.anchoredPosition = new Vector2(container.sizeDelta.x * 0.5f, container.sizeDelta.y * 0.5f);
        /*float graphHeight = container.sizeDelta.y;
        float yMax = 10000f;
        float xSize = container.sizeDelta.x / samplesCount;

        float lastX = -1;
        float lastY = -1;
        for (int i = 0; i < samples.Length; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = 0;
            if (mode == GraphMode.Signal)
            {
                yPosition = graphHeight / (yMax * 2.0f) * ((float)samples[i] + yMax);
            }
            else
            {
                yPosition = (float)samples[i] * graphHeight / yMax;
            }
            yPosition = Mathf.Clamp(yPosition, 0, graphHeight);

            if (lastX > -1 && lastY > -1) {
                connections.Add(CreateConnection(new Vector2(lastX, lastY),
                    new Vector2(xPosition, yPosition)));
            }
            lastX = xPosition;
            lastY = yPosition;
        }*/
    }

    protected RectTransform CreateConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("connection", typeof(Image));
        gameObject.transform.SetParent(container, false);
        gameObject.GetComponent<Image>().color = Color.black;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        
        SetConnectionSizes(rectTransform, dotPositionA, dotPositionB);
        return rectTransform;
    }

    /*private void FixedUpdate()
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
            float yPosition = 0;
            if (mode == GraphMode.Signal)
            {
                yPosition = graphHeight / (yMax * 2.0f) * ((float)samples[i] + yMax);
            }
            else
            {
                yPosition = (float)samples[i] * graphHeight / yMax;
            }
            yPosition = Mathf.Clamp(yPosition, 0, graphHeight);

            if (lastX > -1 && lastY > -1 && i < connections.Count)
            {
                UpdateConnection(new Vector2(lastX, lastY), new Vector2(xPosition, yPosition), i);
            }
            lastX = xPosition;
            lastY = yPosition;
        }
    }*/

    public virtual void UpdateGraph(double[] newSamples)
    {
        /*for (int i = 0; i < newSamples.Length; i++)
        {
            if (samplesQueue.Count < samplesCount)
            {
                samplesQueue.Enqueue(newSamples[i]  * 1e6);
            }
            else
            {
                samplesQueue.Enqueue(newSamples[i] * 1e6);
                samplesQueue.Dequeue();
            }
        }*/

    }

    protected void UpdateConnection(Vector2 dotPositionA, Vector2 dotPositionB, int connectionId)
    {
        SetConnectionSizes(connections[connectionId], dotPositionA, dotPositionB);
    }

    private void SetConnectionSizes(RectTransform rectTransform, Vector2 dotPositionA, Vector2 dotPositionB)
    {
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 1f);
        
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
    }

    private float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public virtual void Close()
    {
        //samplesQueue.Clear();
        connections.ForEach((connection) => { Destroy(connection.gameObject); });
        connections.Clear();
    }
}
