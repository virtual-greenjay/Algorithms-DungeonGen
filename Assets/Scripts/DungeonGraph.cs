using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGraph : MonoBehaviour
{
    [SerializeField]
    DungeonGenerator dungeonGenerator;

    Graph<RectInt> dungeonGraph = new Graph<RectInt>();
    private List<RectInt> _Dungeon_rooms;

    private void Start()
    {
        dungeonGenerator = GetComponent<DungeonGenerator>();
    }

    [Button]
    void populateDungeonGraph()
    {
        _Dungeon_rooms = dungeonGenerator.GetRooms();
        List<RectInt> _Dungeon_doors = dungeonGenerator.GetDoors();
        List<RectInt> _discovered_doors = new List<RectInt>();

        foreach (RectInt room in _Dungeon_rooms)
        {
            dungeonGraph.AddNode(room);
            foreach (RectInt door in _Dungeon_doors)
            {
                if (AlgorithmsUtils.Intersects(room, door))
                {
                    if (!_discovered_doors.Contains(door))
                    {
                        dungeonGraph.AddNode(door);
                        _discovered_doors.Add(door);
                    }
                    dungeonGraph.AddEdge(room, door);
                }

            }
        }
    }

    void drawDungeonGraph()
    {
        //problem for later. Idk how to do this (and it's apparently not required? check that)
    }

    [Button]
    void checkConnectivity()
    {
        if (dungeonGraph == null)
        {
            Debug.Log("Graph not generated yet.");
        } else
        {
            dungeonGraph.BFS(_Dungeon_rooms[0]);
        }
            
    }
}
