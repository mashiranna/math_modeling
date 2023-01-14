using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeManager : MonoBehaviour
{
    public GameObject edgeParent;

    public GameObject edgePrefab;

    private void Start()
    {
        // edgeParent = GameObject.Find("Edges");
    }


    public Edge CreateEdge(Node A, Node B, Graph graph, bool twoWay = true)
    {
        Edge edge = Instantiate(edgePrefab, Vector3.zero, Quaternion.identity).GetComponent<Edge>();

        edge.twoWay = twoWay;

        edge.Initialize(graph, A, B);

        edge.transform.SetParent(edgeParent.transform);

        return edge;
        // graph.AddEdge(A, B, edge);
    }

    public void CreateEdge((Node, Node) nodePaar, Graph graph)
    {
        CreateEdge(nodePaar.Item1, nodePaar.Item2, graph);
    }



}
