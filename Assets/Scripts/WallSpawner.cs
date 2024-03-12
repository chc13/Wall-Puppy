using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    public GameObject walls;

    public GameObject[] wallLibrary;

    //wall libraries for individual left and right pieces
    public GameObject[] wallLibraryLeft;
    public GameObject[] wallLibraryRight;

    public int wallCount = 0;

    public GameObject player;
    float playerPos;
    float highestPlayerPos;

    private int lastSpawned =-1;//use to indicate which wall was spawned last

    // Start is called before the first frame update
    void Start()
    {
        highestPlayerPos = 0;

        //Debug.Log("wall library size: " + wallLibrary.Length);
        spawnWalls(0);//spawn the very first wall in the library
        //Debug.Log("spawn first wall");
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = player.gameObject.transform.position.y;

        if(playerPos>highestPlayerPos)
        {
            highestPlayerPos = playerPos;
        }

        if(highestPlayerPos>((wallCount+1)*100)-100)//(wallCount+1)*100)
        {
            Debug.Log("spawn wall " + wallCount);
            //spawnWalls();
            spawnWallsAsym();
        }
    }

    void spawnWalls()
    {
        //Instantiate(pipe, new Vector3(transform.position.x,Random.Range(lowestPoint,highestPoint),0), transform.rotation);

        GameObject temp;

        int roll = Random.Range(0, wallLibrary.Length);

        if(roll==lastSpawned)//if the first roll is the same as the last wall spawned, reroll
        {
            roll = Random.Range(0, wallLibrary.Length);
        }

        temp = wallLibrary[roll];

        float ypos = wallCount * 100;
        //Instantiate(walls, new Vector3(0, ypos, 0), transform.rotation);
        Instantiate(temp, new Vector3(0, ypos, 0), transform.rotation);

        lastSpawned = roll;

        wallCount++;
    }

    void spawnWalls(int x)
    {
        GameObject temp;
        temp = wallLibrary[x];

        float ypos = wallCount * 100;
        //Instantiate(walls, new Vector3(0, ypos, 0), transform.rotation);
        Instantiate(temp, new Vector3(0, ypos, 0), transform.rotation);

        lastSpawned = x;

        wallCount++;
    }

    void spawnWallsAsym()//spawn asymmetrical walls
    {
        GameObject leftTemp;
        GameObject rightTemp;

        int rollLeft = Random.Range(0, wallLibraryLeft.Length);
        int rollRight = Random.Range(0, wallLibraryRight.Length);

        leftTemp = wallLibraryLeft[rollLeft];
        rightTemp = wallLibraryRight[rollRight];

        float ypos = wallCount * 100;

        Instantiate(leftTemp, new Vector3(0, ypos, 0), transform.rotation);
        Instantiate(rightTemp, new Vector3(0, ypos, 0), transform.rotation);

        wallCount++;
    }
}
