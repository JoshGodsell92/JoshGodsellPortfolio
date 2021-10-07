/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///Name: StateBasedAI.cs 
///Created by: Charlie Bullock
///Description: This script allows for state based AI which follow nodes, the state, order and other aspects of this AI
///script are editable from the inspector via many private variables being serializable.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBasedAi : MonoBehaviour {
    //Variables
    #region variables
    //Private serialised variables
    [SerializeField]
    private Transform[] enemyPointNodes;
    [SerializeField]
    private AudioClip defaultNoise;
    [SerializeField]
    private AudioClip firingNoise;
    [SerializeField]
    private AudioClip deathNoise;
    [SerializeField]
    private GameObject projectile;
    private GameObject projectileObject;
    [SerializeField]
    private float fSpeed;
    [SerializeField]
    private bool bInvertPath;
    [SerializeField]
    private bool bSpeedIncreaseWhileChasingPlayer;
    [SerializeField]
    enum EnemyType
    {
        FLYER,
        SHOOTER,
        WALKER,
    }
    [SerializeField]
    EnemyType enemyType;
    //Private variables
    private Vector3 m_v3StartPos;

    //private ParticleSystem ps;
    private GameObject player;
    public bool bPatrolPoints;
    private int iNodes;
    //Public static variables
    public static bool bProjectileLaunched;
    #endregion variables;

    public bool m_bDestructable = true;

    private PlayerControl m_Player;

    private GameManager m_GM;
    private AudioSource m_AudioSource;

    private bool m_bPlayerInRoom;

    public AudioClip m_Audio;
    public float m_fSoundDelay;

	//Start function sets up the state based AI and sets the state and value of various values
	void Awake () {        
        bProjectileLaunched = false;
        player = GameObject.FindWithTag("Player");
        //ps = this.gameObject.GetComponent<ParticleSystem>();
        bPatrolPoints = true;
        if (bInvertPath == false)
        {
            iNodes = 0;
        }
        else
        {
            iNodes = enemyPointNodes.Length - 1;
        }

        try
        {
            m_GM = FindObjectOfType<GameManager>();
            m_Player = FindObjectOfType<PlayerControl>();
        }
        catch (System.Exception)
        {

            throw;
        }

        if(GetComponent<AudioSource>() == null)
        {
           m_AudioSource = this.gameObject.AddComponent<AudioSource>();
           m_AudioSource.playOnAwake = false;
        }
        else
        {
            m_AudioSource = GetComponent<AudioSource>();
        }
        

        m_v3StartPos = this.transform.position;

    }


    //FixedUpdate function will either patrol between nodes assigned to the enemyPointNodes array or will act in a certain way based upon their EnemyType state
    private void FixedUpdate()
    {

        CheckPlayerRoom();

        //As long as the ai is patrolling and it's array of point nodes is not null
        if (bPatrolPoints == true && enemyPointNodes.Length != 0)
        {
            CheckWaypointCompletion(bInvertPath);
            if ((int)enemyType == 0)
            {
                if (this.gameObject.transform.position == enemyPointNodes[iNodes].transform.position)
                {
                    if (bInvertPath == false)
                    {
                        iNodes++;
                    }
                    else
                    {
                        iNodes--;
                    }            
                }
                CheckWaypointCompletion(bInvertPath);

                float LookDiff = transform.position.x - enemyPointNodes[iNodes].transform.position.x;
                Vector3 rotateY = new Vector3(0, 180, 0);
                Vector3 normalY = new Vector3(0, 0, 0);

                if (LookDiff < 0)
                {
                    transform.localEulerAngles = rotateY;
                    //GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    transform.localEulerAngles = normalY;

                    //GetComponent<SpriteRenderer>().flipX = false;

                }

                transform.position = Vector3.MoveTowards(transform.position, enemyPointNodes[iNodes].transform.position, Time.deltaTime * fSpeed);
            }
            else
            {
                if (this.gameObject.transform.position.x == enemyPointNodes[iNodes].transform.position.x)
                {
                    if (bInvertPath == false)
                    {
                        iNodes++;
                    }
                    else
                    {
                        iNodes--;
                    }
                }
                CheckWaypointCompletion(bInvertPath);

                float LookDiff = transform.position.x - enemyPointNodes[iNodes].transform.position.x;
                Vector3 rotateY = new Vector3(0, 180, 0);
                Vector3 normalY = new Vector3(0, 0, 0);

                if (LookDiff < 0)
                {
                    transform.localEulerAngles = rotateY;
                    //GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    transform.localEulerAngles = normalY;

                    //GetComponent<SpriteRenderer>().flipX = false;

                }

                transform.position = Vector3.MoveTowards(transform.position, new Vector3 (enemyPointNodes[iNodes].transform.position.x,this.gameObject.transform.position.y, this.gameObject.transform.position.z), Time.deltaTime * fSpeed);

            }
        }
        //Else if the ai is not patrolling points we will do this statement
        else if (bPatrolPoints == false)
        {
            //Switch statement for the enemy types
            switch ((int)enemyType)
            {
                //Flyer type will move towards the player on the X and Y axis as it will be flying, also if bSpeedIncreaseWhileChasing is true then the speed will increase twice it's normal
                case (int)EnemyType.FLYER:

                    if (bSpeedIncreaseWhileChasingPlayer == true)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * (fSpeed * 2));
                    }
                    else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * fSpeed);
                    }
                    break;
                //Both Shooter and Walker will move towards the player only on the X axis, also if bSpeedIncreaseWhileChasing is true then the speed will increase twice it's normal
                case (int)EnemyType.SHOOTER:
                case (int)EnemyType.WALKER: 
                    //Additionally Shooter type will fire projectiles in the players direction
                    if ((int)enemyType == 1 && bProjectileLaunched == false)
                    {
                        bProjectileLaunched = true;
                        Instantiate(projectile, this.transform.position, this.transform.rotation);
                    }

                    float LookDiff = transform.position.x - enemyPointNodes[iNodes].transform.position.x;
                    Vector3 rotateY = new Vector3(0, 180, 0);
                    Vector3 normalY = new Vector3(0, 0, 0);

                    if (LookDiff < 0)
                    {
                        transform.localEulerAngles = rotateY;
                        //GetComponent<SpriteRenderer>().flipX = true;
                    }
                    else
                    {
                        transform.localEulerAngles = normalY;

                        //GetComponent<SpriteRenderer>().flipX = false;

                    }

                    if (bSpeedIncreaseWhileChasingPlayer == true)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z), Time.deltaTime * (fSpeed * 2));
                    }
                    else
                    {
                        
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z), Time.deltaTime * fSpeed);
                    }
                    break; 
                default:
                    break;
            }
        }
    }

    //Function to reset iNodes to either length of node point arrays or to 0 depending on if the ai is going through the nodes backwards or forwards
    private void CheckWaypointCompletion(bool a_pathInversion)
    {
        if (a_pathInversion == false)
        {
            if (iNodes >= enemyPointNodes.Length)
            {
                iNodes = 0;
            }
        }
        else
        {
            if (iNodes < 0)
            {
                iNodes = enemyPointNodes.Length - 1;
            }
        }
    }

    //Function for playing noise
    private void DefaultAudioNoise()
    {
        if (defaultNoise != null)
        {

        }
    }

    //This function will set bPatrolPoints to true when the player exits the collision collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerBullet" )
        {
            collision.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Player")
        {
            //Kills player
            GameManager GMScript = FindObjectOfType<GameManager>();
            GMScript.KillPlayer();
        }
    }

    //This function will set bPatrolPoints to false when the player enters the trigger collider
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            bPatrolPoints = false;
        }
        else if (collider.gameObject.tag == "PlayerBullet")
        {
            if (m_bDestructable)
            {

                collider.gameObject.SetActive(false);
                this.gameObject.SetActive(false);
            }
        }
    }

    //This function will set bPatrolPoints to true when the player exits the trigger collider
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            bPatrolPoints = true;
        }
    }

    //This function will destroy this Ai when called
    private void DestroyThisObject()
    {
        if (deathNoise != null)
        {
        }
        Destroy(this.gameObject);
    }

    //Additional functionality - JG
    public void Reset()
    {
        this.transform.position = m_v3StartPos;

        iNodes = 0;

        bPatrolPoints = true;

        this.gameObject.SetActive(true);
    }

    public void CheckPlayerRoom()
    {
        Room[] t_Rooms = m_GM.GetLevelRooms();

        foreach(Room room in t_Rooms)
        {
            if(room.m_bounds.Contains(this.transform.position))
            {
                if(room == m_Player.GetRoom())
                {
                    if(m_bPlayerInRoom == false)
                    {

                        m_bPlayerInRoom = true;

                        StartCoroutine(PlayAudioWithTimeGap());

                    }


                }
                else
                {
                    StopAllCoroutines();
                    m_bPlayerInRoom = false;
                }
            }
        }
    }

    public IEnumerator PlayAudioWithTimeGap()
    {

        while(true)
        {
            if (m_bPlayerInRoom)
            {

                if (m_Audio != null)
                {
                    m_AudioSource.PlayOneShot(m_Audio);

                    yield return new WaitForSeconds(m_fSoundDelay + m_Audio.length);
                }
                else
                {
                    yield return null;
                }
            }
        }

    }
}
