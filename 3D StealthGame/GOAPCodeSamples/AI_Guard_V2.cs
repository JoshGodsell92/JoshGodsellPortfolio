//////////////////////////////////////////////////////////////////////////
///File name: AI_Guard_V2.cs
///Date Created: 04/02/2021
///Created by: JG
///Brief: override class for the guard Agents. Version 2
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Guard_V2 : AI_Agent
{

    #region Member_Variables

    //script controlling the AI awareness icon
    private StatusIcon StatusScript;

    //the attack object for the AI
    protected GameObject AttackObject;

    //Player object
    [SerializeField]
    private GameObject m_goPlayer;

    //audio clips for AI
    [SerializeField]
    private AudioClip m_acAlertAudio;
    [SerializeField]
    private AudioClip m_acEngagedAudio;
    [SerializeField]
    private AudioClip m_acLostTargetAudio;

    //audio source for AI
    private AudioSource m_asAudioSource;

    //Player layer
    private LayerMask m_PlayerLayer;

    //if the player location is known
    [SerializeField]
    private bool m_bPlayerLocated = false;
    //if the AI ius ready for an action
    [SerializeField]
    private bool m_bActionAvailable = true;
    //if a sound has completed
    [SerializeField]
    private bool m_bSoundWait = false;

    //cooldown time for grab
    [SerializeField]
    private float m_fGrabCooldown;

    //is activly searching
    [SerializeField]
    private bool m_bActiveSearch = false;

    //timer for searching cooldown
    [SerializeField]
    private bool m_bCanSearch = true;

    //Hiding spot to search
    [SerializeField]
    private HidingPlace m_HidingSpotSearch;

    //is trying to improve
    private bool m_bImproveChance;

    //has found object
    private bool m_bImproveObjectSelected;

    //was decoyed
    private bool m_bDecoyed;

    #endregion //!Member Variables

    Animator m_AnimatorController;

    //Base constructor
    private AI_Guard_V2()
    {

    }

    public void Start()
    {
        //set the Agent type
        m_eType = AGENT_TYPE.GUARD;

        //get the icon script
        StatusScript = GetComponentInChildren<StatusIcon>();

        //get the attack object
        AttackObject = transform.Find("AttackObject").gameObject;

        //find the audio source
        m_asAudioSource = this.gameObject.AddComponent<AudioSource>();

        //find the animator component
        m_AnimatorController = this.gameObject.GetComponent<Animator>();

        //set the aource values
        m_asAudioSource.volume = 0.8f;
        m_asAudioSource.spatialBlend = 1.0f;
        m_asAudioSource.maxDistance = 10.0f;

        //get the player layer
        m_PlayerLayer = LayerMask.GetMask("Player");

        //set the awareness state
        m_eAwarenessState = AWARENESS_STATE.UNAWARE;

        //call the base agent start func
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {

        //set the animation states
        m_AnimatorController.SetBool("IsMoving", this.GetIsMoving());

        m_AnimatorController.SetBool("IsAware", m_bPlayerLocated);

        //if the guard is stunned
        if (GetState() == AGENT_STATE.STUNNED)
        {

            //stop the engage and fade indicators
            StatusScript.StopEngage();
            StatusScript.StopFade();
            //StatusScript.SliderActiveState(false);

            //set the animation state
            m_AnimatorController.SetBool("IsStunned", true);

            //reset the indicator slider and stop the coroutine
            StatusScript.SetSliderValue(0);
            StopCoroutine(TimeToSee(0));

            //if not already stunned
            if (!m_bStunned)
            {

                //reset the stun time and awareness states
                m_eAwarenessState = AWARENESS_STATE.UNAWARE;

                StatusScript.StartColourChange(fStunTime);


                m_bStunned = true;
            }


        }
        else
        {
            m_AnimatorController.SetBool("IsStunned", false);

        }

        //Speed change for pursuit
        if (m_bPlayerLocated)
        {
            m_fSpeed = 7.5f;
        }
        else
        {
            m_fSpeed = 2.4f;
        }

        //if the player is seen and the agent is not stunned
        if (m_VisionCone.GetPlayerSeen() && !m_bStunned)
        {
            SeenPlayer();
        }
        else
        {

            //otherwise start the break sight line coroutine
            StartCoroutine(BreakSightTimer(2.5f));

            m_bPlayerSighted = false;

        }

        //if the agent is not engaged with the player
        if (m_eAwarenessState != AWARENESS_STATE.ENGAGED)
        {
            //and the agent can activly search then start active searching
            if (m_bActiveSearch)
            {
                if (m_bCanSearch)
                {
                    ActiveSearching();
                }
            }

            //if objects that improve the chances of detection exist then use them
            if (m_bImproveChance)
            {
                ImprovingChances();
            }
        }

        //ensure a valid travel path
        CheckValidPath();

        base.Update();
    }

    //function to set this agents goal
    public override void SetGoal()
    {
        //primary and overall goal
        m_PrimaryGoal = new HashSet<KeyValuePair<string, bool>>();
        m_Goal = new HashSet<KeyValuePair<string, bool>>();

        //Primary goal is to complete its patrol which will never complete
        m_PrimaryGoal.Add(new KeyValuePair<string, bool>("HasPatrolled", true));

        //goal of improving chances of player detection and finishing the patrol
        m_Goal.Add(new KeyValuePair<string, bool>("HasPatrolled", true));
        m_Goal.Add(new KeyValuePair<string, bool>("ImprovedChances", true));
    }

    //overide for finding the agents current data source
    public override void FindDataSource()
    {
        try
        {
            m_WorldDataSource = GetComponent<AI_WorldState>();
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public override void AwareProgression()
    {

    }

    //Overide for when an agent detects a sound
    public override void HeardSound(Vector3 a_v3Position, bool a_bIsDecoy)
    {

        //if the player has not already been located
        if (!m_bPlayerSighted && !m_bPlayerLocated && !m_bDecoyed)
        {

            if (a_bIsDecoy)
            {
                m_bDecoyed = true;
            }

            //set a position for the detected sound and change states to alerted
            m_v3Stimulus = a_v3Position;

            m_eAwarenessState = AWARENESS_STATE.ALERT;

            //start the indication that an agent is alerted
            StartAwareFade();

            RecalculatePath(m_v3Stimulus);

            //set the world data source condition
            GetWorldDataSource().SetCondition("Investigating", true);
        }
    }

    //function for when an agent has seen the player
    public void SeenPlayer()
    {
        //if not already seen and located
        if (!m_bPlayerSighted && !m_bPlayerLocated)
        {

            //set the condition for investigating
            GetWorldDataSource().SetCondition("Investigating", true);

            //if improvement objects exist start using them
            if (m_bHasImprovementObjects)
            {
                m_bImproveChance = true;

            }

            //start a sight timer until fully seen
            StartSightTimer(m_VisionCone.GetAISightTime());

            m_v3Stimulus = m_PlayerObject.transform.position;

            //recalculate the current path to the sighted location
            RecalculatePath(m_v3Stimulus);
        }

        m_bPlayerSighted = true;

    }

    //Function to grab the player (Converted into seperate grab action class)
    public void GrabPlayer()
    {
        m_goPlayer.GetComponent<PlayerController>().SetGrabbed(true);

        m_AnimatorController.SetBool("HasGrabbed", true);

        Vector3 newForward = this.transform.position - m_goPlayer.transform.position;

        Quaternion lookDir = Quaternion.LookRotation(newForward, Vector3.up);

        m_goPlayer.transform.rotation = lookDir;

    }


    //Set a global reset on grab timer so player isd not repeatedly grabed by multiple agents
    public void GlobalGrabCooldownReset()
    {
        //set the global conditions
        GetWorldDataSource().SetCondition("CanGrab", false);
        GetWorldDataSource().SetCondition("PlayerGrabbed", false);

        m_AnimatorController.SetBool("HasGrabbed", false);

        //start the cooldown routine
        StartCoroutine(GrabCooldown(m_fGrabCooldown));
    }

    public void StartActiveSearch()
    {
        m_bActiveSearch = true;
        m_bCanSearch = true;
    }

    //function for active searching finds an hiding object for the asgent to search
    public void ActiveSearching()
    {
        LayerMask Layermask = LayerMask.GetMask("Default");

        foreach (Collider col in Physics.OverlapSphere(this.transform.position, 5.0f, Layermask))
        {

            Vector3 v3DirectionVector = col.gameObject.transform.position - transform.position;

            RaycastHit hitLong;
            if (Physics.Raycast(transform.position, v3DirectionVector, out hitLong))
            {

                HidingPlace interact = hitLong.transform.GetComponentInParent<HidingPlace>();

                if (interact != null)
                {
                    m_HidingSpotSearch = interact;

                    GetWorldDataSource().SetCondition("ActiveSearch", true);

                    m_v3Stimulus = interact.GetSearchPoint().transform.position;

                    RecalculatePath(m_v3Stimulus);

                    m_bCanSearch = false;

                }
            }
        }
    }

    //function for checking if there is an improvement object nearby
    public void ImprovingChances()
    {
        LayerMask Layermask = LayerMask.GetMask("Default");

        //for each collider in range
        foreach (Collider col in Physics.OverlapSphere(this.transform.position, 5.0f, Layermask))
        {

            Vector3 v3DirectionVector = col.gameObject.transform.position - transform.position;

            RaycastHit hitLong;
            if (Physics.Raycast(transform.position, v3DirectionVector, out hitLong))
            {

                //if the objedct had an object action script attached
                AI_ObjAction interact = col.transform.GetComponent<AI_ObjAction>();

                if (interact != null)
                {
                    //check if the object is available to interact with and there is not already one selected
                    if (interact.GetAvailable() && !m_bImproveObjectSelected)
                    {
                        //Debug.Log("Improvement object found");

                        //set the agents world condition to improve chances false
                        GetWorldDataSource().SetCondition("ImprovedChances", false);

                        m_v3Stimulus = interact.transform.position;

                        RecalculatePath(m_v3Stimulus);

                        m_bImproveObjectSelected = true;
                    }
                }
            }
        }
    }


    public void ForceUnhide()
    {
        if (m_HidingSpotSearch.bInUse)
        {
            m_HidingSpotSearch.AIInteract(this);

            GetWorldDataSource().SetCondition("KnowsPlayer", true);

        }

        m_HidingSpotSearch = null;

        StartCoroutine(ActiveSearchCooldown(10.0f));
    }

    public void ForceUnhide(HidingPlace a_HidingPlace)
    {
        if (a_HidingPlace.bInUse)
        {
            a_HidingPlace.AIInteract(this);

            GetWorldDataSource().SetCondition("KnowsPlayer", true);

        }

        m_HidingSpotSearch = null;

        StartCoroutine(ActiveSearchCooldown(10.0f));
    }

    //Check if the current path is valid
    public void CheckValidPath()
    {
        if (m_Path != null)
        {
            if (m_Path.status == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
            {

                if (m_bPlayerLocated)
                {

                    Debug.Log("<color=red>Player lost </color>");

                    //if the current path to the player is invalid then the player has been lost return to investigating
                    GetWorldDataSource().SetCondition("Investigating", true);
                    GetWorldDataSource().SetCondition("KnowsPlayer", false);

                    m_bPlayerLocated = false;

                    m_v3Stimulus = this.transform.position;

                    StatusScript.StopEngage();

                    StatusScript.StartFade();
                }
            }
        }

    }


    public void StartSightTimer(float a_fTimeToSee)
    {
        StartCoroutine(TimeToSee(a_fTimeToSee));
    }

    public void AttackAnim()
    {

        m_AnimatorController.SetTrigger("AttackTrigger");

    }

    public void StartActionTimer(float a_fTimeForAction)
    {
        StartCoroutine(ActionTimer(a_fTimeForAction));
    }

    public bool GetPlayerLocated()
    {
        return m_bPlayerLocated;
    }

    public void SetPlayerLocated(bool a_bool)
    {
        m_bPlayerLocated = a_bool;
    }

    public void StartAwareFade()
    {
        StatusScript.StartFade();
    }
    public void StopAwareFade()
    {

        m_asAudioSource.clip = m_acLostTargetAudio;

        m_asAudioSource.Play();

        StatusScript.StopFade();
    }

    public float GetSliderVal()
    {
        return StatusScript.GetSliderValue();
    }


    public bool GetActionAvailable()
    {
        return m_bActionAvailable;
    }

    public bool GetDecoyed()
    {
        return m_bDecoyed;
    }

    public void SetDecoyed(bool a_bool)
    {
        m_bDecoyed = a_bool;
    }

    public bool GetObjectSelected()
    {
        return m_bImproveObjectSelected;

    }
    public void SetObjectSelected(bool a_bool)
    {
        m_bImproveObjectSelected = a_bool;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Block")
        {
            SetState(AGENT_STATE.STUNNED);
        }
    }

    public IEnumerator TimeToSee(float a_fTimerToSee)
    {
        m_bPlayerSighted = true;

        float fTimer = 0;

        StatusScript.SliderActiveState(true);

        StatusScript.SetMaxSliderValue(a_fTimerToSee);

        m_eAwarenessState = AWARENESS_STATE.ALERT;

        m_asAudioSource.clip = m_acAlertAudio;

        m_asAudioSource.Play();

        do
        {
            if (m_bPlayerSighted)
            {

                m_v3Stimulus = m_goPlayer.transform.position;

                fTimer += Time.deltaTime;
            }
            else
            {
                if (fTimer > 0)
                {
                    fTimer -= Time.deltaTime;
                }
                else
                {

                    StatusScript.SetSliderValue(fTimer);

                    m_eAwarenessState = AWARENESS_STATE.UNAWARE;

                    m_bPlayerLocated = false;


                    yield break;
                }
            }

            StatusScript.SetSliderValue(fTimer);

            yield return new WaitForFixedUpdate();

        } while (fTimer < a_fTimerToSee);


        Debug.Log("Player Located");

        m_asAudioSource.clip = m_acEngagedAudio;

        m_asAudioSource.Play();

        //m_bImproveChance = true;

        GetWorldDataSource().SetCondition("CanGrab", true);


        m_bPlayerLocated = true;

        StatusScript.StartEngage();

        m_eAwarenessState = AWARENESS_STATE.ENGAGED;

        StatusScript.SetSliderValue(0);

        yield break;
    }

    public IEnumerator GrabCooldown(float a_fGrabCooldown)
    {


        yield return new WaitForSeconds(a_fGrabCooldown);


        GetWorldDataSource().SetCondition("CanGrab", true);

        yield break;
    }

    public IEnumerator ActiveSearchCooldown(float a_fCooldown)
    {

        yield return new WaitForSeconds(a_fCooldown);

        m_bCanSearch = true;

        yield break;
    }

    public IEnumerator ActionTimer(float a_fActionTime)
    {
        m_bActionAvailable = false;

        yield return new WaitForSeconds(a_fActionTime);


        m_bActionAvailable = true;

        yield return null;

    }

    public IEnumerator BreakSightTimer(float a_fTimeToBreakSight)
    {
        float fTimer = 0;

        do
        {

            fTimer += Time.deltaTime;

            if(m_bPlayerSighted)
            {
                yield break;
            }

            yield return new WaitForFixedUpdate();

        } while (fTimer < a_fTimeToBreakSight);

        if (m_bPlayerLocated)
        {
            Debug.Log("<color=red>Player lost </color>");

            GetWorldDataSource().SetCondition("SearchNearbyHide", true);
            GetWorldDataSource().SetCondition("Investigating", true);
            GetWorldDataSource().SetCondition("KnowsPlayer", false);
            GetWorldDataSource().SetCondition("CanGrab", false);

            m_eAwarenessState = AWARENESS_STATE.ALERT;

            m_bPlayerLocated = false;

            m_v3Stimulus = m_PlayerObject.transform.position;

            StatusScript.StopEngage();
            
            StatusScript.StartFade();

        }


        yield return null;

    }


    public IEnumerator SoundWait(AudioClip AudioToPlay)
    {

        m_asAudioSource.clip = AudioToPlay;

        m_asAudioSource.Play();

        yield return new WaitForSeconds(AudioToPlay.length + 0.5f);

        m_bSoundWait = false;


        yield return null;
    }
}
