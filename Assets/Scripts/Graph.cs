using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[ExecuteAlways]
[CreateAssetMenu(fileName = "Graph", menuName = "ScriptableObjects/Graph", order = 1)]
public class Graph : ScriptableObject
{

    public Dictionary<Node, Dictionary<Node, Edge>> nodeList = new Dictionary<Node, Dictionary<Node, Edge>>();


    public void AddNode(Node node)
    {
        if (!nodeList.ContainsKey(node))
        {
            nodeList.Add(node, new Dictionary<Node, Edge>());
        }
    }
    public void RemoveNode(Node node) // не тестил
    {
        if (nodeList.ContainsKey(node))
        {
            foreach (KeyValuePair<Node, Dictionary<Node, Edge>> nodes in nodeList)
            {
                nodes.Value.Remove(node);
            }

            nodeList.Remove(node);

        }
    }

    public void AddEdge(Node from, Node to, Edge edge)
    {
        if (nodeList.ContainsKey(from))
        {
            //  если между этими узлами ребро уже существует и оно не текущее, значит дубль пытается податься
            if (nodeList[from].ContainsKey(to) && nodeList[from][to] != edge)
            {
                edge.DeleteEdge(true);

                UnityEditor.EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(edge.gameObject);
                };

            }
            else
            // такого ребра еще не существует
            {
                nodeList[from].Add(to, edge);

                from.AddEdge(edge);
                to.AddEdge(edge);

            }
        }


    }

    public void RemoveEdge(Node from, Node to) // не тестил
    {
        if (nodeList.ContainsKey(from) && nodeList[from].ContainsKey(to))
        {
            nodeList[from].Remove(to);
        }
    }

    public void RemoveEdge(Edge edge) // не тестил
    {
        foreach (KeyValuePair<Node, Dictionary<Node, Edge>> node in nodeList)
        {
            // мб эта проверка дублирует следующий перебор
            if (node.Value.ContainsValue(edge))
            {
                foreach (KeyValuePair<Node, Edge> subnode in node.Value)
                {
                    if (subnode.Value == edge)
                    {
                        RemoveEdge(node.Key, subnode.Key);
                        return;
                    }
                }
            }
        }
    }
    

    [ContextMenu("Recalculate all")]
    public void ReGraph()
    {
        nodeList.Clear();

        Edge[] edges = FindObjectsOfType<Edge>();
        Node[] nodes = FindObjectsOfType<Node>();

        foreach (Node node in nodes)
        {
            node.ReGraph();
        }

        foreach (Edge edge in edges)
        {
            edge.ReGraph();
        }
    }

}
