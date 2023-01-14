using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ODManager : MonoBehaviour
{
    public Graph graph;
    public GameObject linePrefab;
    public District[] districts;

    [ContextMenu("Population")]
    public void CheckPopulation()
    {
        int population = 0; 
        int workers = 0; 

        foreach (District district in districts)
        {
            population += district.population;
            workers += district.workers;
        }

        Debug.Log($"Мешканців: {population}");
        Debug.Log($"Робочих місць: {workers}");
        Debug.Log($"Різниця: {population - workers}");
    }

    [ContextMenu("GenerateOD")]
    public void GenerateOD()
    {
        float[,] od = Analysis.GenerateOD(districts, graph);

        
        for (int i = 0; i < districts.Length; i++)
        {
            for (int j = 0; j < districts.Length; j++)
            {
                Debug.Log($"З {districts[i].name} в {districts[j].name}: {od[i, j]} од.");
            }
        }
        
        /*
        string matrix = "            ";

        for (int i = 0; i < districts.Length; i++)
        {
            matrix += districts[i].name + "      ";
        }


        for (int i = 0; i < districts.Length; i++)
        {
            matrix += districts[i].name;
            for (int j = 0; j < districts.Length; j++)
            {
                matrix += od[i, j] + "      ";
            }
            matrix += System.Environment.NewLine;
        }
        Debug.Log(matrix);
        */
    }

    public void VisualizeOD()
    {
        float[,] od = Analysis.GenerateOD(districts, graph);

        for (int i = 0; i < districts.Length; i++)
        {
            Vector3 posA = districtPosition(districts[i]);
            for (int j = 0; j < districts.Length; j++)
            {
                if (i != j)
                {
                    Vector3 posB = districtPosition(districts[j]);

                    LineRenderer line = Instantiate(linePrefab, districts[i].transform).GetComponent<LineRenderer>();
                    float t = Mathf.InverseLerp(500, 8000, od[i, j]);
                    line.widthMultiplier = Mathf.Lerp(100, 1500, t);

                    Vector3[] points = new Vector3[2] { posA, posB };
                    line.SetPositions(points);
                }
            }
        }

        Vector3 districtPosition(District district)
        {
            Vector3 pos = new Vector3();
            foreach (Node node in district.nodes)
            {
                pos += node.gameObject.transform.position;
            }
            pos /= district.nodes.Count;
            return pos;
        }
    }

    public void DistributeFlows()
    {
        float[,] odMatrix = Analysis.GenerateOD(districts, graph);

        List<(Node, Node, float)> ODpaars = new List<(Node, Node, float)>();

        for (int i = 0; i < districts.Length; i++)
        {
            for (int j = 0; j < districts.Length; j++)
            {
                if (i != j)
                {
                    int paarAmount = districts[i].nodes.Count * districts[j].nodes.Count;
                    float flow = odMatrix[i, j] / paarAmount;

                    foreach (Node from in districts[i].nodes)
                    {
                        foreach (Node to in districts[j].nodes)
                        {
                            ODpaars.Add((from, to, flow));
                        }
                    }
                }
            }
        }

        Analysis.BigBalance(ODpaars, graph);



        for (int i = 0; i < districts.Length; i++)
        {
            for (int j = 0; j < districts.Length; j++)
            {
                if (i != j)
                {
                    float averageFreeTime = 0;
                    float averageContTime = 0;
                    int avCounter = 0;

                    // среднее время на маршрут между районами
                    foreach (Node nodeFrom in districts[i].nodes)
                    {
                        foreach (Node nodeTo in districts[j].nodes)
                        {
                            // LinkedList<Node> way = Analysis.AStar(graph, nodeFrom, nodeTo, true);
                            averageFreeTime += Analysis.WayPrice(Analysis.AStar(graph, nodeFrom, nodeTo, false), false,  graph);
                            averageContTime += Analysis.WayPrice(Analysis.AStar(graph, nodeFrom, nodeTo, true), true,  graph);

                            avCounter++;
                        }
                    }
                    averageFreeTime /= avCounter;
                    averageContTime /= avCounter;

                    float congestionLvl = (averageContTime - averageFreeTime) / averageFreeTime;

                    Debug.Log($"Показники маршрутів з {districts[i]} у {districts[j]}");
                    Debug.Log($"Об'єм кореспонденції: {odMatrix[i, j]} од.");
                    Debug.Log($"Вільний час: {averageFreeTime:F2} год");
                    Debug.Log($"Завантажений час: {averageContTime:F2} год");
                    Debug.Log($"Congestion level: {(congestionLvl * 100):F2}%");
                    Debug.Log(System.Environment.NewLine);

                }
            }

            Debug.Log(System.Environment.NewLine);

        }
    }


    public List<Node> nodes;
    public List<float> flows;
    public void LocalDistribute()
    {
        List<(Node, Node, float)> ODpaars = new List<(Node, Node, float)>();
        for (int i = 0; i < flows.Count; i++)
        {
            ODpaars.Add((nodes[i*2], nodes[i*2 + 1], flows[i]));
        }

        Analysis.BigBalance(ODpaars, graph);
    }
}
