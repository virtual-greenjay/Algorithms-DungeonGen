using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class DungeonGenerator : MonoBehaviour
{

    public int dungeonWidth = 100;
    public int dungeonHeight = 60;
    public int minRoomLength = 10;
    public bool splitVertically = true;
    public bool roomsChanged = false;

    RectInt dungeonRoom = new RectInt(0, 0, 100, 60);

    List<RectInt> Rooms = new List<RectInt>(); 

    int step = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rooms.Add(new RectInt(0, 0, dungeonWidth, dungeonHeight));
        //int i = 0;
        //do
        //{
        //    roomsChanged = false;
        //    var parentRoom = Rooms[i];
        //    if (splitVertically)
        //    {
        //        if (parentRoom.width >= minRoomLength)
        //        {
        //            Rooms.Add(new RectInt(parentRoom.x, parentRoom.y, (parentRoom.width / 2) + 1, parentRoom.height));
        //            Rooms.Add(new RectInt(parentRoom.x + (parentRoom.width / 2) - 1, parentRoom.y, (parentRoom.width / 2) + 1, parentRoom.height));
        //            roomsChanged = true;
        //            splitVertically = false;
        //            Rooms.RemoveAt(i);
        //            continue;
        //        }
        //    }
        //    else
        //    {
        //        if (parentRoom.height >= minRoomLength)
        //        {
        //            Rooms.Add(new RectInt(parentRoom.x, parentRoom.y, parentRoom.width, (parentRoom.height / 2) + 1));
        //            Rooms.Add(new RectInt(parentRoom.x, parentRoom.y + (parentRoom.height / 2) - 1, parentRoom.width, (parentRoom.height / 2) + 1));
        //            roomsChanged = true;
        //            splitVertically = true;
        //            Rooms.RemoveAt(i);
        //            continue;
        //        }
        //    }
        //    i++;

        //} while (roomsChanged == true && i < Rooms.Count);

    }

    // Update is called once per frame
    void Update()
    {
        AlgorithmsUtils.DebugRectInt(dungeonRoom, Color.blue); 
        for (int i=0; i<Rooms.Count; i++)
        {
            Color roomDepth = new Color(1, i * 0.02f, 0, 1);
            AlgorithmsUtils.DebugRectInt(Rooms[i], roomDepth);
        }
        
        if(Input.GetKeyUp(KeyCode.R))
        {
            SplitRoom();
        }
    }


    void SplitRoom()
    {
        if (step < Rooms.Count)
        {
            if (splitVertically) {
                VerticalSplit();
            }
            else
            {
                HorizontalSplit();
            }
        } else
        {
            Debug.Log("Generation complete.");
        }
    }

    void VerticalSplit()
    {
        var parentRoom = Rooms[step];
        if (parentRoom.width >= 2*minRoomLength)
        {
            int px = parentRoom.x;
            int py = parentRoom.y;
            int pw = parentRoom.width;
            int ph = parentRoom.height;
            int halfRD = (int) (pw + 1) / 2;

            Rooms.Add(new RectInt(px, py, halfRD, ph));
            Rooms.Add(new RectInt(px + halfRD - 1, py, pw-halfRD+1, ph));
            roomsChanged = true;
            splitVertically = false;
            Rooms.RemoveAt(step);
        } else
        {
            step++;
        }
    }

    void HorizontalSplit()
    {
        var parentRoom = Rooms[step];
        if (parentRoom.height >= 2*minRoomLength)
        {
            int px = parentRoom.x;
            int py = parentRoom.y;
            int pw = parentRoom.width;
            int ph = parentRoom.height;
            int halfRD = (int)(ph + 1) / 2;

            Rooms.Add(new RectInt(px, py, pw, halfRD));
            Rooms.Add(new RectInt(px, py + halfRD - 1, pw, ph-halfRD+1));
            roomsChanged = true;
            splitVertically = true;
            Rooms.RemoveAt(step);
        } else
        {
            step++;
        }
    }





}

