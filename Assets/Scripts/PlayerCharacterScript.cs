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
    public int airJumpCount;
    public bool touchingWall = false;

    //public bool enterWall = false;
    public bool enterWall
    {
        get
        {
            return enterWallCount > 0;
        }
    }
    public int enterWallCount = 0;


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

    private float lateParryTimer = 0;
    private bool lateParry = false;
    private bool superLateParry = false;
    private float lateParryWindow = 0.05f;
    private float superLateParryWindow = 0.25f;

    public float yVelocityCap = 40;//the velocity cap for when going up
    public float yVelocityMin = -40; //the velocity cap for when falling
    public Vector2 currentVelocity;

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
    public bool raySurfaceAbove = false;
    public bool raySurfaceLeft = false;
    public bool raySurfaceRight = false;

    bool onSurface = false;

    //UI stuff
    public GameUI gameUI;

    // Start is called before the first frame update
    void Start()
    {
        airJumpCount = airJumps;
        jumpingTimer = jumpingGrace;
        jumpParryTimer = jumpParryWindow;

        manager = GameObject.FindGameObjectWithTag("Manager");
        hitstop1 = manager.GetComponent<Hitstop1>();

        gameUI = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameUI>();
    }

    // Update is called once per frame
    void Update()
    {

        

        //touchingWall = enterWall;
        //Debug.Log("enter wall: " + enterWall + ", touching wall: " + touchingWall);

        //determines where the character faces
        if (jumpRight)
        {
            gameObject.transform.rotation = new Quaternion(gameObject.transform.rotation.x, -180, gameObject.transform.rotation.z, gameObject.transform.rotation.w);
            //gameObject.GetComponentInChildren<SpriteRenderer>().flipX = true;
            //temp.flipX = true;
        }
        else
        {
            gameObject.transform.rotation = new Quaternion(gameObject.transform.rotation.x, 0, gameObject.transform.rotation.z, gameObject.transform.rotation.w);
            //gameObject.GetComponentInChildren<SpriteRenderer>().flipX = false;
            //temp.flipX = false;
        }

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

        //jump parries for late parries, when the player parries after touching a wall
        if(touchingWall)
        {
            if(lateParryTimer < superLateParryWindow)
            {
                superLateParry = true;
            }
            else
            {
                superLateParry = false;
            }

            if (lateParryTimer < lateParryWindow)
            {
                lateParryTimer += Time.deltaTime;//only count up if touching wall
                lateParry = true;
            }
            else
            {
                lateParry = false;
            }
        }
        else
        {
            //reset late parry timer if not touching a wall
            lateParryTimer = 0;
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

            if (raySurfaceAbove)
            {//if they are bumping on a ceiling, dont halt horizontal movement

            }
            else
            {
                myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);//make it so that the character doesnt move away from the wall, let's them "stick" to it
                //Debug.Log("freezing horizontal movement");
            }
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

            if (touchingWall && (jumpParry /*|| lateParry*/) && movingUp && !onFloor)//jump parry upward wall slide
            {
                if (superJumpParry /*|| superLateParry*/)
                {
                    myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y + (velocity * 2));
                    Debug.Log("SUPER jump parried!");


                    hitstop1.Freeze(0.2f);

                    //Debug.Log("finish hitstop");
                }
                else
                {
                    //myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, velocity * 1.5f); //Vector2.up * velocity; //new Vector2(myRigidbody.velocity.x, 20); 
                    myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y + (velocity * 1));
                    Debug.Log("jump parried!");

                    //histop tests here
                    hitstop1.Freeze();

                    //Debug.Log("finish hitstop");
                    
                }
                jumpParryTimer = jumpParryWindow;//reset jump parry after a successful parry
                lateParryTimer = jumpParryWindow;//reset late jump parry to make sure that you cant late jump parry after a successful parry
            }
        }

        bool playerInput = false;

        //touch input code here
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            
            if (Input.touches[0].rawPosition.x <= Screen.width / 2f)
            {
                jumpRight = false;
                playerInput = true;
                Debug.Log("touched left side of screen");
            }
            else if (Input.touches[0].rawPosition.x > Screen.width / 2f)
            {
                jumpRight = true;
                playerInput = true;
                Debug.Log("touched right side of screen");
            }
            

            //code for no directional jumps
            //playerInput = true;
        }

#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.x <= Screen.width / 2f)
            {
                jumpRight = false;
                playerInput = true;
                Debug.Log("touched left side of screen");
            }
            else if (Input.mousePosition.x > Screen.width / 2f)
            {
                jumpRight = true;
                playerInput = true;
                Debug.Log("touched right side of screen");
            }
        }

