using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph : MonoBehaviour
{
    public RectTransform container;
    public RectTransform background;
    public RectTransform labelTemplateX;
    public RectTransform labelTemplateY;



    protected int samplesCount = 0;
    protected List<RectTransform> connections = new List<RectTransform>();

    public virtual void InitGraph(int samplesCount)
    {
        this.samplesCount = samplesCount;
    }

    protected virtual void ShowGraph(double[] samples)
    {
        container.sizeDelta = new Vector2(background.rect.width, background.rect.height);


        container.anchoredPosition = new Vector2(container.sizeDelta.x * 0.5f, container.sizeDelta.y * 0.5f);

    }

    protected RectTransform CreateConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("connection", typeof(Image));
        gameObject.transform.SetParent(container, false);
        gameObject.GetComponent<Image>().color = Color.blue;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();


        SetConnectionSizes(rectTransform, dotPositionA, dotPositionB);
        return rectTransform;
    }

    public virtual void UpdateGraph(double[] newSamples)
    {


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
        labelTemplateY = container.Find("labelTemplateY").GetComponent<RectTransform>();
        //DashTemplate = container.Find("DashTemplate").GetComponent<RectTransform>();
        labelTemplateX = container.Find("labelTemplateX").GetComponent<RectTransform>();



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

        connections.ForEach((connection) => { Destroy(connection.gameObject); });
        connections.Clear();
    }
}
