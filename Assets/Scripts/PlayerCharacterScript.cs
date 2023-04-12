using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCharacterScript : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public float velocity = 10;
    public bool jumpRight = true;
    public bool canJump = true;
    public int airJumps = 1;
    private int airJumpCount;
    public bool touchingWall = false;

    //public bool enterWall = false;
    bool enterWall
    {
        get
        {
            return enterWallCount > 0;
        }
    }
    private int enterWallCount = 0;


    public bool onFloor = false;

    public int defaultDrag = 0;
    public int wallDrag = 5;
    //public int upDrag = 0;

    private bool jumping = false;
    private float jumpingGrace = 0.1f;
    private float jumpingTimer = 0;

    public bool movingUp = false;

    /*
    public bool upwardSlide = false;
    public float upwardSlideGrace = 0.1f;
    private float upwardSlideTimer = 0;
    */

    //try: when press jump in midair, check if player touches the wall in the next small time window
    private bool jumpParry = false;
    private float jumpParryWindow = 0.1f;
    private float jumpParryTimer = 0;

    private bool superJumpParry = false;
    private float superJumpParryWindow = 0.05f;

    public float yVelocityCap = 60;//the velocity cap for when going up
    public float yVelocityMin = -40; //the velocity cap for when falling
    public float currentVelocity;

    //manager
    public GameObject manager;
    private Hitstop1 hitstop1;

    //smoke effect when jumping
    public GameObject jumpSmoke;

    //animations
    public Animator animator;

    //raycasts
    //public ContactFilter2D contactFilter;
    public LayerMask _layerMask;
    public bool raySurfaceBelow = false;

    // Start is called before the first frame update
    void Start()
    {
        airJumpCount = airJumps;
        jumpingTimer = jumpingGrace;
        jumpParryTimer = jumpParryWindow;

        manager = GameObject.FindGameObjectWithTag("Manager");
        hitstop1 = manager.GetComponent<Hitstop1>();
    }

    // Update is called once per frame
    void Update()
    {
        //can only jump if they are either touching a wall, floor, or if they have air jumps
        if (touchingWall || onFloor || airJumpCount > 0)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }

        //detect if the player is moving upwards, used for the upward wall slide
        if (myRigidbody.velocity.y>0)
        {
            movingUp = true;
        }
        else
        {
            movingUp = false;
        }
        //Debug.Log(myRigidbody.velocity.y);

        if (jumpingTimer < jumpingGrace)
        {
            jumpingTimer += Time.deltaTime;
            jumping = true;
            //Debug.Log("jumping");
        }
        else
        {
            jumping = false;
            //Debug.Log("not jumping");
        }

        if (jumpParryTimer < superJumpParryWindow)
        {
            superJumpParry = true;
        }
        else
        {
            superJumpParry = false;
        }

        if (jumpParryTimer<jumpParryWindow)
        {
            jumpParryTimer += Time.deltaTime;
            jumpParry = true;
        }
        else
        {
            jumpParry = false;
        }

        

        /*
        if(upwardSlideTimer<upwardSlideGrace)
        {
            upwardSlideTimer += Time.deltaTime;
            upwardSlide = true;
        }
        else
        {
            upwardSlide = false;
        }
        */

        //if touching wall, lower drop velocity. if not, return drag to normal
        if(touchingWall && !onFloor && !jumping && !movingUp)
        {
            //myRigidbody.AddForce(Vector2.up * 0.5f);

            myRigidbody.drag = wallDrag;

            myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);//make it so that the character doesnt move away from the wall, let's them "stick" to it
        }
        else
        {
            myRigidbody.drag = defaultDrag;

            //upward wall slide; BROKEN! NEEDS WORK; maybe check for not touching wall in a small window before it touches the wall
            //try: when press jump in midair, check if player touches the wall in the next small time window
            /*if(touchingWall && jumping && movingUp && !onFloor)
            {
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, velocity); //Vector2.up * velocity; //new Vector2(myRigidbody.velocity.x, 20); 
                //myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y + velocity);
            }*/

            if (touchingWall && jumpParry && movingUp && !onFloor)//jump parry upward wall slide
            {
                if (superJumpParry)
                {
                    myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y + (velocity * 4));
                    Debug.Log("SUPER jump parried!");


                    hitstop1.Freeze(0.2f);

                    //Debug.Log("finish hitstop");
                }
                else
                {
                    //myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, velocity * 1.5f); //Vector2.up * velocity; //new Vector2(myRigidbody.velocity.x, 20); 
                    myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y + (velocity * 2));
                    Debug.Log("jump parried!");

                    //histop tests here
                    hitstop1.Freeze();

                    //Debug.Log("finish hitstop");
                    
                }
                jumpParryTimer = jumpParryWindow;//reset jump parry after a successful parry
            }
        }

        //Debug.Log(myRigidbody.velocity);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canJump)
            {
                jumpingTimer = 0;

                //TODO: add a velocity cap; halve velocity on double jumps?
                //when sliding upwards, add to the velocity for momentum

                if(touchingWall)//for wall jumps
                {
                    if (movingUp)
                    {
                        //Debug.Log("momentum added!");
                        if (jumpRight)
                        {
                            myRigidbody.velocity = new Vector2(1 * velocity, myRigidbody.velocity.y + (velocity * .5f));
                        }
                        else
                        {
                            myRigidbody.velocity = new Vector2(-1 * velocity, myRigidbody.velocity.y + (velocity * .5f));
                        }
                    }
                    else
                    {
                        if (jumpRight)
                        {
                            myRigidbody.velocity = (Vector2.up + Vector2.right) * velocity;
                        }
                        else
                        {
                            myRigidbody.velocity = (Vector2.up + Vector2.left) * velocity;
                        }
                    }
                }
                else if(onFloor)//for floor jumps
                {
                    if (jumpRight)
                    {
                        myRigidbody.velocity = (Vector2.up + Vector2.right) * velocity;
                    }
                    else
                    {
                        myRigidbody.velocity = (Vector2.up + Vector2.left) * velocity;
                    }
                }
                else//for air jumps
                {
                    if (movingUp)
                    {
                        if (jumpRight)
                        {
                            //myRigidbody.velocity = new Vector2(velocity, velocity);
                            myRigidbody.velocity = new Vector2(velocity, myRigidbody.velocity.y + (velocity * 0.5f));
                        }
                        else
                        {
                            //myRigidbody.velocity = new Vector2(velocity * -1, velocity);
                            myRigidbody.velocity = new Vector2(velocity * -1, myRigidbody.velocity.y + (velocity * 0.5f));
                        }
                    }
                    else
                    {
                        if (jumpRight)
                        {
                            myRigidbody.velocity = (Vector2.up + Vector2.right) * velocity;
                        }
                        else
                        {
                            myRigidbody.velocity = (Vector2.up + Vector2.left) * velocity;
                        }
                    }
                }

                if (airJumpCount > 0 && !touchingWall)
                {
                    airJumpCount--;

                    //reset jump parry timer when you are air jumping
                    jumpParryTimer = 0;
                }

                //Instantiate(jumpSmoke, gameObject.transform);
                Instantiate(jumpSmoke, new Vector3(gameObject.transform.position.x,gameObject.transform.position.y,0),transform.rotation);
                //Debug.Log("spawned smoke");

                //jumpRight = !jumpRight;
                animator.SetTrigger("jumpTrigger");
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        //velocity cap
        if(myRigidbody.velocity.y>yVelocityCap)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, yVelocityCap);
        }
        //velocity min
        if(myRigidbody.velocity.y<yVelocityMin)
        {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, yVelocityMin);
        }

        currentVelocity = myRigidbody.velocity.y;//for debugging purposes; will display on character script
        //Debug.Log("velocity: " + myRigidbody.velocity);

        //determines where the character faces
        if(jumpRight)
        {
            gameObject.transform.rotation = new Quaternion(gameObject.transform.rotation.x, -180, gameObject.transform.rotation.z, gameObject.transform.rotation.w);
        }
        else
        {
            gameObject.transform.rotation = new Quaternion(gameObject.transform.rotation.x, 0, gameObject.transform.rotation.z, gameObject.transform.rotation.w);
        }

        //trying raycasts
        float rayDistance = 0.6f;
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.down) * rayDistance, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.down), rayDistance, _layerMask);

        if (hit)
        {
            raySurfaceBelow = true;
            Debug.Log("hit something " + hit.collider.name);
            //hit.transform.GetComponent
        }
        else
        {
            raySurfaceBelow = false;
            Debug.Log("no raycast hit");
        }

        //air animations
        if (!(onFloor || raySurfaceBelow))
        {
            animator.SetBool("jumping", true);
        }
        else
        {
            animator.SetBool("jumping", false);
        }

        if((enterWall || touchingWall) && myRigidbody.velocity.y!=0)//!onFloor)
        {
            animator.SetBool("climbing", true);
        }
        else
        {
            animator.SetBool("climbing", false);
        }

        if(myRigidbody.velocity.y<0 && !(onFloor || raySurfaceBelow))
        {
            animator.SetBool("falling", true);
        }
        else
        {
            animator.SetBool("falling", false);
        }

        if(onFloor || raySurfaceBelow)
        {
            //set animation onfloor to true
            animator.SetBool("onFloor", true);
        }
        else
        {
            animator.SetBool("onFloor", false);
        }

        
    }

    private void OnTriggerStay2D(Collider2D collision)//maybe use this as a replacement for collisionstay, since we can have a trigger hitbox bigger than the collider, it may avoid the not touching wall because it bounced out issue
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //Debug.Log("enter wall");
            touchingWall = true;

            if (transform.position.x < collision.gameObject.transform.position.x)
            {
                jumpRight = false;
            }
            else if (transform.position.x > collision.gameObject.transform.position.x)
            {
                jumpRight = true;
            }

            //TODO:check to see if player is on top of a wall? should probably do a more specific check, like take the object size into consideration to actual make sure where we are in relation to it
            //collision.gameObject.GetComponent<>
            //if (transform.position.y < collision.gameObject.transform.position.y - collision.gameObject.transform.height)
            /*if(collision.gameObject.GetComponent<Collision2D>().contacts[0].normal == Vector2.up)
            {
                
                Debug.Log("on top of object");
            }*/
            /*
            RaycastHit2D[] hits = new RaycastHit2D[0];
            if(collision.Raycast(Vector2.up, hits)>0)
            {
                Debug.Log("raycasting");
                if (hits[0].collider.gameObject.CompareTag("Player"))
                {
                    Debug.Log("player on top");
                }
            }*/
            //NOTE: try raycasts
        }

        if (collision.gameObject.CompareTag("Floor"))
        {
            onFloor = true;
        }
        //Debug.Log("stay collision");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            //Debug.Log("hit a wall");

            //enterWall = true;
            enterWallCount++;

            /*
            if (transform.position.x < collision.gameObject.transform.position.x)
            {
                jumpRight = false;
            }
            else if (transform.position.x > collision.gameObject.transform.position.x)
            {
                jumpRight = true;
            }
            */

            //TODO:CLING TO WALL

            //upwardSlideTimer = 0;
        }
        //Debug.Log("enter collision");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {


        if (collision.gameObject.CompareTag("Wall"))
        {
            //enterWall = false;
            enterWallCount--;
            airJumpCount = airJumps;

            //todo:check if object is still touching wall so that if they exit a wall but is actually still touching another, it wont falsely say that theyre not touching a wall
            //Debug.Log("exit wall");
            touchingWall = false;
        }

        if (collision.gameObject.CompareTag("Floor"))
        {
            airJumpCount = airJumps;
            onFloor = false;
        }
        //Debug.Log("exit collision");
    }
}
