using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class District : MonoBehaviour
{
    public int population, workers;
    public List<Node> nodes;
    public float split=0.15f;

    private void Start()
    {
        population = (int) Mathf.Floor(population * 0.15f / 3);
        workers = (int) Mathf.Floor(workers * 0.15f / 3);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Node node = collision.gameObject.transform.parent.GetComponent<Node>();
        if (!nodes.Contains(node))
        {
            nodes.Add(node);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Node node = collision.gameObject.transform.parent.GetComponent<Node>();
        if (nodes.Contains(node))
        {
            nodes.Remove(node);
        }
    }
}
