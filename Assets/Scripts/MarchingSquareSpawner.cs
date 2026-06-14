using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class MarchingSquareSpawner : MonoBehaviour
{
    [SerializeField]
    TileMapGenerator tileMapGenerator;
    [SerializeField]
    DungeonGenerator dungeonGenerator;

    public GameObject[] wallTiles;
    private int[,] _wallTilesMap;

    void Start()
    {
        tileMapGenerator = GetComponent<TileMapGenerator>();
        dungeonGenerator = GetComponent<DungeonGenerator>();
    }

    public void startWallGen()
    {
        StartCoroutine(GenerateWalls());
    }

    //to do: make this an IEnumerator/Coroutine, get it animated
    [Button]
    public IEnumerator GenerateWalls()
    {
        yield return null;

        int[,] _tileMap = tileMapGenerator.GetTileMap();
        _wallTilesMap = new int[_tileMap.GetLength(0), _tileMap.GetLength(1)];

        GameObject parentGameObject = new GameObject("Walls");

        int multiplexer = 0;


        for (int i = 0; i < _tileMap.GetLength(0) - 1; i++)
        {
            for (int j = 0; j < _tileMap.GetLength(1) - 1; j++)
            {
                int _br_cell = _tileMap[i, j];
                int _tr_cell = _tileMap[i + 1, j];
                int _bl_cell = _tileMap[i, j + 1];
                int _tl_cell = _tileMap[i + 1, j + 1];
                int _case = _br_cell + (2 * _tr_cell) + (4 * _tl_cell) + (8 * _bl_cell);

                if (_case == 0)
                {
                    //empty position.
                }
                else if (_case < 0 || _case == 5 || _case == 10 || _case > 14)
                {
                    Debug.Log("Error: Case equals " + _case + ".");
                    //case 5 and 10 are touching corners, which should not occur. 
                }
                else
                {
                    //spawn a tile at position y = i+0.5, x = j+0.5. 
                    var newWall = Instantiate(wallTiles[_case], new Vector3(j + 1, 0, i + 1), Quaternion.identity, parentGameObject.transform);
                    newWall.name = $"Wall_{j}_{i}";

                    if (multiplexer++ % 4 == 0 && dungeonGenerator.isAnimated) yield return null;

                }
                _wallTilesMap[i, j] = _case;
            }
        }
    }

    public string ToString(bool flip)
    {
        if (_wallTilesMap == null) return "Tile map not generated yet.";

        int rows = _wallTilesMap.GetLength(0);
        int cols = _wallTilesMap.GetLength(1);

        var sb = new StringBuilder();

        //ternary conditional operator - if false, prints table as is (upside down), if true, flips the table to print right side up.
        int start = flip ? rows - 1 : 0;
        int end = flip ? -1 : rows;
        int step = flip ? -1 : 1;

        for (int i = start; i != end; i += step)
        {
            for (int j = 0; j < cols; j++)
            {
                sb.Append(_wallTilesMap[i, j] + ".");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }


    [Button]
    public void PrintTileNrs()
    {
        Debug.Log(ToString(true));
    }
}

