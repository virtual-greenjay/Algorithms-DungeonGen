using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;
using NaughtyAttributes;
using UnityEditor;
using Unity.AI.Navigation;
using UnityEngine.Events;

public class DungeonGenerator : MonoBehaviour
{

    [SerializeField]
    private NavMeshSurface navMeshSurface;

    [SerializeField]
    private UnityEvent dungeonGen;

    public int dungeonWidth = 100;
    public int dungeonHeight = 60;
    public int minRoomLength = 10;
    public bool roomsChanged = false;

    public int dungeonSeed = 20;
    public bool useDungeonSeed = true;

    public bool isGenerating = false;
    public bool isPaused = false; // to add: pause functionality.

    public bool isAnimated = true; // to add: toggle for animation on/off.

    private Random.State stateBeforeGen;

    RectInt dungeonRoom;

    List<RectInt> Rooms = new List<RectInt>();
    List<RectInt> Doors = new List<RectInt>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dungeonRoom = new RectInt(0, 0, dungeonWidth, dungeonHeight);
        Rooms.Add(new RectInt(0, 0, dungeonWidth, dungeonHeight));

    }

    // Update is called once per frame
    void Update()
    {
        //draws a minimum size reference room. 
        RectInt tinyRoom = new RectInt(-20, -20, minRoomLength, minRoomLength);
        AlgorithmsUtils.DebugRectInt(tinyRoom, Color.blue);
        //draws dungeon bounds.
        AlgorithmsUtils.DebugRectInt(dungeonRoom, Color.blue);

        //draw the generated rooms.
        for (int i = 0; i < Rooms.Count; i++)
        {

            AlgorithmsUtils.DebugRectInt(Rooms[i], Color.red);
        }

        // draw generated doors.
        foreach (RectInt thisDoor in Doors)
        {
            AlgorithmsUtils.DebugRectInt(thisDoor, Color.cyan);
        }

        //here: draw "active" (highlighted) rooms. 

        // not necessary now that I have button, but can also start the process with spacebar.
        if (Input.GetKeyUp(KeyCode.Space) && !isGenerating)
        {
            StartCoroutine(GenerateDungeon());
        }
    }

    [Button]
    IEnumerator CreateRandomSequence()
    {
        //for seed testing. 
        if (useDungeonSeed)
        {
            Random.InitState(dungeonSeed);
        }

        int counter = 10;
        while (counter > 0)
        {
            Random.InitState(dungeonSeed);
            int randomNumber = Random.Range(0, 100);
            Debug.Log("Random number: " + randomNumber);
            randomNumber = Random.Range(0, 100);
            Debug.Log("Random number: " + randomNumber);
            counter--;

            yield return null;
        }
        
    }

    //Creates the flat outline of the dungeon. Animated.
    [Button]
    IEnumerator GenerateDungeon()
    {
        isGenerating = true;

        if (useDungeonSeed)
        {
            //only works when animations are turned off. For some reason. >:(
            Random.InitState(dungeonSeed);
            
        }

        Queue<RectInt> roomQueue = new Queue<RectInt>(Rooms); //create a queue and add all rooms to it. 

        while (roomQueue.Count > 0)
        {
            RectInt currentRoom = roomQueue.Dequeue();

            bool widthCheck = currentRoom.width >= 2 * minRoomLength;
            bool heightCheck = currentRoom.height >= 2 * minRoomLength;

            if (!widthCheck && !heightCheck)
            {
                continue; // if room is too small to split, skip it and go to next room in queue
            }
            
            if (isAnimated)
                yield return new WaitForSeconds(0.3f); // wait for a short time to create an animation effect

            //set up variables for room splitting. 
            int px = currentRoom.x;
            int py = currentRoom.y;
            int pw = currentRoom.width;
            int ph = currentRoom.height;

            bool splitVertically = false;
            if (widthCheck && !heightCheck) // if wide enough, split vertically
            {
                splitVertically = true;
            }
            else if (heightCheck && !widthCheck) // else if tall enough split horizontally
            {
                splitVertically = false;
            }
            else if (heightCheck && widthCheck) //if both, randomly choose which way to split
            {

                float r = Random.value;
                if (r >= 0.5f)
                {
                    splitVertically = true;
                }
                else
                {
                    splitVertically = false;
                }
            }

            RectInt childA;
            RectInt childB;

            if (splitVertically)
            {
                int aWidth = Random.Range(minRoomLength, pw - minRoomLength + 1);

                childA = new RectInt(px, py, aWidth, ph);
                childB = new RectInt(px + aWidth - 1, py, pw - aWidth + 1, ph);

            }
            else
            {
                int aHeight = Random.Range(minRoomLength, ph - minRoomLength + 1);

                childA = new RectInt(px, py, pw, aHeight);
                childB = new RectInt(px, py + aHeight - 1, pw, ph - aHeight + 1);
                
            }

            // add child rooms to queue for further splitting
            roomQueue.Enqueue(childA);
            roomQueue.Enqueue(childB);

            // replace current room with child rooms in Rooms list
            int index = Rooms.IndexOf(currentRoom);
            Rooms.RemoveAt(index);
            Rooms.Add(childA);
            Rooms.Add(childB);

        }

        Debug.Log("Room generation complete.");

        //this is where code goes for removing rooms. 

  
        StartCoroutine(GenerateDungeonDoors());



    }

    IEnumerator GenerateDungeonDoors()
    {

        for (int i = 0; i < Rooms.Count - 1; i++)
        {
            for (int j = i + 1; j < Rooms.Count; j++)
            {

                //highlight currently searching rooms (not working right but whatever.) (to do: push to DebugDrawingBatcher and call in Update).
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
                            if (isAnimated) 
                                yield return new WaitForSeconds(0.05f);
                        }
                    }
                    else
                    {
                        // if width intersection < 3, skip 
                        if (intersectionRoom.width >= 3)
                        {
                            int randomOffset = (int)Random.Range(1, intersectionRoom.width - 1);
                            DoorRect(intersectionRoom.x + randomOffset, intersectionRoom.y);
                            if (isAnimated) 
                                yield return new WaitForSeconds(0.05f);
                        }
                    }

                }
            }
        }

        Debug.Log("Door generation complete.");
        dungeonGen.Invoke();

    }

    [Button]
    public void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();

        Debug.Log("NavMesh baked. Generation complete.");
        isGenerating = false;
    }

    void DoorRect(int doorx, int doory)
    {
        Doors.Add(new RectInt(doorx, doory, 1, 1));
    }


    

    [Button]
    void ResetGeneration()
    {
        StopAllCoroutines();
        isGenerating = false;

        //empty Rooms array and set it back to initial room. 
        Rooms = new List<RectInt>();
        Rooms.Add(new RectInt(0, 0, dungeonWidth, dungeonHeight));
        //empty Doors array
        Doors = new List<RectInt>();

        //should also clean up walls/floors/navmesh. 

        GameObject walls = GameObject.Find("Walls");
        GameObject floor = GameObject.Find("Floors");

        Destroy(walls);
        Destroy(floor);
        navMeshSurface.RemoveData();

    }

    public RectInt GetDungeonBounds()
    {
        return dungeonRoom;
    }

    public List<RectInt> GetRooms()
    {
        return Rooms;
    }

    public List<RectInt> GetDoors()
    {
        return Doors;
    }







}

