using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class FloodFillSpawner : MonoBehaviour
{
    [SerializeField]
    TileMapGenerator tileMapGenerator;
    [SerializeField]
    DungeonGenerator dungeonGenerator;
    [SerializeField]
    UnityEvent floorFilled;

    public GameObject floorTile;
    void Start()
    {
        tileMapGenerator = GetComponent<TileMapGenerator>();
        dungeonGenerator = GetComponent<DungeonGenerator>();
    }

    public void startFloorGen()
    {
        StartCoroutine(BFSFloorFill());
    }

    [Button]
    public IEnumerator BFSFloorFill()
    {
        yield return null;
        List<RectInt> _rooms = dungeonGenerator.GetRooms();
        int[,] _tileMap = tileMapGenerator.GetTileMap();

        int start_x = _rooms[0].y + _rooms[0].height / 2;
        int start_y = _rooms[0].x + _rooms[0].width / 2;

        var startingNode = (start_x, start_y);
        GameObject parentGameObject = new GameObject("Floors");

        //Uses Queue, FIFO method
        Queue<(int x, int y)> Q = new Queue<(int x, int y)>();

        HashSet<(int x, int y)> discovered = new HashSet<(int x, int y)>();


        Q.Enqueue(startingNode);
        discovered.Add(startingNode);

        while (Q.Count > 0)
        {
            var currentNode = Q.Dequeue();

            //Debug.Log(currentNode[0] + " " + currentNode[1]+ " " + currentNode[2]);
            if (_tileMap[currentNode.x, currentNode.y] == 0)
            {
                int current_x = currentNode.x;
                int current_y = currentNode.y;

                // spawn in the floor tile asset
                var newFloor = Instantiate(floorTile, new Vector3(current_y + 0.5f, 0, current_x + 0.5f), Quaternion.identity, parentGameObject.transform);
                newFloor.name = $"Floor_{current_y}_{current_x}";

                yield return null;

                //add adjacent coordinates to queue
                (int x, int y)[] directions = new (int x, int y)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };

                foreach ((int dx, int dy) in directions)
                {

                    int new_x = current_x + dx;
                    int new_y = current_y + dy;

                    if (!discovered.Contains((new_x, new_y)))
                    {
                        Q.Enqueue((new_x, new_y));
                        discovered.Add((new_x, new_y));
                    }
                }

            }


        }

        floorFilled.Invoke();
    }
}
