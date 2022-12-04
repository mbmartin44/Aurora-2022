///--------------------------------------------------------------------------------------
/// <file>    Graph.cs                                </file>
/// <date>    Last Edited: 12/03/2022                              </date>
///--------------------------------------------------------------------------------------
/// <summary>
/// This class is used as a generic base class from which other graphs derive.
///<see cref="SignalGraph"/> for an example of a graph that derives from this class.
/// See also:
/// -   Unity GameObjects:    https://docs.unity3d.com/Manual/GameObjects.html
/// -   Unity line renderers: https://docs.unity3d.com/ScriptReference/LineRenderer.html
/// -   Unity Components:     https://docs.unity3d.com/ScriptReference/Component.html
/// -   Unity MonoBehaviour:  https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
/// -   Unity RectTransform:  https://docs.unity3d.com/ScriptReference/RectTransform.html
/// </summary>
/// -------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is used to control the Graph.
/// </summary>
public class Graph : MonoBehaviour
{
    public RectTransform container;
    public RectTransform background;
    public RectTransform labelTemplateX;
    public RectTransform labelTemplateY;

    protected int samplesCount = 0;
    protected List<RectTransform> connections = new List<RectTransform>();

    /// <summary>
    /// This function initializes the graph, setting the number of samples to be displayed
    /// to the specified value. This function should be called before any other function
    /// in this class.
    /// </summary>
    public virtual void InitGraph(int samplesCount)
    {
        this.samplesCount = samplesCount;
    }

    /// <summary>
    /// ShowGraph() is called when the graph should be drawn. It is called every time the graph is updated.
    /// It takes in a double array of samples and draws the graph based on the samples.
    /// This function draws the graph by setting the size of the container to match the size of the background.
    /// Then, the container's position is set to be the center of the background.
    /// </summary>
    /// <param name="samples">The samples to be drawn on the graph.</param>
    protected virtual void ShowGraph(double[] samples)
    {
        container.sizeDelta = new Vector2(background.rect.width, background.rect.height);
        container.anchoredPosition = new Vector2(container.sizeDelta.x * 0.5f, container.sizeDelta.y * 0.5f);
    }


    /// <summary>
    /// This method creates a connection between two dots. It creates a new GameObject "connection" with an Image component that is a child of the container.
    /// The RectTransform of the connection is set to the size of the distance between the two dots.
    /// </summary>
    /// <param name="dotPositionA">The position of the first dot</param>
    /// <param name="dotPositionB">The position of the second dot</param>
    /// <returns>The RectTransform of the connection</returns>
    protected RectTransform CreateConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        // Create a new GameObject "connection" with an Image component that is a child of the container.
        GameObject gameObject = new GameObject("connection", typeof(Image));
        // Set the parent of the connection to the container.
        gameObject.transform.SetParent(container, false);
        gameObject.GetComponent<Image>().color = Color.blue;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        SetConnectionSizes(rectTransform, dotPositionA, dotPositionB);
        return rectTransform;
    }

    /// <summary>
    /// Updates the graph from the new samples.
    /// </summary>
    /// <param name="newSamples">The new samples to be plotted</param>
    /// <remarks>
    /// This function must be implemented by each individual derived class.
    /// </remarks>
    public virtual void UpdateGraph(double[] newSamples)
    {
    }

    /// <summary>
    /// This function sets the size of the connection between two dots.
    /// </summary>
    /// <param name="rectTransform">The RectTransform of the connection</param>
    /// <param name="dotPositionA">The position of the first dot</param>
    /// <param name="dotPositionB">The position of the second dot</param>
    protected void UpdateConnection(Vector2 dotPositionA, Vector2 dotPositionB, int connectionId)
    {
        SetConnectionSizes(connections[connectionId], dotPositionA, dotPositionB);
    }

    /// <summary>
    /// This function sets the size of a connection, based on the position of its dots.
    /// </summary>
    private void SetConnectionSizes(RectTransform rectTransform, Vector2 dotPositionA, Vector2 dotPositionB)
    {
        // Get the unit vector in the direction of the connection
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        // Get the distance between the two dots
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        // Set the size of the connection
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 1f);

        // Set the position of the connection
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
        // Get the component of the connection's RectTransform that is used to rotate the connection
        // and set its rotation to the angle between the two dots
        labelTemplateY = container.Find("labelTemplateY").GetComponent<RectTransform>();
        labelTemplateX = container.Find("labelTemplateX").GetComponent<RectTransform>();
    }

    /// <summary>
    /// This function returns the angle of a vector in degrees.
    /// </summary>
    private float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    /// <summary>
    /// Destroy all connection objects and clear the list of connections
    /// </summary>
    public virtual void Close()
    {
        connections.ForEach((connection) => { Destroy(connection.gameObject); });
        connections.Clear();
    }
}