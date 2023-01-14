using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Linq;

[ExecuteAlways]
public class Edge : MonoBehaviour
{
    public Color emptyColor = Color.white;
    public Color fullColor = Color.red;

    [Header("Параметри дороги")]
    [Delayed, Tooltip("В кожному напрямку")] public int lines;
    [Delayed] public float speedLimit;
    public bool twoWay = true;

    [Space, Header("Параметри мережі")]
    public Graph manualGraph;
    public Node nodeA, nodeB;

    [Space, Header("Технічні дані (readonly!)")] // заприватить мб
    public float flow;
    public float capacity;
    public float distance;
    public Edge oppositeLine;

    private Text capacityInputField;
    private Text flowText;

    private LineRenderer line;

    public float ResidualFlow
    {
        get { return capacity - flow; }
    }

    private void OnValidate()
    {
        if (nodeA != null && nodeB != null)
        {
            DeleteEdge(false);
            Initialize(manualGraph, nodeA, nodeB);
        }
    }

    private void Start()
    {
        transform.SetParent(GameObject.Find("Edge container").transform);

    }


    [ContextMenu("Reboot")]
    public void Reboot()
    {
        if (nodeA != null && nodeB != null)
        {
            DeleteEdge(false);
            Initialize(manualGraph, nodeA, nodeB);
        }
    }

    public void Initialize(Graph graph, Node from, Node to)
    {
        DeleteEdge(false);

        manualGraph = graph;

        nodeA = from;
        nodeB = to;


        flow = 0;
        CalculateWeight();

        nodeA.AddEdge(this);
        nodeB.AddEdge(this);

        graph.AddEdge(nodeA, nodeB, this);

        line = gameObject.GetComponent<LineRenderer>();

        capacityInputField = transform.Find("Canvas/InputField/Text").GetComponent<Text>();
        flowText = transform.Find("Canvas/Panel/FlowText").GetComponent<Text>();


        try
        {
            oppositeLine?.DrawEdge();
        }
        catch (MissingReferenceException)
        {
            Debug.LogWarning("Catched exeption!");
            oppositeLine = null;
            throw;
        }
        
        if (twoWay)
        {
            if (oppositeLine == null)
            {
                // ищем может встречка есть, но потерялась
                IEnumerable<Edge> intersect = nodeA.edgeList.Intersect(nodeB.edgeList);
                if (intersect != null)
                {
                    foreach (Edge edge in intersect)
                    {
                        if (edge != this)
                        {
                            oppositeLine = edge;
                            edge.oppositeLine = this;
                        }
                    }
                }
            }


            // если встречки еще не уществует
            if (oppositeLine == null )
            {

                oppositeLine = this;
                // создает копию себя
                oppositeLine = Instantiate(gameObject, gameObject.transform.parent).GetComponent<Edge>();
                oppositeLine.oppositeLine = this;

                oppositeLine.Initialize(manualGraph, to, from);
            }
            else
            // встречка существует, обновить и её
            {
                if (oppositeLine.speedLimit != speedLimit)
                    oppositeLine.speedLimit = this.speedLimit;

                if (oppositeLine.lines != lines)
                    oppositeLine.lines = this.lines;



                if (oppositeLine.nodeA != this.nodeB || oppositeLine.nodeB != this.nodeA)
                {
                    oppositeLine.DeleteEdge(false);
                    oppositeLine.Initialize(manualGraph, to, from);
                }

                oppositeLine.CalculateCapacity();
                oppositeLine.DrawEdge();
            }
        }
        else
        {
            if (oppositeLine != null)
            {
                GameObject tempOpLine = oppositeLine.gameObject;
                oppositeLine.DeleteEdge(true);

                UnityEditor.EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(tempOpLine);
                };
            }
        }

        CalculateCapacity();
        FlowColor();
        DrawEdge();
    }


    [ContextMenu("Recalculate graph")]
    public void ReGraph()
    {
        if (nodeA && nodeB)
        {
            manualGraph.RemoveEdge(this);
            manualGraph.AddEdge(nodeA, nodeB, this);
        }
    }

    [ContextMenu("Change direction")]
    private void ChangeDir()
    {
        Node temp = nodeA;
        nodeA = nodeB;
        nodeB = temp;

        ReGraph();
        DrawEdge();
    }


    private void OnDestroy()
    {
        DeleteEdge(true);
    }

    public void DeleteEdge(bool includingOppositeLine)
    {
        nodeA?.edgeList.Remove(this);
        nodeB?.edgeList.Remove(this);
        manualGraph.RemoveEdge(this);

        if (includingOppositeLine)
        {
            if (oppositeLine != null)
                oppositeLine.oppositeLine = null;
            oppositeLine = null;
        }
    }

    public void CalculateWeight()
    {
        distance = (nodeA.transform.position - nodeB.transform.position).magnitude * 0.579f; // scale fix
    }

    public void SetFlowText()
    {
        flowText.text = flow.ToString();
    }

    [ContextMenu("ReDraw")]
    public void DrawEdge(bool both=true)
    {
        line = GetComponent<LineRenderer>();

        float c = Mathf.InverseLerp(2000, 6000, capacity);
        line.widthMultiplier = Mathf.Lerp(20, 100, c);

        Vector3 posA, posB;

        if (twoWay)
        {
            posA = nodeA.gameObject.transform.position;
            posB = nodeB.gameObject.transform.position;

            Vector3 dir = posB - posA;
            Vector3 offset = Vector3.Cross(dir, Vector3.back).normalized;

            posA += offset * (line.widthMultiplier / 2 + 5);
            posB += offset * (line.widthMultiplier / 2 + 5);

            if (both)
            {
                oppositeLine?.DrawEdge(false);
            }
            
        }
        else
        {
            posA = nodeA.gameObject.transform.position;
            posB = nodeB.gameObject.transform.position;
        }

        gameObject.transform.position = (posA + posB) / 2f;
        Vector3[] points = new Vector3[2] { posA, posB };
        line.SetPositions(points);
    }


    [Space, Header("Параметри функції пропускної здатності")]
    [Range(0.2f, 0.8f), Delayed, Tooltip("Якість покриття. Значення 0.3-0.5 приблизно відповідають мокрому асфальту")]
    public float phi = 0.6f;
    public float heavyVehicleKoof = 1;
    public void CalculateCapacity()
    {
        /*
        capacity = lines * (3600 * speedLimit * phi) /
            (heavyVehicleKoof * (phi * (speedLimit + 7) + 0.015f * Mathf.Pow(speedLimit, 2)));
        */
        capacity = lines * 2000;
    }


    // виклик кнопкою
    public void SetCapacity()
    {
        if (capacityInputField.text.Length == 0)
        {
            capacity = 1;
        }
        else
        {
            capacity = float.Parse(capacityInputField.text, CultureInfo.InvariantCulture.NumberFormat);
        }

        capacityInputField.text = capacity.ToString(); // TODO не работает, вообще исправь чтоб запись через точку работала а не только запятую
    }
    
    public void FlowColor()
    {
        float load = flow / capacity;
        Color color = Color.Lerp(emptyColor, fullColor, load);

        line.startColor = color;
        line.endColor = color;

        /*
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];

        
        colorKeys[0].color = Color.red; 
        colorKeys[0].time = 0; 
        colorKeys[1].color = Color.red;
        colorKeys[1].time = 1;

        alphaKeys[0].alpha = 1;
        alphaKeys[0].time = 0;
        alphaKeys[1].alpha = 1;
        alphaKeys[1].time = 1;

        gameObject.GetComponent<LineRenderer>().colorGradient.SetKeys(colorKeys, alphaKeys);
        */
    }

}
