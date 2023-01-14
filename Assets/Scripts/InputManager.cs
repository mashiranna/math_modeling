using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public enum Mode { idle, build, analize }
    public static Mode mode = Mode.idle;

    public Graph graph;

    public Color normalModeColor, buildModeColor, analizeModeColor;
    

    [Header("Cash")]
    public GameObject managers;
    public Image panel;

    private void Start()
    {
        // managers = GameObject.Find("Managers");

        // graph = GameObject.Find("MainGraph").GetComponent<Graph>();

        // panel = GameObject.Find("Canvas/Panel").GetComponent<Image>();
    }


    private void Update()
    {

        // левый клик
        if (Input.GetMouseButtonDown(0))
        {
            switch (mode)
            {
                case Mode.idle:
                    break;

                case Mode.build:
                    // левый клик по пустой позиции создает узел
                    if (CheckMousePosition().collider == null)
                        managers.GetComponent<NodeManadger>().CreateNode(graph);
                    break;

                case Mode.analize:
                    break;

                default:
                    break;
            }
            
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // выбираем два узла, сбрасываем выделение при клике по уже выбраном, либо вне узлов
            RaycastHit hit = CheckMousePosition();

            if (RecieveNode(hit.collider?.gameObject.GetComponent<Node>(), out (Node, Node) nodePaar))
            {

                switch (mode)
                {
                    case Mode.idle:
                        break;
                    case Mode.build:
                        managers.GetComponent<EdgeManager>().CreateEdge(nodePaar, graph);

                        break;
                    case Mode.analize:
                        //managers.GetComponent<CalculationsManager>().ShowWay(graph, nodePaar.Item1, nodePaar.Item2);
                        Analysis.MaxFlow(graph, nodePaar.Item1, nodePaar.Item2);
                        break;
                    default:
                        break;
                }

            }



        }
    }


    // TODO выделить цветом выбраный узел
    private Node nodeA, nodeB;
    public bool RecieveNode(Node node, out (Node, Node) nodePaar) // получив два узла делает между ними ребро
    {
        nodePaar = (null, null);
        if (node == null)
        {
            nodeA = null;
            nodeB = null;
        }
        else if (nodeA == null)
        {
            nodeA = node;
        }
        else
        {
            if (node == nodeA)
            {
                nodeA = null;
            }
            else
            {
                nodeB = node;

                nodePaar = (nodeA, nodeB);

                nodeA = null;
                nodeB = null;

                return true;
            }
        }

        return false;
    }

    private RaycastHit CheckMousePosition()
    {
        Physics.Raycast(ray: Camera.main.ScreenPointToRay(Input.mousePosition), hitInfo: out RaycastHit hit);
        return hit;
    }

    public void NormalButton()
    {
        mode = Mode.idle;
        panel.color = normalModeColor;
    }
    public void BuildButton()
    {
        mode = Mode.build;
        panel.color = buildModeColor;
    }

    public void AnalizeButton()
    {
        mode = Mode.analize;
        panel.color = analizeModeColor;
    }


}
