using UnityEngine;

public class GraphTester : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Graph<string> graph = new Graph<string>();

        graph.AddNode("A");
        graph.AddNode("B");
        graph.AddNode("C");
        graph.AddNode("D");
        graph.AddNode("E");
        graph.AddNode("F");

        graph.AddEdge("A", "B");
        graph.AddEdge("A", "C");
        graph.AddEdge("B", "D");
        graph.AddEdge("C", "D");
        graph.AddEdge("E", "D");
        graph.AddEdge("A", "E");
        graph.AddEdge("A", "F");
        graph.AddEdge("E", "F");

        Debug.Log("Graph Structure:");
        graph.PrintGraph();
    }

    // Update is called once per frame
}
