using System;
using System.Collections.Generic;
using UnityEngine;

public class Graph<T>
{
    private Dictionary<T, List<T>> adjacencyList;
    public Graph() { 
        adjacencyList = new Dictionary<T, List<T>>(); 
    }
    public void AddNode(T node) { 
        if (!adjacencyList.ContainsKey(node))
        {
            adjacencyList[node] = new List<T>();
        } 
    }
    public void AddEdge(T fromNode, T toNode) {
        if (!adjacencyList.ContainsKey(fromNode) || !adjacencyList.ContainsKey(toNode))
        {
            Debug.Log("One or both nodes do not exist in the graph.");
            return; // exit out of method
        }
        adjacencyList[fromNode].Add(toNode);
        adjacencyList[toNode].Add(fromNode); // two way connection!
    }
    public List<T> GetNeighbors(T node)
    {
        if (!adjacencyList.ContainsKey(node))
        {
            Debug.Log("Node does not exist in the graph.");
        }
        return adjacencyList[node];
    }

    public void PrintGraph()
    {
        foreach (KeyValuePair<T,List<T>> node in adjacencyList)
        {
            string adjacents = "";
            foreach (T adj in node.Value) {
                adjacents += adj.ToString() + ", ";
            }
            adjacents = adjacents.Substring(0, adjacents.Length - 2);

            Debug.Log(String.Format("The node {0} is next to {1}", node.Key, adjacents));
        }
    }

}
