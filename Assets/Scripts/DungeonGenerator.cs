using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;

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

    //WIP animated Dungeon.
    IEnumerator GenerateDungeon()
    {
        bool splitFurther = true;
        yield return null;

        while (splitFurther)
        {
            splitFurther = false;
            int roomCount = Rooms.Count; //set loop length at current nr of rooms so all rooms only get checked once
            int ri = 0; //room index
            for (int i=0; i<roomCount; i++)
            {
                var parentRoom = Rooms[ri];
                bool widthCheck = parentRoom.width >= 2 * minRoomLength;
                bool heightCheck = parentRoom.height >= 2 * minRoomLength;

                if (splitVertically)
                {
                    if (widthCheck)
                    {
                        VerticalSplit(parentRoom);
                        splitFurther = true;
                    } 
                    else if (heightCheck)
                    {
                        HorizontalSplit(parentRoom);
                        splitFurther = true;
                    }
                    else
                    {
                        ri++;
                    }
                    splitVertically = false;
                } else
                {
                    if (heightCheck)
                    {
                        HorizontalSplit();
                        splitFurther = true;
                    }
                    else if (widthCheck)
                    {
                        VerticalSplit();
                        splitFurther = true;
                    } else
                    {
                        ri++;
                    }
                    splitVertically = true;
                }

            }
        }
        Debug.Log("Generation complete.");
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
            //int halfRD = (int) (pw + 1) / 2; //half-room division
            int aWidth = Random.Range(minRoomLength, pw - minRoomLength + 1);

            Rooms.Add(new RectInt(px, py, aWidth, ph));
            Rooms.Add(new RectInt(px + aWidth - 1, py, pw-aWidth+1, ph));
            roomsChanged = true;
            
            Rooms.RemoveAt(step);
        } else
        {
            step++;
        }
        //splitVertically = false;
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
            //int halfRD = (int)(ph + 1) / 2; //half-room division
            int aHeight = Random.Range(minRoomLength, ph - minRoomLength + 1);

            Rooms.Add(new RectInt(px, py, pw, aHeight));
            Rooms.Add(new RectInt(px, py + aHeight - 1, pw, ph-aHeight +1));
            roomsChanged = true;
            
            Rooms.RemoveAt(step);
        } else
        {
            step++;
        }
        //splitVertically = true;
    }





}

