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

    int ri = 0; //room index

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rooms.Add(new RectInt(0, 0, dungeonWidth, dungeonHeight));

    }

    // Update is called once per frame
    void Update()
    {
        RectInt tinyRoom = new RectInt(-20, -20, minRoomLength, minRoomLength);
        AlgorithmsUtils.DebugRectInt(tinyRoom, Color.blue);

        AlgorithmsUtils.DebugRectInt(dungeonRoom, Color.blue); 
        for (int i=0; i<Rooms.Count; i++)
        {
            Color roomDepth = new Color(1, i * 0.02f, 0, 1);
            AlgorithmsUtils.DebugRectInt(Rooms[i], roomDepth);
        }
        
        if(Input.GetKeyUp(KeyCode.Space))
        {
            StartCoroutine(GenerateDungeon());
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
            int roomCount = Rooms.Count; //set FOR loop length at current nr of rooms so all rooms only get checked once per WHILE loop
            ri = 0; //reset room index
            for (int i=0; i<roomCount; i++)
            {
                var parentRoom = Rooms[ri];
                bool widthCheck = parentRoom.width >= 2 * minRoomLength;
                bool heightCheck = parentRoom.height >= 2 * minRoomLength;

                if (splitVertically)
                {
                    if (widthCheck) // if wide enough, split vertically
                    {
                        yield return new WaitForSeconds(0.3f);
                        VerticalSplit(parentRoom);
                        splitFurther = true;
                    } 
                    else if (heightCheck) // else if tall enough split horizontally
                    {
                        yield return new WaitForSeconds(0.3f);
                        HorizontalSplit(parentRoom);
                        splitFurther = true;
                    }
                    else // move on to next room
                    {
                        ri++;
                    }
                    splitVertically = false;
                } else // same as above but vice versa.
                {
                    if (heightCheck)
                    {
                        yield return new WaitForSeconds(0.3f);
                        HorizontalSplit(parentRoom);
                        splitFurther = true;
                    }
                    else if (widthCheck)
                    {
                        yield return new WaitForSeconds(0.3f);
                        VerticalSplit(parentRoom);
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

        //generate doors
        //find walls: for each room, check which rooms are adjacent by checking if they overlap.
        //use room coordinates to determine if the adjacent rooms are on the north, east, south or west side. 
        // generate a door somewhere along the correct edge, exclude the parts near the corners. 
    }


    void VerticalSplit(RectInt pr)
    {
      
            int px = pr.x;
            int py = pr.y;
            int pw = pr.width;
            int ph = pr.height;
            //int halfRD = (int) (pw + 1) / 2; //half-room division
            int aWidth = Random.Range(minRoomLength, pw - minRoomLength + 1);

            Rooms.Add(new RectInt(px, py, aWidth, ph));
            Rooms.Add(new RectInt(px + aWidth - 1, py, pw-aWidth+1, ph));
            roomsChanged = true;
            
            Rooms.RemoveAt(ri);
    }

    void HorizontalSplit(RectInt pr)
    {

            int px = pr.x;
            int py = pr.y;
            int pw = pr.width;
            int ph = pr.height;
            //int halfRD = (int)(ph + 1) / 2; //half-room division
            int aHeight = Random.Range(minRoomLength, ph - minRoomLength + 1);

            Rooms.Add(new RectInt(px, py, pw, aHeight));
            Rooms.Add(new RectInt(px, py + aHeight - 1, pw, ph-aHeight +1));
            roomsChanged = true;
            
            Rooms.RemoveAt(ri);

    }





}

