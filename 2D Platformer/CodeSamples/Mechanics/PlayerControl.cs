//////////////////////////////////////////////////////////////////
// File Name: PlayerControl.cs                                  //
// Author: Josh Godsell                                         //
// Date Created: 25/1/19                                        //
// Date Last Edited: 30/5/19                                    //
// Brief: Class containing player control functionality         //
//////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    //Game manager
    private GameManager m_GameManager;

    //Player Head
    private GameObject m_playerHead;

    //the audiosource attached to the player
    private AudioSource m_AudioSource;

    //all the audio clips for the players actions
    public AudioClip m_Jump;
    public AudioClip m_JumpBoost;
    public AudioClip m_Shoot;
    public AudioClip m_Death;

    //floats for player Movement calculations
    public float m_fJumpStrength;
    public float m_fAcceleration;
    public float m_fMaxVelocity;
    public float m_fCurrrentMaxVelocity;
    public float m_fVelocityOnContact;

    //number of times player can jump
    [SerializeField]
    private int m_iJumps = 2;
    private int m_iMaxJumps = 2;

    //number of bullets player has 
    private int m_iBullets = 5;

    //array of player bullets
    private PlayerBullet[] m_aPlayerBullets;

    //fire delay
    public float m_fFireDelay = 0.5f;
    //if player can fire
    private bool m_bCanFire = true;

    //Player Respawn Position
    private Vector3 m_v3RespawnPos;

    //Player RigidBody
    private Rigidbody2D m_PlayerRigidBody;

    //Player Facing Direction
    private Vector2 m_v2PlayerFacingDir;

    //bool for if player is in contact with the ground
    [SerializeField]
    private bool m_bGrounded;

    public Room m_currentRoom;
    private Transform m_platform;
    private Vector3 offset;

    //bool for if player is in contact with a wall
    [SerializeField]
    private bool m_bWallContact;

    //bool for if the player has stopped on each axis
    [SerializeField]
    private bool m_bStoppedOnX;
    [SerializeField]
    private bool m_bStoppedOnY;

    //the levels boss controller
    private BossController m_Boss;

    //the animator component for the player
    private Animator m_Animator;

    //bools for animations
    private bool m_bIsMoving;
    private bool m_bIsJumping;
    private bool m_bIsFalling;
    private bool m_bIsShooting;
    private bool m_bIsCrouching;

    // Use this for initialization
    void Start ()
    {

        //try to find the rigid body component
        try
        {
            m_PlayerRigidBody = this.transform.GetComponent<Rigidbody2D>();

        }
        catch (System.Exception)
        {

            throw new System.Exception("RigidBody2D not found attached to player");
        }

        //try to assign the animator,audiosource and boss controller
        try
        {
            m_Animator = GetComponent<Animator>();

            m_AudioSource = GetComponent<AudioSource>();

            m_Boss = FindObjectOfType<BossController>();
        }
        catch (System.Exception)
        {

            throw;
        }

        //try and find the game manager 
        try
        {
            m_GameManager = FindObjectOfType<GameManager>();
        }
        catch (System.Exception)
        {

            throw new System.Exception("Game manager not found");
        }

        //find the head object of the player
        foreach(Transform child in this.transform)
        {
            if(child.name == "Head")
            {
                m_playerHead = child.gameObject;
            }
        }

        //setup the players bullets array and initialise them
        m_aPlayerBullets = new PlayerBullet[m_iBullets];

        for(int i = 0; i < m_aPlayerBullets.Length; i++)
        {
            m_aPlayerBullets[i] = new PlayerBullet(this.gameObject);
        }

        //get the starting looking direction and respawn position
        m_v2PlayerFacingDir = transform.right;
        m_v3RespawnPos = transform.position;
	}

    private void FixedUpdate()
    {
        //when stopped on x and grounded
        if (m_bStoppedOnX && m_bGrounded)
        {
            HandlePlatformX();
        }
        //when stopped on y
        if (m_bStoppedOnY)
        {
            HandlePlatformY();
        }

        //check if the player is in contact with a wall and the ground
        m_bGrounded = GroundCheck();
        m_bWallContact = WallContactCheck();

        //if the player is grounded 
        if (m_bGrounded)
        {
            //and is on a platform
            if (m_platform != null)
            {
                //check for iceSurface if it is present assign the drag from the ice surface to the player 
                if (m_platform.GetComponent<IceSurface>() != null)
                {
                    m_PlayerRigidBody.drag = m_platform.GetComponent<IceSurface>().m_fDrag;
                }
                else
                {
                    //otherwise assign the normal drag of 3
                    m_PlayerRigidBody.drag = 3.0f;
                }
            }
        }
        else
        {
            //if in the air drag is 0
            m_PlayerRigidBody.drag = 0.0f;

        }

    }

    // Update is called once per frame
    void Update ()
    {
        //calculate for gravity and  animate the player
        GravityCalc();
        Animate();

        //Calls the Horizontal movement on Key Press Function each frame
        OnPressMove();
        OnPressCrouch();
        OnPressJump();
        OnPressFire();

        //check if the player is stopped
        CheckStopped();

    }

    //function for controlling the animator test bools
    public void Animate()
    {
        if (m_bIsMoving && m_bGrounded)
        {
            m_Animator.SetBool("IsMoving", true);
        }
        else
        {
            m_Animator.SetBool("IsMoving", false);

        }

        if(m_bGrounded)
        {
            m_Animator.SetBool("IsGrounded", true);

        }
        else
        {
            m_Animator.SetBool("IsGrounded", false);

        }

        if(m_bIsCrouching)
        {
            m_Animator.SetBool("IsCrouched", true);

        }
        else
        {
            m_Animator.SetBool("IsCrouched", false);

        }
    }
    //function to play an audio clip
    public void PlayAudio(AudioClip a_AudioClip)
    {
        m_AudioSource.clip = a_AudioClip;

        m_AudioSource.PlayOneShot(a_AudioClip);
    }
    //the gravity calculation function
    private void GravityCalc()
    {

        //if the current room is assigned
        if (m_currentRoom != null)
        {
            //get the gravity from the current room
            Gravity t_GravityInRoom = m_currentRoom.GetRoomGravity();

            //if gravity is not null
            if (t_GravityInRoom != null)
            {
                //get the gravity direction vector
                Vector2 GravDir = t_GravityInRoom.GetGravityDirection();

                //aassign a rotation for the player based on the gravity vector
                Vector3 Rotation = new Vector3(0, 0, 0);
                if(GravDir.y == -1.0)
                {
                    Rotation.z = 0;
                    transform.localEulerAngles = Rotation;                 
                }
                else if (GravDir.y == 1.0)
                {
                    Rotation.z = 180;
                    transform.localEulerAngles = Rotation;
                }
                else if (GravDir.x == -1.0)
                {
                    Rotation.z = -90;
                    transform.localEulerAngles = Rotation;
                }
                else if (GravDir.x == 1.0)
                {
                    Rotation.z = 90;
                    transform.localEulerAngles = Rotation;
                }

                //move the player based on gravity
                m_PlayerRigidBody.velocity += GravDir * t_GravityInRoom.GetGravityMultiplier() * Gravity.Gravity_Constant * Time.deltaTime;
            }
            else
            {
                throw new System.Exception("No Gravity found");
            }
        }
    }
    //Function for player horizontal movement
    private void OnPressMove()
    {
        this.transform.parent = null;

        //float for current acceleration value
        float t_fAccel;

        //if not in contact with the ground then half the players acceleration
        if (GroundCheck())
        {
          t_fAccel = m_fAcceleration;

        }
        else
        {
            t_fAccel = m_fAcceleration * 0.5f;
        }

        //if A or Left arrow pressed and not D or Right arrow
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.RightArrow) ||
           Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.D))
        {

            //set the facing direction to left and flip the sprite
            m_v2PlayerFacingDir = -transform.right;
            GetComponent<SpriteRenderer>().flipX = true;

            //if the rotation is for being upside down dont flip the sprite
            if (transform.eulerAngles.z == 180)
            {

                GetComponent<SpriteRenderer>().flipX = false;


            }

            //get the current velocity on x
            float t_fMoveVector = m_PlayerRigidBody.velocity.x;

            //if the player is Horizontal get the y velocity instead
            if (transform.eulerAngles.z == 90 || transform.eulerAngles.z == 270)
            {

                t_fMoveVector = m_PlayerRigidBody.velocity.y;
            }

            //get a movement vector from the acceleration
            t_fMoveVector -= t_fAccel * Time.deltaTime;

            //if the player is horizontal then use the y velocity to fix to max otherwise use the x
            if (transform.eulerAngles.z == 90 || transform.eulerAngles.z == 270)
            {
                if (m_PlayerRigidBody.velocity.y < m_fCurrrentMaxVelocity)
                {
                    t_fMoveVector = -m_fCurrrentMaxVelocity;
                }
            }
            else
            {
                if (m_PlayerRigidBody.velocity.x < -m_fCurrrentMaxVelocity)
                {
                    t_fMoveVector = -m_fCurrrentMaxVelocity;
                }
            }
                
            if (transform.eulerAngles.z > 180)
            {

                t_fMoveVector = -t_fMoveVector;

            }

            //set the new movement vector
            Vector2 t_NewMoveVector = new Vector2(t_fMoveVector, m_PlayerRigidBody.velocity.y);

            //if horizontal assign to the y element
            if (transform.eulerAngles.z == 90 || transform.eulerAngles.z == 270)
            {
                t_NewMoveVector = new Vector2(m_PlayerRigidBody.velocity.x, t_fMoveVector);
            }

            //assign the players velocity
            m_PlayerRigidBody.velocity = t_NewMoveVector;

            //set is moving to true
            m_bIsMoving = true;

        }
        //the same again but for the other direction
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow) ||
                 Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.A))
        {

            m_v2PlayerFacingDir = transform.right;
            GetComponent<SpriteRenderer>().flipX = false;


            float t_fMoveVector = m_PlayerRigidBody.velocity.x;

            if (transform.eulerAngles.z == 180)
            {

                GetComponent<SpriteRenderer>().flipX = true;


            }

            if (transform.eulerAngles.z == 90 || transform.eulerAngles.z == 270)

            {

                t_fMoveVector = m_PlayerRigidBody.velocity.y;
            }

            t_fMoveVector += t_fAccel * Time.deltaTime;


            if (transform.eulerAngles.z == 90 || transform.eulerAngles.z == 270)

            {
                if (m_PlayerRigidBody.velocity.y > -m_fCurrrentMaxVelocity)
                {
                    t_fMoveVector = m_fCurrrentMaxVelocity;
                }
            }
            else
            {
                if (m_PlayerRigidBody.velocity.x > m_fCurrrentMaxVelocity)
                {
                    t_fMoveVector = m_fCurrrentMaxVelocity;
                }
            }

          
           

            if (transform.eulerAngles.z > 180)
            {

                t_fMoveVector = -t_fMoveVector;

            }

            Vector2 t_NewMoveVector = new Vector2(t_fMoveVector, m_PlayerRigidBody.velocity.y);

            if (transform.eulerAngles.z == 90 || transform.eulerAngles.z == 270)

            {
                t_NewMoveVector = new Vector2(m_PlayerRigidBody.velocity.x, t_fMoveVector);
            }

            m_PlayerRigidBody.velocity = t_NewMoveVector;

            m_bIsMoving = true;

        }
        else
        {
            //set moving to false
            m_bIsMoving = false;

        }
    }
    //Function for player jump
    private void OnPressJump()
    {
        this.transform.parent = null;

        //if has jumps available jump when up or W pressed
        if (m_iJumps > 0)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                //if in contact with wall use a wall jump else use a jump
                if (WallContactCheck())
                {
                    WallJump();
                }
                else
                {
                    GroundJump();
                }

                m_Animator.SetTrigger("Jumped");

            }
        }
        else
        {
            //if no jumps available but is in contact with a wall perform a wall jump
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (WallContactCheck())
                {
                    WallJump();
                }
            }
        }
    }
    //function for regular jump
    private void GroundJump()
    {

        PlayAudio(m_Jump);

        //set new velocity
        Vector2 t_NewJumpVector = new Vector2(m_PlayerRigidBody.velocity.x, 0);

        if(transform.up.x == 1 || transform.up.x == -1)
        {
            t_NewJumpVector = new Vector2(0, m_PlayerRigidBody.velocity.y);
        }


        Vector2 t_NewJumpVector2 = (Vector2)transform.up * (m_fJumpStrength) + t_NewJumpVector;

        //m_PlayerRigidBody.velocity += (Vector2)transform.up * (m_fJumpStrength);

        //apply the velocity to the player
        m_PlayerRigidBody.velocity = t_NewJumpVector2;

        //decrement the players jumps
        m_iJumps--;

    }
    //function for wall jump
    private void WallJump()
    {

        PlayAudio(m_Jump);

        //create a new vector for the jump based on the jump strength and the current velocity
        Vector2 t_NewJumpVector = new Vector2(m_PlayerRigidBody.velocity.x, 0);

        if (transform.up.x == 1 || transform.up.x == -1)
        {
            t_NewJumpVector = new Vector2(0, m_PlayerRigidBody.velocity.y);
        }

        //platform bitmask
        int t_iPlatformMask = 1 << 8;

        Vector2 t_v2PlayerSize = new Vector2(GetComponent<CapsuleCollider2D>().size.x, GetComponent<CapsuleCollider2D>().size.y * 0.75f) * this.transform.localScale.y;
        float t_fCastDistance = (GetComponent<CapsuleCollider2D>().size.x * .5f) * (this.transform.localScale.x);


        //cast the players size as a box to check if the wall is to the left or right of the player
        if (Physics2D.BoxCast(this.transform.position, t_v2PlayerSize, 0f, m_v2PlayerFacingDir, t_fCastDistance, t_iPlatformMask))
        {
                //if the new x component is greater than -1 make it -1
                t_NewJumpVector.x = -m_v2PlayerFacingDir.x * 3;

                Vector2 t_NewJumpVector2 = (Vector2)transform.up * (m_fJumpStrength) + t_NewJumpVector;


                m_PlayerRigidBody.velocity = t_NewJumpVector2;

        }
    }
    //function to handle platform movement in the x axis
    public void HandlePlatformX()
    {
        //if platform is not null
        if(m_platform != null)
        {
            //get the current position and deduct the offset
            //offset is the players position from the platform when first stopped on the platform
            Vector3 t_temp = this.transform.position;
            t_temp.x = m_platform.transform.position.x - offset.x;

            //assign the newly calculated position
            this.transform.position = t_temp;


        }
    }
    //same as above but for the y axis
    public void HandlePlatformY()
    {
        if (m_platform != null)
        {
            Vector3 t_temp = this.transform.position;
            t_temp.y = m_platform.transform.position.y - offset.y;

            this.transform.position = t_temp;
        }
    }
    //function for player crouch  (Defunct as of latest updates)
    private void OnPressCrouch()
    {
        if (GroundCheck())
        {
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                Vector3 t_CrouchHeadPos = this.transform.position;

                if (m_v2PlayerFacingDir.x > 0)
                {
                    t_CrouchHeadPos.y += 0.1f;
                    t_CrouchHeadPos.x += 0.1f;
                }
                else
                {
                    t_CrouchHeadPos.y += 0.1f;
                    t_CrouchHeadPos.x -= 0.1f;
                }

                //m_playerHead.transform.position = t_CrouchHeadPos;

                m_fCurrrentMaxVelocity = m_fMaxVelocity * 0.5f;

                m_bIsCrouching = true;
            }
            else
            {
               // m_playerHead.transform.position = this.transform.position + m_v3HeadPosOffset;

                m_fCurrrentMaxVelocity = m_fMaxVelocity;

                m_bIsCrouching = false;


            }
        }
        else
        {
            m_bIsCrouching = false;

        }
    }
    //function for player to fire
    private void OnPressFire()
    {
        //when space bar is pressed
        if (Input.GetKey(KeyCode.Space))
        {
            //and the player can fire
            if (m_bCanFire)
            {
                //for each bullet in the bullet array
                foreach (PlayerBullet Bullet in m_aPlayerBullets)
                {
                    //if the bullet is not active
                    if (!Bullet.GetIsActive())
                    {
                        //if the instance of the prefab for the bullet is null
                        if (Bullet.GetInstance() == null)
                        {
                            //create a new instance for the bullet
                            GameObject t_TempObj = Instantiate(Bullet.GetPrefab(), this.transform);

                            //set the instanced bullet in the bullet script
                            Bullet.InstancedBullet(t_TempObj);
                        }
                        else
                        {
                            //if the bullet already has an instance just reset the position to the player
                            Bullet.SetPosition(this.transform.position);
                        }

                        //play the shoot audioclip
                        PlayAudio(m_Shoot);

                        //if the current room is not null
                        if (m_currentRoom != null)
                        {
                            //get the room gravity
                            Gravity t_GravityInRoom = m_currentRoom.GetRoomGravity();

                            //if the gravity is flipped then flip the fireing direction as well
                            if(t_GravityInRoom.GetGravityDirection().y > 0)
                            {
                                Bullet.SetTravelDirection(-m_v2PlayerFacingDir);

                            }
                            else
                            {
                                Bullet.SetTravelDirection(m_v2PlayerFacingDir);
                            }

                        }

                        //set the bullet to active
                            Bullet.SetIsActive(true);
                  
                        //set can fire to false
                        m_bCanFire = false;

                        //start the fire delay
                        StartCoroutine(FireDelay());

                        break;
                    }
                }
            }
        }

        foreach (PlayerBullet Bullet in m_aPlayerBullets)
        {
            if (Bullet.GetIsActive())
            {
                if (!Bullet.GetDelayStarted())
                { 
                    Bullet.SetDelayStarted(true);
                    StartCoroutine(CheckDelay(Bullet));
                }

                Bullet.Update();

            }
        }
    }
    //function to check ground contact
    private bool GroundCheck()
    {
        //get the platform layer mask
        int t_iPlatformMask = 1 << 8;

        //get teh size of the player and the distance to cast for
        Vector2 t_v2PlayerSize = new Vector2(GetComponent<CapsuleCollider2D>().size.x * 0.75f, GetComponent<CapsuleCollider2D>().size.x) * this.transform.localScale.x;
        float t_fCastDistance = (GetComponent<CapsuleCollider2D>().size.y * .5f) * (this.transform.localScale.y);
        
        //if the player is horizontal get the y size instead
        if(transform.up.y != 1.0 && transform.up.y != -1.0)
        {
            float newY = t_v2PlayerSize.x;
            t_v2PlayerSize.x = t_v2PlayerSize.y;
            t_v2PlayerSize.y = newY;
        }

        //if not platform is hit return false
        if(!Physics2D.BoxCast(this.transform.position,t_v2PlayerSize,0f,-transform.up, t_fCastDistance, t_iPlatformMask))
        {

            return false;
        }

        //if an object was hit get the hit data
        RaycastHit2D t_PlatformHit = Physics2D.BoxCast(this.transform.position, t_v2PlayerSize, 0f, -transform.up, t_fCastDistance, t_iPlatformMask);

        //if the hit object is tagged as a platform
        if(t_PlatformHit.transform.tag == "Platform")
        {
            //assign the platform to the variable
            m_platform = t_PlatformHit.transform;

            //if the platform parent is named platform holder the child the player to the holder
            if(m_platform.transform.parent.name == "PlatformHolder")
            {
                this.transform.parent = m_platform.transform.parent;
                m_platform = null;
            }
        }
        else
        {
            //else set the parent to null and the platform to null
            this.transform.parent = null;
            m_platform = null;
        }


        //Debug.DrawRay(this.transform.position, -transform.up * t_fCastDistance, Color.blue);

        //return true
        return true;
    }
    //function for wall check
    private bool WallContactCheck()
    {
        //again get the platform layer mask
        int t_iPlatformMask = 1 << 8;

        //calcualte the player size and cast distance
        Vector2 t_v2PlayerSize = new Vector2(GetComponent<CapsuleCollider2D>().size.x, GetComponent<CapsuleCollider2D>().size.y * 0.75f) * this.transform.localScale.y;
        float t_fCastDistance = (GetComponent<CapsuleCollider2D>().size.x) * (this.transform.localScale.x);

        ////if a cast box hits the wall in the facing direction then return true
        if (Physics2D.BoxCast(this.transform.position, t_v2PlayerSize, 0f, m_v2PlayerFacingDir, t_fCastDistance, t_iPlatformMask))
        {
            //Debug.Log("Contact with wall");

            return true;
        }

        return false;
        
    }
    //function to check if the player is stopped
    private void CheckStopped()
    {
        //if the player x or y velocity is 0 on x or between .5 and -.5 on y
        if (m_PlayerRigidBody.velocity.x == 0 || m_PlayerRigidBody.velocity.y <= 0.5 || m_PlayerRigidBody.velocity.y >= -0.5)
        {
            //if the player has a platform
            if (m_platform != null)
            {
                //calculate the offset from the platform to the player
                offset = m_platform.transform.position - this.transform.position;

                //if x is 0 the set stopped on x to true
                if (m_PlayerRigidBody.velocity.x == 0)
                {
                    m_bStoppedOnX = true;
                }
                else
                {
                    m_bStoppedOnX = false;

                }

                 //if y is between .5 and -.5 then stopped on y is true
                if (m_PlayerRigidBody.velocity.y <= 0.5 && m_PlayerRigidBody.velocity.y >= -0.5)
                {
                    m_bStoppedOnY = true;
                }
                else
                {
                    m_bStoppedOnY = false;

                }
            }
        }
        else
        {
            //else both are false
            m_bStoppedOnX = false;
            m_bStoppedOnY = false;
        }
    }
    
    //function to reset the players base values on death
    public void Reset()
    {
        //play the death sound
        PlayAudio(m_Death);

        //reset the number of jumps the player has
        m_iJumps = 2;

        //assign the players platform to null
        m_platform = null;

        //zero out the velocity on both axis
        m_PlayerRigidBody.velocity = Vector2.zero;
        m_PlayerRigidBody.velocity = Vector2.zero;

        //reset all the rooms gravity 
        Room[] t_LevelRooms = m_GameManager.GetLevelRooms();

        foreach(Room room in t_LevelRooms)
        {
            room.GetRoomGravity().Reset();
        }

        //set can fire to true
        m_bCanFire = true;

        //reset all the bullets in the players bullet array
        foreach(PlayerBullet bullet in m_aPlayerBullets)
        {
            if(bullet.GetIsActive())
            {
                bullet.Reset();
            }
        }

        //stop any coroutines running
        StopAllCoroutines();

        //move the player to the respawn point position
        this.transform.position = m_v3RespawnPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the player enters a extra jump 
        if (collision.gameObject.tag == "ExtraJump")
        {
            //get the extra jump script
            ExtraJump t_ExtraJump = collision.gameObject.GetComponent<ExtraJump>();

            if (t_ExtraJump != null)
            {
                //if the jump is active
                if (t_ExtraJump.GetIsActive())
                {
                    //play the extra jump sound
                    PlayAudio(m_JumpBoost);

                    //increase the available jumps
                    m_iJumps++;

                    //Limit the jumps to the maximum allowed
                    if (m_iJumps > m_iMaxJumps)
                    {
                        m_iJumps = m_iMaxJumps;
                    }

                    //call the collected function on the extraJump script and set its active to false
                    t_ExtraJump.Collected();
                    t_ExtraJump.SetIsActive(false);

                }
            }
        }
        //if the player enters a button
        if (collision.gameObject.tag == "Button")
        {
            //get the button script
            PressButton Button = collision.gameObject.GetComponent<PressButton>();

            if (Button != null)
            {
                if (Button.GetPushable())
                {

                    //reverse the gravity and start the push delay
                    GetRoom().GetRoomGravity().ReverseGrav();

                    StartCoroutine(Button.PushDelay(0.5f));


                }
            }
        }
        //if enters a respawn collider then set a new respawn point
        if (collision.gameObject.tag == "Respawn")
        {
            m_v3RespawnPos = collision.gameObject.transform.position;
        }

      
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //if the player is on level 2
        if (m_GameManager.GetCurrentLevel().GetLevelName() == "Level 2")
        {
            // and the player collides with a projectile
            if (collision.gameObject.tag == "Projectile")
            {
                //get the Relevent script for the prohjectile
                if (collision.gameObject.GetComponent<Boss_Two_Laser>() != null)
                {
                    if (collision.gameObject.GetComponent<Boss_Two_Laser>().GetIsActive())
                    {
                        //call the reset functions for the boss player and the projectile
                        m_Boss.Reset();

                        Reset();

                        collision.gameObject.GetComponent<Boss_Two_Laser>().Reset();

                        m_GameManager.SpikeDeath();
                    }
                }

            }
        }
        //if the player is on level 3 
        if (m_GameManager.GetCurrentLevel().GetLevelName() == "Level 3")
        {
            if (collision.gameObject.tag == "Projectile")
            {
               
                        m_Boss.Reset();

                        Reset();

                        m_GameManager.SpikeDeath();

            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if collides with a platform then reset the jumps
        if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "BouncePad")
        {
            if (GroundCheck())
            {
                m_iJumps = 2;

                //Debug.Log("Floor Contact");
            }
            if (WallContactCheck())
            {

                m_fVelocityOnContact = collision.relativeVelocity.x;

            }
        }
        //if collides with a spike or projectile
        else if (collision.gameObject.tag == "Spike" || collision.gameObject.tag == "Projectile")
        {
            if (m_GameManager.GetCurrentLevel().GetLevelName() == "Level 1")
            {               
                if (collision.gameObject.tag == "Projectile")
                {
                    m_Boss.Reset();
                }


                Reset();

                m_GameManager.SpikeDeath();
            }
            else if (m_GameManager.GetCurrentLevel().GetLevelName() == "Level 2")
            {
                if (collision.gameObject.tag == "Spike")
                {

                    Reset();


                    m_GameManager.SpikeDeath();

                }
            }
            else if (m_GameManager.GetCurrentLevel().GetLevelName() == "Level 3")
            {
                if (collision.gameObject.tag == "Projectile")
                {
                    m_Boss.Reset();
                }


                Reset();

                m_GameManager.SpikeDeath();
            }
            else if (m_GameManager.GetCurrentLevel().GetLevelName() == "Level 4")
            {
                if (collision.gameObject.tag == "Projectile")
                {
                    m_Boss.Reset();
                }


                Reset();

                m_GameManager.SpikeDeath();
            }
        }
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            //ignore collisions with player bullets
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), collision.collider);
        }
        else if (collision.gameObject.tag == "Boss")
        {
            //if the player collides with the boss of level one while it is in the bounce or anger state kill the player and call all needed reset functions
            if (m_GameManager.GetCurrentLevel().GetLevelName() == "Level 1")
            {
                Boss_One t_Boss = (Boss_One)m_Boss;

                if (t_Boss.GetState() == Boss_One.ATTACK_STATE.BOUNCE || t_Boss.GetState() == Boss_One.ATTACK_STATE.ANGER)
                {
                    m_Boss.Reset();

                    Reset();

                    this.transform.position = m_v3RespawnPos;

                    m_GameManager.SpikeDeath();
                }
            }
            //if the player collides with the boss on level 2 the kill the player
            else if (m_GameManager.GetCurrentLevel().GetLevelName() == "Level 2")
            {
                Boss_Two t_Boss = (Boss_Two)m_Boss;


                    m_Boss.Reset();

                    Reset();

                    this.transform.position = m_v3RespawnPos;

                    m_GameManager.SpikeDeath();
            }
            
        }


    }                    

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Spike" || collision.gameObject.tag == "Projectile")
        {

            if (collision.gameObject.tag == "Projectile")
            {
                m_Boss.Reset();
            }


            this.transform.position = m_v3RespawnPos;

            m_GameManager.SpikeDeath();
        }
    }

    //ienumerator for fire delay
    IEnumerator FireDelay()
    {
        yield return new WaitForSeconds(m_fFireDelay);

        m_bCanFire = true;

        yield return null;
    }

    //delay for checking if the player bullet is visible to the camera
    IEnumerator CheckDelay(PlayerBullet a_bullet)
    {
        yield return new WaitForSeconds(0f);

        a_bullet.SetCheckVisible(true);

        //Debug.Log("Bullet Check Started");

        yield return null;
    }

    //functions for getting and setting the player room
    public void SetRoom(Room a_Room)
    {
        m_currentRoom = a_Room;
    }
    public Room GetRoom()
    {
        return m_currentRoom;
    }
}
