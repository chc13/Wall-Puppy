using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    public GameObject walls;
    public int wallCount = 0;

    public GameObject player;
    float playerPos;
    float highestPlayerPos;

    // Start is called before the first frame update
    void Start()
    {
        highestPlayerPos = 0;
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
            Debug.Log("spawn wall");
            spawnWalls();
        }
    }

    void spawnWalls()
    {
        //Instantiate(pipe, new Vector3(transform.position.x,Random.Range(lowestPoint,highestPoint),0), transform.rotation);

        float ypos = wallCount * 100;
        Instantiate(walls, new Vector3(0, ypos, 0), transform.rotation);

        wallCount++;
    }
}
