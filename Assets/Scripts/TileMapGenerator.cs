using System;
using System.Collections.Generic;
using System.Text;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(3)]
public class TileMapGenerator : MonoBehaviour
{

    [SerializeField]
    private UnityEvent onGenerateTileMap;

    [SerializeField]
    DungeonGenerator dungeonGenerator;

    private int[,] _tileMap;

    private void Start()
    {
        dungeonGenerator = GetComponent<DungeonGenerator>();
    }

    [Button]
    public void GenerateTileMap()
    {
        //to add: empty out arrays, remove all existing wall objects. 

        int[,] tileMap = new int[dungeonGenerator.GetDungeonBounds().height, dungeonGenerator.GetDungeonBounds().width];
        int rows = tileMap.GetLength(0);
        int cols = tileMap.GetLength(1);

        //Fill the map with empty spaces
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                tileMap[i, j] = 0;
            }
        }

        //Draw the rooms
        List<RectInt> _Dungeon_rooms = dungeonGenerator.GetRooms();

        foreach (RectInt room in _Dungeon_rooms)
        {
            for (int i = room.x; i < room.x + room.width; i++)
            {
                tileMap[room.y, i] = 1;
                tileMap[room.y + room.height - 1, i] = 1;
            }
            for (int i = room.y + 1; i < room.y + room.height - 1; i++)
            {
                tileMap[i, room.x] = 1;
                tileMap[i, room.x + room.width - 1] = 1;
            }
        }

        //Draw the doors
        List<RectInt> _dungeon_doors = dungeonGenerator.GetDoors();

        //this could be done with a foreach loop too. change later?
        for (int i = 0; i < _dungeon_doors.Count; i++)
        {
            int door_x = _dungeon_doors[i].x;
            int door_y = _dungeon_doors[i].y;
            tileMap[door_y, door_x] = 0;
        }


        _tileMap = tileMap;

        Debug.Log("Tilemap generated.");

        onGenerateTileMap.Invoke();
    }

    public string ToString(bool flip)
    {
        if (_tileMap == null) return "Tile map not generated yet.";

        int rows = _tileMap.GetLength(0);
        int cols = _tileMap.GetLength(1);

        var sb = new StringBuilder();

        int start = flip ? rows - 1 : 0;
        int end = flip ? -1 : rows;
        int step = flip ? -1 : 1;

        for (int i = start; i != end; i += step)
        {
            for (int j = 0; j < cols; j++)
            {
                sb.Append((_tileMap[i, j] == 0 ? '0' : '#')); //Replaces 1 with '#' making it easier to visualize
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public int[,] GetTileMap()
    {
        return _tileMap.Clone() as int[,];
    }

    [Button]
    public void PrintTileMap()
    {
        Debug.Log(ToString(true));
    }


}

