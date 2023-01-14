using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


[ExecuteAlways]
public class Node : MonoBehaviour
{
    [Header("Параметри генерації потоку")]
    public float source;
    public float sink;

    [Space, Header("Параметри мережі")]
    public Graph manualGraph;
    public List<Edge> edgeList = new List<Edge>();
    

    public void AddEdge(Edge edge)
    {
        if (!edgeList.Contains(edge))
        {
            edgeList.Add(edge);
        }
    }


    private void Start()
    {
        // transform.SetParent(GameObject.Find("Node container").transform);

        ManualInit();
    }

    public void Initialize(Graph graph)
    {
        graph.AddNode(this);
        manualGraph = graph;
    }

    public void ManualInit()
    {
        Initialize(manualGraph);
    }

    private void OnDestroy()
    {
        DeleteNode();
    }

    public void DeleteNode()
    {
        foreach (Edge edge in edgeList)
        {
            if (edge != null)
            {
                if (edge.nodeA == this)
                {
                    edge.nodeA = null;
                }
                else if (edge.nodeB == this)
                {
                    edge.nodeB = null;
                }
            }
        }

        manualGraph.RemoveNode(this);
    }

    [ContextMenu("Recalculate graph")]
    public void ReGraph()
    {
        manualGraph.RemoveNode(this);
        manualGraph.AddNode(this);
    }

    [ContextMenu("ReDraw")]
    public void DrawEdges()
    {
        foreach (Edge edge in edgeList)
        {
            edge.DrawEdge();
            edge.CalculateWeight();
        }

    }

    private void OnMouseDrag()  // перемещение узла
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos;

        DrawEdges();
    }
    
}
