/////////////////////////////////////////////////////////////////////////////////////
// Author: Harry Workman
// Date Created: 07/10/2020
// Last Editied: 03/11/2020 
// Last Editied By:  JG
// File Name: AudioOutput
// File Description:
/////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioOutput : MonoBehaviour
{
    //Constant Variables
    //Sound radius modifier if the object hits rug
    private const float CARPETAUDIOMODIFIER = 0.2f;

    //Sound radius modifier if the object hits wood
    private const float WOODAUDIOMODIFIER = 1.0f;

    //Sound radius modifier if the object hits metal
    private const float METALAUDIOMODIFIER = 2.0f;

    //How far sound permiates through a wall
    private const float OCCLUSIONWALLDISTANCE = 1.25f;

    //How long between decoy bursts and how long the decoy runs for
    private const float DECOYBURSTTIMER = 3.0f;
    private const int DECOYBURSTCOUNTER = 5;
    private const float DECOYAUDIODISTANCE = 15.0f;

    //ENUM of the types of the terrain
    private enum TerrainMat
    {
        NULL,
        WOOD,
        METAL,
        CARPET
    }

    //The sound radius is modified by the mass of the object
    //private float fMassModifier;
   
    //The range of the sound produced per collision
    private float fAudioRange;

    //Volume of the noise to compete with passive noises from machinery etc.
    private float m_fVolume = 0.0f;

    //Time between steping sounds
    private float fStepDistance = 0.0f;

    //Has the collision occured
    private bool bCollision = false;

    //An array of the ai agents that have listeners around the scene
    private List<GameObject> a_goAIObjects;

    //Current Floor Object in contact with - JG
    private GameObject m_goContactedFloor;

    //PlayerController script - JG
    private PlayerController m_Playercontroller;

    //AIAgent script - HW
    private AI_Agent m_AIAgent;

    //Bool for when Co-Routine in effect- JG
    private bool m_bStepAudioActive;

    //The base radius of the sound made when an object collision occurs
    //I plan on making this constant later
    [SerializeField] private float fObjectAudioRadius;

    //The mass of an object alters the radius that the sound can be heard from
    //I am undecided if this will be a standalone value of this script or if I will tie this with the rigidbody mass
    [SerializeField] private float fObjectMass;

    LayerMask FloorLayer;

    //Footstep audio sources that will emit a footstep with every sound burst
    //Wood
    [SerializeField] private AudioSource Source;

    [SerializeField] private AudioClip[] a_Clips;

    Func<bool> MovementFunction;

    //Sound Output - CL
    float[,] audioGrid;
    float fStorage;
    [SerializeField] float fOutputMultiplier; //value to multiply the range by to determine the range on the output
    [Range(0.001f, 1f)]
    [SerializeField] float fSoundFade; //how fast will the sound output fade?
    [SerializeField] Texture2D outputImage;
    [SerializeField] Image hudImage;
    [Range(0, 1)]
    [SerializeField] float fOuterLayer = 0.4f;
    [Range(0, 1)]
    [SerializeField] float fMiddleLayer = 0.7f;


    [SerializeField] float fCrouchMod = 0.4f;
    [SerializeField] float fSprintMod = 1.4f;
    [SerializeField] float fCrouchModVol = 0.3f;
    [SerializeField] float fSprintModVol = 1.0f;
    [SerializeField] float fDefaultVol = 0.7f;

    private void Start()
    {
        FloorLayer = LayerMask.GetMask("Level Geometry");

        //At the initialisation of the scene, add all of the AI objects to the list of AI agents
        a_goAIObjects = new List<GameObject>();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("AIAgent"))
        {
            a_goAIObjects.Add(go);
        }

        //find and assign the playercontroller if not throw exception
        try
        {
            m_Playercontroller = FindObjectOfType<PlayerController>();
            MovementFunction = m_Playercontroller.GetInMotion;
        }
        catch (System.Exception)
        {

            throw new System.Exception("PlayerController script not found");
        }

        if (this.transform.CompareTag("Player"))
        {
            //initiate sound output - CL
            audioGrid = new float[128, 128];
            outputImage = new Texture2D(128, 128);
            for (int x = 0; x < 128; x++)
            {
                for (int y = 0; y < 128; y++)
                {
                    Color color = Color.white;
                    color.a = audioGrid[x, y];
                    outputImage.SetPixel(x, y, color);
                }
            }
            outputImage.Apply();
            hudImage.sprite = Sprite.Create(outputImage, new Rect(0, 0, 128, 128), new Vector2(1, 1));
        }

        if (this.gameObject.tag == "AIAgent")
        {
            try
            {
                m_AIAgent = this.GetComponent<AI_Agent>();
                MovementFunction = m_AIAgent.GetIsMoving;
            }
            catch (System.Exception)
            {
                throw new System.Exception("AI_Agent script not found");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(this.tag == "Decoy" && collision.gameObject.tag == "GrenadeNet")
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, collision.gameObject.transform.position.y + (collision.gameObject.transform.localScale.y / 2) + 0.01f, gameObject.transform.position.z);
        }
        else if (this.tag == "Decoy")
        {
            StartCoroutine(DecoyGrenade());
        }
        else if (this.tag != "Player")
        {
            //The value that the sound radius is modified by depending on the material that the object colided with
            float fFloorMod;

            //A switch to choose which modifier to use
            switch (collision.gameObject.transform.tag)
            {
                case "Wood":
                    fFloorMod = WOODAUDIOMODIFIER;
                    break;

                case "Carpet":
                    fFloorMod = CARPETAUDIOMODIFIER;
                    break;

                case "Metal":
                    fFloorMod = METALAUDIOMODIFIER;
                    break;

                //More Modifiers can be added if needed
                default:
                    fFloorMod = 0.0f;
                    break;
            }

            //Simple calculation to calculate the size of the audio. I will make it more complex in the future.
            fAudioRange = fObjectAudioRadius * fFloorMod * fObjectMass/* * GetComponent<Rigidbody>().velocity.y*/;
            m_fVolume = fObjectMass * fFloorMod;

            bCollision = true;
        }
    }

    private void Update()
    {
        if (this.transform.CompareTag("Player"))

        {
            //apply decay to the sound output display - CL
            for (int x = 0; x < 128; x++)
            {
                for (int y = 0; y < 128; y++)
                {
                    audioGrid[x, y] -= fSoundFade;
                }
            }
            //display sound output - CL
            for (int x = 0; x < 128; x++)
            {
                for (int y = 0; y < 128; y++)
                {
                    Color color = Color.white;
                    color.a = audioGrid[x, y];
                    outputImage.SetPixel(x, y, color);
                }
            }
            outputImage.Apply();
            hudImage.sprite = Sprite.Create(outputImage, new Rect(0, 0, 128, 128), new Vector2(1, 1));
        }

        if (this.gameObject.tag == "Player" || this.gameObject.tag == "AIAgent")
        {
            if (!m_bStepAudioActive)
            {
                Steps();
            }
            else if(m_bStepAudioActive && !MovementFunction())
            {
                m_bStepAudioActive = false;
            }
        }

        //If there was a colision in the frame then make the sound
        if (!gameObject.CompareTag("AIAgent"))
        {
            if (bCollision)
            {
                //Loop through all of the AI objects
                foreach (GameObject go in a_goAIObjects)
                {
                    //The distance between the object and the drone and the total length of the sound produced and the drone audio range
                    float fDistance = Vector3.Distance(transform.position, go.transform.position);
                    float fCombinedLength = go.GetComponent<AudioRange>().GetAudioRange() + fAudioRange;
                    Vector3 DirectionVector = go.transform.position - this.transform.position;

                    //Audio Occlusion. If the raycast hits a wall between the sound origin and the AI then the sound will be supressed
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, DirectionVector, out hit, fAudioRange, FloorLayer))
                    {
                        //Drastically reduce the range of the sound if the AI is behind a wall
                        if (Vector3.Distance(hit.point, go.transform.position) <= OCCLUSIONWALLDISTANCE)
                        {
                            if (this.gameObject.tag != "AIAgent")
                            {
                                if (m_fVolume > go.GetComponent<AudioRange>().GetHighestVol())
                                {
                                    if (this.gameObject.tag == "Player")
                                    {
                                        if (m_Playercontroller.GetInMotion())
                                        {
                                            //Debug.Log("Heard");
                                            //Debug.Log(fCombinedLength.ToString());
                                            go.GetComponent<AI_Agent>().HeardSound(this.transform.position,false);
                                        }
                                    }
                                    else
                                    {
                                       // Debug.Log("Heard");
                                       // Debug.Log(fCombinedLength.ToString());
                                        go.GetComponent<AI_Agent>().HeardSound(this.transform.position,true);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //If the distance between the drone and the sound producing object is less than the total combined length of audio ranges then the sound was heard
                        if (fDistance <= fCombinedLength)
                        {
                            if (this.gameObject.tag != "AIAgent")
                            {
                                if (m_fVolume > go.GetComponent<AudioRange>().GetHighestVol())
                                {
                                    if (this.gameObject.tag == "Player")
                                    {

                                        //Debug.Log("Heard");
                                        //Debug.Log(fAudioRange.ToString());
                                        go.GetComponent<AI_Agent>().HeardSound(this.transform.position,false);

                                        if (m_Playercontroller.GetInMotion())
                                        {
                                            //Debug.Log("Heard");
                                            //Debug.Log(fAudioRange.ToString());
                                            go.GetComponent<AI_Agent>().HeardSound(this.transform.position,false);
                                        }

                                    }
                                }
                                else
                                {
                                    //Debug.Log("Heard");
                                    //Debug.Log(fAudioRange.ToString());
                                    go.GetComponent<AI_Agent>().HeardSound(this.transform.position,true);
                                }
                            }
                            //If the sound was heard, then the behaviour of the AI will change to suspicious
                            //Debug.Log("Guard " + go.GetComponent<AudioRange>().GetID() + " can hear");
                        }
                    }
                    //fAudioRange = 0.0f;
                }
                //Disable the sound sphere after the collision
                bCollision = false;
            }
            else if(!gameObject.CompareTag("Player"))
            {
                //Ensure that the object does not make a sound if there is no collision
                //fAudioRange = 0.0f;
            }
        }

    }

    //If the object that the script is attached to is a player or an AI agent, make steping sounds
    public void Steps()
    {
        //If the object is an AI agent then play the sounds
        //If the object is a player, then the 
        if (gameObject.CompareTag("AIAgent"))
        {
            fStepDistance = 1.0f;
        }
        else
        {
            fStepDistance = 0.5f;
        }

        if(!m_bStepAudioActive && MovementFunction())
        {
            StartCoroutine(StepSound(fStepDistance));
        }
        else
        {
            m_bStepAudioActive = false;
            StopAllCoroutines();
            fAudioRange = 0.0f;
        }
    }

    IEnumerator StepSound(float a_fStepTiming)
    {
        //The Object is now making a sound
        m_bStepAudioActive = true;

        //The material that the object is stepping on and a raycast to detect this
        string mat = "null";
        RaycastHit hit;

        do
        {
            //Simple calculation to calculate the size of the audio. I will make it more complex in the future.
            fAudioRange = fObjectAudioRadius * fObjectMass/* * GetComponent<Rigidbody>().velocity.y*/;
            m_fVolume = fObjectMass;

            //Range modifiers for the player's step sound and volume if they are crouching or sprinting
            if (gameObject.CompareTag("Player"))
            {

                if (m_Playercontroller.GetIsCrouched())
                {
                    fAudioRange *= fCrouchMod;
                    Source.volume = fCrouchModVol;
                    m_fVolume *= fCrouchMod;
                }
                else if (m_Playercontroller.GetIsSprinting())
                {
                    fAudioRange *= fSprintMod;
                    Source.volume = fSprintModVol;
                    m_fVolume *= fSprintMod;
                }
                else
                {
                    Source.volume = fDefaultVol;
                }

                fStorage = fAudioRange; //store current audio range for audio visualiser
            }

            //Check the floor to see what material it is
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), Vector3.down, out hit, float.PositiveInfinity, FloorLayer))
            {
                mat = hit.collider.gameObject.tag;

                //Debug.Log(mat.ToString());
            }

            //The modifier for the floor
            float fFloorMod = 1.0f;

            //Play a sound that coresponds with the material underfoot and if a player, make the range for the AI to hear
            switch (mat)
            {
                case "Wood":
                    if (a_Clips.Length > 0)
                    {
                        fFloorMod = WOODAUDIOMODIFIER;
                        Source.clip = a_Clips[UnityEngine.Random.Range(0, 3)];
                        Source.Play();
                    }
                    break;

                case "Metal":
                    if (a_Clips.Length > 0)
                    {
                        fFloorMod = METALAUDIOMODIFIER;
                        Source.clip = a_Clips[UnityEngine.Random.Range(3, 6)];
                        Source.Play();
                    }
                    break;

                case "Carpet":
                    if (a_Clips.Length > 0)
                    {
                        fFloorMod = CARPETAUDIOMODIFIER;
                        Source.clip = a_Clips[UnityEngine.Random.Range(6, 9)];
                        Source.Play();
                    }
                    break;

                default:
                    break;
            }

            m_fVolume *= fFloorMod;

            fAudioRange *= fFloorMod;

            if (gameObject.CompareTag("Player"))
            {
                bCollision = true;
                Visualiser();
            }

            yield return new WaitForSeconds(a_fStepTiming);
        } while (MovementFunction());

        yield return null;
    }

    private void Visualiser()
    {

        //Update audio display - CL
        //get range
        int outputRadius = (int)(fStorage * fOutputMultiplier); //not sure how big the value will be so i'll use a multiplier to make it lower/higher as needed
                                                                //make a circle on the grid with the centre at 1 and other values slowing down until it reaches the radius
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                float pythag = Mathf.Sqrt(Mathf.Pow(63 - x, 2) + Mathf.Pow(63 - y, 2));
                if (pythag <= outputRadius)
                {
                    //start layering effect
                    if (pythag <= outputRadius / 3)
                    {
                        float value = (outputRadius - pythag) / outputRadius;
                        value = Mathf.Clamp(value, fMiddleLayer, 1);
                        audioGrid[x, y] = value;
                    }
                    else if (pythag > outputRadius / 3 && pythag <= (2 * outputRadius) / 3)
                    {
                        audioGrid[x, y] = fMiddleLayer;
                    }
                    else
                    {
                        audioGrid[x, y] = fOuterLayer;
                    }
                }
            }
        }

        for (int x = 64; x < 128; x++)
        {
            for (int y = 0; y < 64; y++)
            {
                float pythag = Mathf.Sqrt(Mathf.Pow(63 - x, 2) + Mathf.Pow(63 - y, 2));
                if (pythag <= outputRadius)
                {
                    //start layering effect
                    if (pythag <= outputRadius / 3)
                    {
                        float value = (outputRadius - pythag) / outputRadius;
                        value = Mathf.Clamp(value, fMiddleLayer, 1);
                        audioGrid[x, y] = value;
                    }
                    else if (pythag > outputRadius / 3 && pythag <= (2 * outputRadius) / 3)
                    {
                        audioGrid[x, y] = fMiddleLayer;
                    }
                    else
                    {
                        audioGrid[x, y] = fOuterLayer;
                    }
                }
            }
        }

        for (int x = 0; x < 64; x++)
        {
            for (int y = 64; y < 128; y++)
            {
                float pythag = Mathf.Sqrt(Mathf.Pow(63 - x, 2) + Mathf.Pow(63 - y, 2));
                if (pythag <= outputRadius)
                {
                    //start layering effect
                    if (pythag <= outputRadius / 3)
                    {
                        float value = (outputRadius - pythag) / outputRadius;
                        value = Mathf.Clamp(value, fMiddleLayer, 1);
                        audioGrid[x, y] = value;
                    }
                    else if (pythag > outputRadius / 3 && pythag <= (2 * outputRadius) / 3)
                    {
                        audioGrid[x, y] = fMiddleLayer;
                    }
                    else
                    {
                        audioGrid[x, y] = fOuterLayer;
                    }
                }
            }
        }

        for (int x = 64; x < 128; x++)
        {
            for (int y = 64; y < 128; y++)
            {
                float pythag = Mathf.Sqrt(Mathf.Pow(63 - x, 2) + Mathf.Pow(63 - y, 2));
                if (pythag <= outputRadius)
                {
                    //start layering effect
                    if (pythag <= outputRadius / 3)
                    {
                        float value = (outputRadius - pythag) / outputRadius;
                        value = Mathf.Clamp(value, fMiddleLayer, 1);
                        audioGrid[x, y] = value;
                    }
                    else if (pythag > outputRadius / 3 && pythag <= (2 * outputRadius) / 3)
                    {
                        audioGrid[x, y] = fMiddleLayer;
                    }
                    else
                    {
                        audioGrid[x, y] = fOuterLayer;
                    }
                }
            }
        }
        return;
    }

    IEnumerator DecoyGrenade()
    {
        for(int i = 0; i < DECOYBURSTCOUNTER; i++ )
        {
            fAudioRange = DECOYAUDIODISTANCE;
            bCollision = true;
            Source.Play();
            yield return new WaitForSeconds(DECOYBURSTTIMER);
        }

        Destroy(this.gameObject);
    }

    //Draw a gizmo to display the audio ranges of the objects
    private void OnDrawGizmosSelected()
    {

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, fAudioRange);
    }
}