#endif

        //can use this to switch jump directions
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            jumpRight = true;
            playerInput = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            jumpRight = false;
            playerInput = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit") || playerInput)
        {
            if (canJump)
            {
                jumpingTimer = 0;

                //TODO: add a velocity cap; halve velocity on double jumps?
                //when sliding upwards, add to the velocity for momentum

                if(touchingWall)//for wall jumps
                {

                    //late jump parry
                    if(lateParry)
                    {
                        if(superLateParry)
                        {
                            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y + (velocity * 2));
                            Debug.Log("SUPER late jump parried!");


                            hitstop1.Freeze(0.2f);
                        }
                        else
                        {
                            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y + (velocity * 1));
                            Debug.Log("late jump parried!");

                            hitstop1.Freeze();
                        }
                        lateParryTimer = jumpParryWindow;//reset timer to window so that you cant parry again
                        jumpParryTimer = jumpParryWindow;//reset jump parry
                    }
                    else if (movingUp)//else if not a late parry, it's a normal wall jump
                    {
                        //add momentum
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

        currentVelocity = myRigidbody.velocity;//for debugging purposes; will display on character script
        gameUI.updateSpeed(myRigidbody.velocity.y);


        //trying raycasts
        float rayDistance = 0.6f;
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.down) * rayDistance, Color.red);

        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.down), rayDistance, _layerMask);//raycast used to check for surfaces below character

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * rayDistance, Color.blue);

        if (hitDown)
        {
            raySurfaceBelow = true;
            //Debug.Log("hit something down " + hitDown.collider.name);
            //hit.transform.GetComponent
        }
        else
        {
            raySurfaceBelow = false;
            //Debug.Log("no raycast hit");
        }

        Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * rayDistance, Color.blue);

        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.up), rayDistance, _layerMask);//raycast used to check for surfaces above character

        if (hitUp)
        {
            raySurfaceAbove = true;
            //Debug.Log("hit something up " + hitUp.collider.name);
        }
        else
        {
            raySurfaceAbove = false;
        }

        Debug.DrawRay(transform.position, Vector2.left * rayDistance, Color.green);

        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, rayDistance, _layerMask);//raycast used to check for surfaces above character

        if (hitLeft)
        {
            raySurfaceLeft = true;
            //Debug.Log("hit something left " + hitLeft.collider.name);
        }
        else
        {
            raySurfaceLeft= false;
        }

        Debug.DrawRay(transform.position, Vector2.right * rayDistance, Color.yellow);

        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, rayDistance, _layerMask);//raycast used to check for surfaces above character

        if (hitRight)
        {
            raySurfaceRight = true;
            //Debug.Log("hit something right " + hitRight.collider.name);
        }
        else
        {
            raySurfaceRight = false;
        }

        //combine both onFloor and raySurfaceBelow to a new onSurface bool
        if (onFloor || raySurfaceBelow)
        {
            onSurface = true;
        }
        else
        {
            onSurface = false;
        }

        //air animations
        if (!onSurface)
        {
            animator.SetBool("jumping", true);
        }
        else
        {
            animator.SetBool("jumping", false);
        }

        if((enterWall || touchingWall) && myRigidbody.velocity.y!=0 && !raySurfaceAbove)//!onFloor)
        {
            animator.SetBool("climbing", true);
        }
        else
        {
            animator.SetBool("climbing", false);
        }

        if(myRigidbody.velocity.y<0 && !onSurface)
        {
            animator.SetBool("falling", true);
        }
        else
        {
            animator.SetBool("falling", false);
        }

        if(onSurface)
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

            
            if (onSurface || raySurfaceAbove)
            {
                //the player is on top of a surface or if they hit a ceiling, do nothing
                //Debug.Log("player on surface or hit a ceiling");
            }
            else if (raySurfaceRight) //(transform.position.x < collision.gameObject.transform.position.x)
            {
                jumpRight = false;
                //Debug.Log("surface to the right, jumpRight = false");
            }
            else if (raySurfaceLeft) //(transform.position.x > collision.gameObject.transform.position.x)
            {
                jumpRight = true;
                //Debug.Log("surface to the left, jumpRight = true");
            }
            

            /*
            if (onSurface || raySurfaceAbove)
            {
                //the player is on top of a surface or if they hit a ceiling, do nothing
                //Debug.Log("player on surface or hit a ceiling");
            }
            else if (transform.position.x < collision.gameObject.transform.position.x)
            {
                jumpRight = false;
            }
            else if (transform.position.x > collision.gameObject.transform.position.x)
            {
                jumpRight = true;
            }*/

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

            if (raySurfaceAbove)
            {
                //dont reset airjumps if player bumps a ceiling
            }
            else
            {
                airJumpCount = airJumps;
            }

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
