//////////////////////////////////////////////////////////////////////////
///File name: AI_Guard.cs
///Date Created: 09/11/2020
///Created by: JG
///Brief: override class for the guard Agents
///Last Edited by: JG
///Last Edited on: 14/12/2020
//////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AI_Guard : AI_Agent
{

    protected GameObject AttackObject;

    private StatusIcon StatusScript;

    [SerializeField]
    private AudioClip m_acAlertAudio;
    [SerializeField]
    private AudioClip m_acEngagedAudio;
    [SerializeField]
    private AudioClip m_acLostTargetAudio;

    private AudioSource m_asAudioSource;

    private bool m_bSoundWait = false;
    public bool bSeeTimer = false;
    public bool bSeeTimerComplete = false;

    [SerializeField]
    private float m_fTimeToSee;

    [SerializeField]
    private GameObject m_goPlayer;

    private LayerMask m_PlayerLayer;

    private const float GRABCOOLDOWNMAX = 8.0f;
    private float m_fGrabCooldown;

    //base constructor
    public AI_Guard()
    {

    }

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

    public override void SetGoal()
    {

        m_Goal = new HashSet<KeyValuePair<string, bool>>();

        m_Goal.Add(new KeyValuePair<string, bool>("HasPatrolled", true));
        m_Goal.Add(new KeyValuePair<string, bool>("PlayerDefeated", true));
    }

    public override void Start()
    {
        m_eType = AGENT_TYPE.GUARD;

        AttackObject = transform.Find("AttackObject").gameObject;

        StatusScript = GetComponentInChildren<StatusIcon>();

        m_asAudioSource = this.gameObject.AddComponent<AudioSource>();

        m_asAudioSource.volume = 0.8f;
        m_asAudioSource.spatialBlend = 1.0f;
        m_asAudioSource.maxDistance = 10.0f;

        m_PlayerLayer = LayerMask.GetMask("Player");

        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {

        AwareProgression();

        if (m_VisionCone.GetPlayerSeen())
        {

            if (!bSeeTimer)
            {
                StatusScript.StopFade();

                m_fTimeToSee = m_VisionCone.GetAISightTime();

                StartCoroutine(WaitToSee(m_VisionCone.GetAISightTime()));
            }
            else if (bSeeTimerComplete)
            {

                 m_v3Stimulus = m_PlayerObject.transform.position;
                 
            }
        }
        else
        {

            if (m_bPlayerSighted)
            {
                //Debug.Log("Lost Player");
                StopCoroutine(WaitToSee(0));
                
                StatusScript.SetSliderValue(0);


                m_v3Stimulus = m_PlayerObject.transform.position;

                bSeeTimerComplete = false;

                bSeeTimer = false;
            }

            

            m_bPlayerSighted = false;

        }

        if(m_fGrabCooldown > 0)
        {
            m_fGrabCooldown -= Time.deltaTime;
        }

        GrabPlayer();

        base.Update();
    }

    public override void AwareProgression()
    {
        if (GetState() == AGENT_STATE.STUNNED)
        {

            StatusScript.StopEngage();
            StatusScript.StopFade();
            //StatusScript.SliderActiveState(false);

            StatusScript.SetSliderValue(0);

            StopCoroutine(WaitToSee(0));

            if (!m_bStunned)
            {

                m_eAwarenessState = AWARENESS_STATE.UNAWARE;

                StatusScript.StartColourChange(fStunTime);

                m_bStunned = true;
            }


        }
        else
        {
            if (GetPlayersighted())
            {
                StatusScript.StopFade();

                if (m_eAwarenessState != AWARENESS_STATE.ENGAGED)
                {
                    //Show Engaged
                    StatusScript.StartEngage();
                    StatusScript.SetSliderValue(0);

                    //StatusScript.SliderActiveState(false);

                    m_eAwarenessState = AWARENESS_STATE.ENGAGED;


                    if (!m_bSoundWait)
                    {

                        StartCoroutine(SoundWait(m_acEngagedAudio));

                        m_bSoundWait = true;

                    }

                }
            }
            else if (GetAlert())
            {
                StatusScript.StopEngage();
                //StatusScript.SliderActiveState(false);
                StatusScript.SetSliderValue(0);


                if (m_eAwarenessState != AWARENESS_STATE.ALERT)
                {
                    if (m_eAwarenessState == AWARENESS_STATE.UNAWARE)
                    {
                        if (!m_bSoundWait)
                        {

                            StartCoroutine(SoundWait(m_acAlertAudio));

                            m_bSoundWait = true;

                        }
                    }

                    StopCoroutine(WaitToSee(0));

                    bSeeTimer = false;

                    //Flash Solid icon
                    StatusScript.StartFade();
                    //Fade in and out until change
                    m_eAwarenessState = AWARENESS_STATE.ALERT;


                }
            }
            else
            {

                StatusScript.StopFade();
                StatusScript.StopEngage();

                if (m_eAwarenessState != AWARENESS_STATE.UNAWARE)
                {
                    //Show Unaware
                    m_eAwarenessState = AWARENESS_STATE.UNAWARE;
                    if (!m_bSoundWait)
                    {

                        StartCoroutine(SoundWait(m_acLostTargetAudio));

                        StopCoroutine(WaitToSee(0));

                        StatusScript.SetSliderValue(0);

                        bSeeTimer = false;

                        m_bSoundWait = true;

                    }

                }

            }
        }

    }

    //Trial for grabbing the player
    public void GrabPlayer()
    {
        if(GetPlayersighted() && !m_goPlayer.GetComponent<PlayerController>().GetGrabbed() && m_fGrabCooldown <= 0)
        {
            foreach(Collider col in Physics.OverlapSphere(transform.position, 1.5f, m_PlayerLayer))
            {   
                m_goPlayer.GetComponent<PlayerController>().SetGrabbed(true);
                m_goPlayer.transform.rotation = this.gameObject.transform.rotation;
                m_goPlayer.transform.Rotate(new Vector3(0, 180, 0));
            }
        }
    }

    public void NoLongerGrabbed()
    {
        m_fGrabCooldown = GRABCOOLDOWNMAX;
    }

    public override void HeardSound(Vector3 m_v3SoundPosition,bool a_isDecoy)
    {
        m_v3Stimulus = m_v3SoundPosition;

        //Debug.Log("Audio detected");

        SetAlert(true);


        AI_WorldState t_WorldState = (AI_WorldState)GetWorldDataSource();

        t_WorldState.SetCondition("Investigating", true);
    }

    public void SeenPlayer()
    {
        if (!m_bPlayerSighted)
        {
            GetWorldDataSource().SetCondition("InPursuit", true);
            GetWorldDataSource().SetCondition("Investigating", false);
            GetWorldDataSource().SetCondition("PlayerDefeated", false);
            GetWorldDataSource().SetCondition("KnowsPlayer", true);
        }

        m_bPlayerSighted = true;

        SetAlert(true);

        GetBlackboard().ShareData(this);

        m_v3Stimulus = m_PlayerObject.transform.position;
    }

    public void AlarmHeard()
    {
        Debug.Log("Alarm detected");

        AI_WorldState t_WorldState = (AI_WorldState)GetWorldDataSource();

        SetAlert(true);

        t_WorldState.SetCondition("Investigating", true);
    }

    public void AttackAnim()
    {
        AttackObject.GetComponent<Animator>().SetTrigger("Attack");

    }

    IEnumerator SoundWait(AudioClip AudioToPlay)
    {

        m_asAudioSource.clip = AudioToPlay;

        m_asAudioSource.Play();

        yield return new WaitForSeconds(AudioToPlay.length + 0.5f);

        m_bSoundWait = false;


        yield return null;
    }

    IEnumerator WaitToSee(float a_fTimeTillSeen)
    {
        GetNavAgent().isStopped = true;

        if (!m_bSoundWait)
        {

            StartCoroutine(SoundWait(m_acAlertAudio));

            m_bSoundWait = true;

        }

        yield return new WaitForSeconds(0.25f);

        GetNavAgent().isStopped = false;

        GetNavAgent().SetDestination(m_v3Stimulus);
        GetNavAgent().speed = 1.0f;

        bSeeTimer = true;

        bSeeTimerComplete = false;

        float fTimer = 0;

        StatusScript.SliderActiveState(true);

        StatusScript.SetMaxSliderValue(a_fTimeTillSeen);

        do
        {

            fTimer += Time.deltaTime;

            StatusScript.SetSliderValue(fTimer);

            yield return new WaitForFixedUpdate();

        } while (fTimer < a_fTimeTillSeen);

        bSeeTimerComplete = true;

        StatusScript.SetSliderValue(0);

        GetNavAgent().speed = 1.8f;



        SeenPlayer();

        yield return null;

    }
}
