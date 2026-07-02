using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph<T>
{
    private Dictionary<T, HashSet<T>> adjacencyList;
    public Graph() { 
        adjacencyList = new Dictionary<T, HashSet<T>>(); 
    }
    public void AddNode(T node) { 
        if (!adjacencyList.ContainsKey(node))
        {
            adjacencyList[node] = new HashSet<T>();
        } else
        {
            Debug.Log("Node already exists.");
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
    public HashSet<T> GetNeighbors(T node)
    {
        if (!adjacencyList.ContainsKey(node))
        {
            Debug.Log("Node does not exist in the graph.");
        }
        return adjacencyList[node];
    }

    public void PrintGraph()
    {
        foreach (KeyValuePair<T,HashSet<T>> node in adjacencyList)
        {
            string adjacents = "";
            foreach (T adj in node.Value) {
                adjacents += adj.ToString() + ", ";
            }
            adjacents = adjacents.Substring(0, adjacents.Length - 2); //removes the last comma and space

            Debug.Log(String.Format("The node {0} is next to {1}", node.Key, adjacents));
        }
    }

    public void BFS(T startingNode) //T? is a nullable type / optional parameter (but doesn't work here?)
    {
        //Uses Queue, FIFO method
        Queue<T> Q = new Queue<T>();
        List<T> discovered = new List<T>();

        Q.Enqueue(startingNode);
        discovered.Add(startingNode);

        while (Q.Count > 0) {
            var currentNode = Q.Dequeue();

            // do something with the node

            //add adjacent nodes to Queue
            foreach (T childNode in adjacencyList[currentNode])
            {
                if (!discovered.Contains(childNode))
                {
                    Q.Enqueue(childNode);
                    discovered.Add(childNode);
                }
            }

        }
        //compare list of discovered and keys in Adjacency list. 
        if (discovered.Count == adjacencyList.Count)
        {
            Debug.Log("All Nodes Visited.");
        } else
        {
            Debug.Log("Graph is discontinuous.");
        }

    }


    public void DFS(T startingNode)
    {
        //Uses Stack, FILO method
        Stack<T> S = new Stack<T>();
        List<T> discovered = new List<T>();

        S.Push(startingNode);
        discovered.Add(startingNode);

        while (S.Count > 0)
        {
            var currentNode = S.Pop();

            // do something with the node

            //add adjacent nodes to Stack
            foreach (T childNode in adjacencyList[currentNode])
            {
                if (!discovered.Contains(childNode))
                {
                    S.Push(childNode);
                    discovered.Add(childNode);
                }
            }
        }
        //compare list of discovered and keys in Adjacency list. 
        if (discovered.Count == adjacencyList.Count)
        {
            Debug.Log("All Nodes Visited.");
        } else
        {
            Debug.Log("Graph is discontinuous.");
        }
    }
}
