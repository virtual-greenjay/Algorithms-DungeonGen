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
    public bool addDoors = true;

    RectInt dungeonRoom = new RectInt(0, 0, 100, 60);

    List<RectInt> Rooms = new List<RectInt>();
    List<RectInt> Doors = new List<RectInt>();

    int ri = 0; //room index

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rooms.Add(new RectInt(0, 0, dungeonWidth, dungeonHeight));

    }

    // Update is called once per frame
    void Update()
    {
        //draws a minimum size reference room. 
        RectInt tinyRoom = new RectInt(-20, -20, minRoomLength, minRoomLength);
        AlgorithmsUtils.DebugRectInt(tinyRoom, Color.blue);
        AlgorithmsUtils.DebugRectInt(dungeonRoom, Color.blue); 

        //draw the generated rooms.
        for (int i=0; i<Rooms.Count; i++)
        {
            //Color roomDepth = new Color(1, i * 0.02f, 0, 1);
            //AlgorithmsUtils.DebugRectInt(Rooms[i], roomDepth);
            AlgorithmsUtils.DebugRectInt(Rooms[i], Color.red);
        }
        
        foreach (RectInt thisDoor in Doors)
        {
            AlgorithmsUtils.DebugRectInt(thisDoor, Color.cyan);
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
        Debug.Log("Room generation complete.");

        if (addDoors)
        {
            StartCoroutine(GenerateDungeonDoors());
        }

        
    }

    IEnumerator GenerateDungeonDoors()
    {
        yield return null;
        

        for (int i = 0; i<Rooms.Count-1; i++)
        {
            for(int j=i+1; j<Rooms.Count; j++)
            {
                //not working correctly yet; current code will generate doors in corners. test with steps and breakpoints.

                yield return new WaitForSeconds(0.1f);

                //highlight currently searching rooms
                AlgorithmsUtils.DebugRectInt(Rooms[i], Color.green);
                AlgorithmsUtils.DebugRectInt(Rooms[j], Color.yellow);

                if (AlgorithmsUtils.Intersects(Rooms[i], Rooms[j]))
                {
                    bool isVertical = false;
                    RectInt intersectionRoom = AlgorithmsUtils.Intersect(Rooms[i], Rooms[j]);
                    //checks if overlap is horizontal or vertical
                    if (intersectionRoom.width == 1) isVertical = true;

                    if (isVertical)
                    {
                        // if height of intersection less than 3, skip (to prevent doors on corner intersects)
                        if (intersectionRoom.height >= 3) 
                        { 
                            int randomOffset = (int)Random.Range(1, intersectionRoom.height - 1);
                            DoorRect(intersectionRoom.x, intersectionRoom.y + randomOffset);
                        }
                    } else
                    {
                        // if width intersection < 3, skip 
                        if (intersectionRoom.width >= 3)
                        {
                            int randomOffset = (int)Random.Range(1, intersectionRoom.width - 1);
                            DoorRect(intersectionRoom.x + randomOffset, intersectionRoom.y);
                        }
                    }

                }
            }
        }

        //pseudocode: generate doors
        //find walls: for each room, check which rooms are adjacent by checking if they overlap.
        // find if there's already a door connecting these two rooms (no, just check only the rooms AFTER itself in the list, more efficient)
        //in the intersection, check which side has length 1, then generate a random int in the other direction between x+1, x+length-2
        // draw door of size 1.1 at the generated position along the side. 

        Debug.Log("Door generation complete.");

    }

    void DoorRect(int doorx, int doory)
    {
        Doors.Add(new RectInt(doorx, doory, 1, 1));
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

