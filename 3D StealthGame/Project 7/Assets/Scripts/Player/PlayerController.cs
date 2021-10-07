using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.AI;
using UnityEngine.UI;

////////////////////////////////////////////////////////////
// File: PlayerController.cs
// Author: Cameron Lillie
// Brief: Controller script for player movement and interactions
////////////////////////////////////////////////////////////

//TO DO: Perhaps make crouching and ducking more fluid. Ask others/Will for feedback

public class PlayerController : MonoBehaviour, Controls.IGameplayActions
{
    #region VARIABLES

    //Game Manager
    [Header("Manager")]
    [SerializeField] private GameObject m_goGameManager;

    //Health
    [Header("Health")]
    [SerializeField] private int iHealth;
    private int iMaxHealth = 3;

    //Health UI elements
    [Header("UI Elements")]
    [SerializeField]
    private GameObject[] a_goHealthIcons;

    [SerializeField]
    private GameObject[] a_goItemIcons;

    [SerializeField]
    private GameObject goItemTextBox;

    [SerializeField]
    private GameObject[] a_goItemsInHand;

    //Death screen for when the player dies
    [SerializeField]
    private GameObject goDeathScreen;

    //Win screen if the player wins
    [SerializeField]
    private GameObject goWinScreen;

    [SerializeField]
    private GameObject goPressQReminder;

    //Pause Screen
    [SerializeField]
    private GameObject goPauseScreen;

    //Player's inventory
    private Inventory inventory;

    private Controls controls; //Reference to the input system
    private Rigidbody rigidbody; //rigidbody attached to the player
    private bool bIsGrounded;

    //colliders
    [Header("Colliders")]
    [SerializeField]
    private CapsuleCollider normalCollider;

    [SerializeField]
    private CapsuleCollider crouchedCollider;

    [SerializeField]
    private Collider cameraCollider;

    private float fHorizontal; //horizontal input
    private float fVertical; //vertical input
    private float fCameraHorizontal; //camera horizontal input
    private float fCameraVertical; //camera vertical input
    private float fMoveValue; //movement value
    private Vector3 v3MoveDirection; //direction the player will move in

    [Header("Camera")]
    [SerializeField]
    private Camera camera; //reference to the camera

    //movement
    [Header("Movement Stats")]
    private float fMoveSpeed = 2.4f;
    private float fCrawlSpeed = 1.6f;
    private float fSprintSpeed = 6.0f;
    [SerializeField] private float fSpikeMultiplier = 0.4f;
    [SerializeField] private bool bOnSpikes;
    private bool bSprinting;
    private float fStamina = 100f;
    private float fMaxStamina = 100f;
    private float fStaminaDrain = 10f;
    private float fStaminaRecovery = 2.5f;
    private float fRotateSpeed = 0.2f;

    private bool m_bTut;

    //camera clamping
    private float fCameraUpClamp = 90f;
    private float fCameraDownClamp = -90f;

    //hidden camera variables
    private Vector2 v2HidingCameraMinClamp;
    private Vector2 v2HidingCameraMaxClamp;
    private Vector2 v2HidingCameraLeanClamp; //no need for min/max clamp since it's symmetrical
    private bool bCanCrouchWhenHiding;

    //crouching
    private bool bIsCrouched;
    private bool bIsUnderObject;
    private float fCrouchHeight = 1f; //distance to lower the camera (find way to automatically set this?)
    private float fDuckHeight = 0.1f; //distance to lower the camera when ducking down under an object

    //crouch/duck smoothing
    float fCrouchTime = 0.5f; //time for crouching
    float fDuckTime = 0.1f; //time for ducking
    float fStartTime; //time at start
    float fFracComplete;

    Vector3 v3CameraPosition; //current position of the camera

    public int iPlayerLayer;

    private float fHSensitivity = 1f; //Horizontal Camera Sensitivity
    private float fVSensitivity = 0.1f; //Vertical     "         "

    //Leaning
    private bool bLeaning;
    [Header("Lean Pivot")]
    [SerializeField] private GameObject goLeanPivot;


    //Interaction Variables
    private float fInteractDistance = 1f; //distance the player can interact from
    private float fInteractAngle = 40f; //angle for checking if an object is close to the centre of the screen
    private bool bCanInteract; //Can the player interact?
    private bool bInteractHeld; //Is the interact button held?
    [SerializeField] private float fInteractHoldTime; //Time required for the current interaction to complete
    [SerializeField] private GameObject currentInteractable; //the current interactable object that can be interacted with

    //Hiding
    private bool bHiding;
    private GameObject goBoxView; //Box object attached to player's head

    //Inventory Items
    private bool bUseHeld;
    private bool bGrenadeAiming;
    [Header("Inventory Prefabs")]
    [SerializeField] private GameObject goSmokeGrenadePrefab;
    [SerializeField] private GameObject goStunGrenadePrefab;
    [SerializeField] private GameObject goDecoyPrefab;
    [Header("Grenade Start Point")]
    [Tooltip("Empty GameObject marking position the grenade spawns and is thrown from.")]
    [SerializeField] private GameObject goGrenadeStart;
    private float fGrenadeThrowForce = 6f;

    //LeanModifiers
    [Header("Leaning Modifiers")]
    [SerializeField]
    private float fLeanSpeedMod = 0.01f;

    [SerializeField]
    private float fLeanRotateMod = 0.5f;

    [SerializeField]
    private float fLeanRestriction = 0.3f;

    private bool bIsGrabbed;

    //Leaning values
    private bool m_bInMotion;
    private bool m_bMouseLock;
    private Vector2 m_v2MouseLock;
    private float m_fLeanMax = 1f;
    private float Difference;
    private float TiltDifference;
    [SerializeField] private float fHorizontalLean;
    [SerializeField] private float fVerticalLean;
    [SerializeField] private float fLean;
    [SerializeField] private float fRotation;

    private const float MAXBREAKOUTPRESSES = 10;
    private float fGrabBreakoutPresses;

    private LayerMask AILayer;

    private bool bIsInSmoke;
    private bool bHasWon;

    private bool bInteractWait;

    private bool m_bFloorCols = false;

    //private LineRenderer m_Aimline;
    private CylinderLine m_LineRenderer;

    [SerializeField] private Material m_LineMaterial;

    #endregion VARIABLES

    // Start is called before the first frame update
    public void Start()
    {
        controls = new Controls();
        controls.Gameplay.SetCallbacks(this);
        controls.Enable();
        rigidbody = GetComponent<Rigidbody>();
        normalCollider.enabled = true;
        crouchedCollider.enabled = false;
        inventory = GetComponent<Inventory>();
        Physics.IgnoreCollision(cameraCollider, normalCollider, true);
        Physics.IgnoreCollision(cameraCollider, crouchedCollider, true);
        m_bFloorCols = false;

        //m_Aimline = this.gameObject.AddComponent<LineRenderer>();

        m_LineRenderer = GetComponent<CylinderLine>();

        bHasWon = false;

        AILayer = LayerMask.GetMask("AI");

        goItemTextBox.GetComponent<Text>().text = inventory.GetMedkitStock().ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (iHealth > 0 && !goPauseScreen.activeSelf && !bHasWon)
        {
            if (!m_bTut)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if(bGrenadeAiming)
        {
            if(m_LineRenderer.enabled == false)
            {
                //m_Aimline.enabled = true;
                m_LineRenderer.enabled = true;
            }

            AimLine();
        }

        //Is the player under an object right now?
        DuckCamera();

        if (!bIsGrabbed)
        {
            goItemTextBox.GetComponent<Text>().text = inventory.GetCurrentStock().ToString();

            bLeaning = (controls.Gameplay.Lean.ReadValue<float>() >= 0.8f);

            if (bLeaning)
            {
                if (!m_bMouseLock)
                {
                    m_bMouseLock = true;

                    m_v2MouseLock = controls.Gameplay.MousePos.ReadValue<Vector2>();
                }
            }
            else
            {
                m_bMouseLock = false;
            }

            if (!bIsCrouched && fStamina > 0)
            {
                bSprinting = (controls.Gameplay.Sprint.ReadValue<float>() >= 0.8f);
            }
            else
            {
                bSprinting = false;
            }

            if (fStamina < fMaxStamina && !(bSprinting && m_bInMotion))
            {
                fStamina += fStaminaRecovery * Time.deltaTime;
                if (fStamina > fMaxStamina)
                {
                    fStamina = fMaxStamina;
                }
            }

            if (bSprinting && fStamina > 0 && m_bInMotion)
            {
                fStamina -= fStaminaDrain * Time.deltaTime;
                if (fStamina < 0)
                {
                    fStamina = 0;
                }
            }

            //Move player in 2 axes based on WASD keys or whatever's set to Move
            Movement();

            //Leaning
            Leaning();

            //Interactions 
            Interact();

            //aiming grenade
            if (bGrenadeAiming)
            {
                //do aiming stuff (e.g. change cursor, animate arms, etc.)
            }
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
        }
    }

    #region FIXED UPDATE FUNCTIONS

    void Movement()
    {
        if(bHiding)
        {
            m_bInMotion = false;
        }

        rigidbody.isKinematic = bLeaning;
        if (!bLeaning && !bHiding)
        {
            if (controls.Gameplay.Move.ReadValue<Vector2>() != null)
            {
                fHorizontal = controls.Gameplay.Move.ReadValue<Vector2>().x;
                fVertical = controls.Gameplay.Move.ReadValue<Vector2>().y;
            }
            fMoveValue = Mathf.Clamp01(Mathf.Abs(fHorizontal) + Mathf.Abs(fVertical));
            v3MoveDirection = gameObject.transform.forward * fVertical;
            v3MoveDirection += gameObject.transform.right * fHorizontal;

            if (!m_bFloorCols)
            {
                v3MoveDirection += gameObject.transform.up * -10f;
            }

            //RaycastHit hit;
            //Debug.DrawRay(transform.position, Vector3.down, Color.cyan);
            //if (Physics.SphereCast(new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), 0.2f, Vector3.down, out hit, 0.2f))
            //{
            //    transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            //}
            //else
            //{
            //    v3MoveDirection += gameObject.transform.up * -50f;
            //}


            v3MoveDirection.Normalize();

            if (fHorizontal != 0 || fVertical != 0)
            {
                m_bInMotion = true;
            }
            else
            {
                m_bInMotion = false;
            }

            

            float delta = Time.deltaTime;

            if (bIsCrouched)
            {
                rigidbody.velocity = v3MoveDirection * (bOnSpikes ? fCrawlSpeed * fSpikeMultiplier : fCrawlSpeed);
            }
            else if (bSprinting)
            {
                rigidbody.velocity = v3MoveDirection * (bOnSpikes ? fSprintSpeed * fSpikeMultiplier : fSprintSpeed);
            }
            else if (bLeaning || bIsGrabbed)
            {
                rigidbody.velocity = Vector3.zero;
            }
            else
            {
                rigidbody.velocity = v3MoveDirection * (bOnSpikes ? fMoveSpeed * fSpikeMultiplier : fMoveSpeed);
            }
        }

        if (!bLeaning)
        {
            //Rotate player based on mouse Y movement
            fCameraHorizontal = controls.Gameplay.Camera.ReadValue<Vector2>().x * fHSensitivity;
            gameObject.transform.Rotate(Vector3.up, fCameraHorizontal * fRotateSpeed);

            //Rotate camera up and down (find way to clamp camera)
            fCameraVertical += controls.Gameplay.Camera.ReadValue<Vector2>().y * fVSensitivity;

            //clamp camera if the player is hiding
            if (bHiding)
            {
                Vector3 currentRot = gameObject.transform.localRotation.eulerAngles;
                currentRot.y = NegativeAngleClamp(currentRot.y, v2HidingCameraMinClamp.x, v2HidingCameraMaxClamp.x);

                gameObject.transform.localRotation = Quaternion.Euler(currentRot);
                fCameraVertical = Mathf.Clamp(fCameraVertical, v2HidingCameraMinClamp.y, v2HidingCameraMaxClamp.y);
            }
            else
            {
                fCameraVertical = Mathf.Clamp(fCameraVertical, fCameraDownClamp, fCameraUpClamp);
            }
            camera.transform.localRotation = Quaternion.Euler(fCameraVertical, 0, 0);
            camera.transform.position = goLeanPivot.transform.position;
        }
    }
    
    void Leaning()
    {
        if (!bHiding)
        {
            if (bLeaning)
            {
                m_bInMotion = false;

                if (iHealth > 0 && !goPauseScreen.activeSelf && !bHasWon)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = false;
                }

                Vector3 NewHorizontal = Vector3.zero;
                Vector3 NewVertical = Vector3.zero;
                Vector3 inputVec3 = Vector3.zero;
                Vector3 dirVec = Vector3.zero;
                Quaternion NewRotation = Quaternion.identity;
                float clamp = 0f;

                Vector2 input = controls.Gameplay.MousePos.ReadValue<Vector2>();


                //get direction the player wants to lean in
                //convert input vector to world
                inputVec3 += goLeanPivot.transform.right * (input.x - m_v2MouseLock.x);
                inputVec3 += goLeanPivot.transform.up * (input.y - m_v2MouseLock.y);

                //get direction of this vector
                dirVec = inputVec3.normalized;

                //spherecast in that direction to set clamp
                if (!bHiding)
                {
                    RaycastHit hit;
                    int layerMask;
                    layerMask = 1 << 10;
                    layerMask = ~layerMask;
                    if (Physics.SphereCast(goLeanPivot.transform.position, 0.1f, dirVec, out hit, m_fLeanMax, layerMask))
                    {
                        clamp = hit.distance - 0.1f;
                    }
                    else
                    {
                        clamp = m_fLeanMax;
                    }
                }
                else
                {
                    clamp = v2HidingCameraLeanClamp.x;
                }

                //move camera accordingly
                Difference = Vector2.Distance(m_v2MouseLock, input);
                TiltDifference = Mathf.Abs(goLeanPivot.transform.localPosition.x - camera.transform.localPosition.x) * Mathf.Sign(input.x - m_v2MouseLock.x) * -1f;

                fLean = Difference * fLeanSpeedMod;
                fRotation = TiltDifference * fLeanRotateMod;

                fLean = Mathf.Clamp(fLean, 0, clamp);
                fRotation = Mathf.Clamp(fRotation, -0.2f, 0.2f);

                NewRotation = new Quaternion(camera.transform.localRotation.x, camera.transform.localRotation.y, fRotation, 1.0f);

                Vector3 leanVector = dirVec * fLean;

                camera.transform.position = new Vector3(goLeanPivot.transform.position.x, goLeanPivot.transform.position.y, goLeanPivot.transform.position.z) + leanVector;

                camera.transform.localRotation = new Quaternion(camera.transform.localRotation.x, camera.transform.localRotation.y, NewRotation.z, camera.transform.localRotation.w);
            }
        }
    }

    void Interact()
    {
        if (!bHiding)
        {
            Collider[] colliders = Physics.OverlapSphere(camera.transform.position, 1);

            for (int i = 0; i < colliders.Length; i++)
            {
                Interactable interact = colliders[i].transform.GetComponentInParent<Interactable>();
                if (interact != null)
                {
                    if (!bInteractHeld)
                    {
                        if (RaycastToInteractable(interact))
                        {
                            if (currentInteractable == null)
                            {
                                currentInteractable = interact.gameObject;
                                fInteractHoldTime = interact.GetInteractTime();
                            }
                            else
                            {
                                currentInteractable = interact.gameObject;
                                fInteractHoldTime = interact.GetInteractTime();

                            }

                            bCanInteract = true;
                        }
                        else
                        {
                            if (currentInteractable != null)
                            {
                                currentInteractable = null;
                                fInteractHoldTime = 0.0f;
                            }
                            bCanInteract = false;
                        }
                    }
                }
            }

            if (currentInteractable != null)
            {
                float InteractDistance = 2.0f;

                Vector3 InteractableVector = camera.transform.position - currentInteractable.transform.GetChild(0).gameObject.transform.position;

                float distance = InteractableVector.magnitude;

                if (distance > InteractDistance)
                {
                    currentInteractable = null;
                }
            }

            if (bCanInteract && bInteractHeld && currentInteractable != null)
            {
                if (fInteractHoldTime > 0.0f)
                {
                    fInteractHoldTime -= Time.deltaTime;
                }
                else if (fInteractHoldTime <= 0.0f)
                {
                    if (!bInteractWait)
                    {
                        currentInteractable.GetComponent<Interactable>().Interact();

                        bInteractHeld = false;

                        bInteractWait = true;

                        StartCoroutine(InteractWait());
                    }
                }
            }
            else if (bCanInteract && !bInteractHeld && currentInteractable != null)
            {
                //reset timer
                fInteractHoldTime = currentInteractable.GetComponent<Interactable>().GetInteractTime();
            }
        }
        else
        {
            //no need to check if the player can interact since they can while in the box
            if (bInteractHeld && currentInteractable != null)
            {
                if (!bInteractWait)
                {
                    if (fInteractHoldTime > 0.0f)
                    {
                        fInteractHoldTime -= Time.deltaTime;
                    }
                    else if (fInteractHoldTime <= 0.0f)
                    {
                        currentInteractable.GetComponent<Interactable>().Interact();
                        bInteractHeld = false;
                    }
                }
            }
            else if (currentInteractable == null)
            {
                Debug.LogError("Player is hiding but the box isn't set as the current interactable");
            }
            else if (!bInteractHeld && currentInteractable != null)
            {
                //reset timer
                fInteractHoldTime = currentInteractable.GetComponent<Interactable>().GetInteractTime();
            }
        }
    }
    void DuckCamera()
    {
        if (bIsCrouched)
        {
            RaycastHit hit;
            int layerMask = (1 << iPlayerLayer);
            layerMask = ~layerMask;
            float distance = fDuckHeight;

            //Debug.DrawRay(camera.transform.position, (Vector3.up * (distance * 5.0f)),Color.red);

            

            //if raycast is too long?
            if (Physics.Raycast(camera.transform.position, Vector3.up, out hit, (distance * 5.0f), layerMask))
            {
                if (!bIsUnderObject)
                {
                    //if the player is under an object, set to true
                    bIsUnderObject = true;
                    //duck camera down
                    goLeanPivot.transform.position = new Vector3(goLeanPivot.transform.position.x, goLeanPivot.transform.position.y - fDuckHeight, goLeanPivot.transform.position.z);
                    camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y - fDuckHeight, camera.transform.position.z);
                }

            }
            else
            {
                if (bIsUnderObject)
                {
                    //if the player isn't under an object, then set to false
                    bIsUnderObject = false;
                    //bring camera back up
                    goLeanPivot.transform.position = new Vector3(goLeanPivot.transform.position.x, goLeanPivot.transform.position.y + fDuckHeight, goLeanPivot.transform.position.z);
                    camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y + fDuckHeight, camera.transform.position.z);
                }

            }
        }
        else
        {
            if (bIsUnderObject)
            {
                //if the player isn't crouching, reset this value just in case
                bIsUnderObject = false;
                //bring camera back up
                goLeanPivot.transform.position = new Vector3(goLeanPivot.transform.position.x, goLeanPivot.transform.position.y + fDuckHeight, goLeanPivot.transform.position.z);
                camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y + fDuckHeight, camera.transform.position.z);
            }
        }
    }

    #endregion FIXED UPDATE FUNCTIONS

    #region MISC FUNCTIONS

    private bool RaycastToInteractable(Interactable interact)
    {


        //Vector3 direction = interact.transform.position - camera.transform.position;
        Vector3 direction = interact.GetTargetObject().transform.position - camera.transform.position;

        direction.Normalize();
        float angle = Vector3.Angle(camera.transform.forward, direction);

        if (angle < fInteractAngle)
        {
            Vector3 origin = camera.transform.position;

            Debug.DrawRay(origin, direction * 20, Color.red);

            Ray InteractRay = new Ray(origin, direction);

            LayerMask mask = LayerMask.GetMask("Level Geometry");

            RaycastHit hit;

            if (Physics.Raycast(InteractRay, out hit, 20,mask,QueryTriggerInteraction.Collide))
            {



                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void Crouch()
    {
        //crouch
        //StartCoroutine(SmoothCrouch(-fCrouchHeight, fCrouchTime));
        goLeanPivot.transform.position = new Vector3(goLeanPivot.transform.position.x, goLeanPivot.transform.position.y - fCrouchHeight, goLeanPivot.transform.position.z);
        camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y - fCrouchHeight, camera.transform.position.z);
        normalCollider.enabled = false;
        m_bFloorCols = false;
        crouchedCollider.enabled = true;
        bIsCrouched = true;
    }

    public void Uncrouch()
    {
        //uncrouch
        //if there's an object above the player, don't uncrouch
        if (!bIsUnderObject)
        {
            //StartCoroutine(SmoothCrouch(fCrouchHeight, fCrouchTime));
            goLeanPivot.transform.position = new Vector3(goLeanPivot.transform.position.x, goLeanPivot.transform.position.y + fCrouchHeight, goLeanPivot.transform.position.z);
            camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y + fCrouchHeight, camera.transform.position.z);
            crouchedCollider.enabled = false;
            m_bFloorCols = false;
            normalCollider.enabled = true;
            bIsCrouched = false;
        }
    }

    //IEnumerator SmoothCrouch(float crouchHeight, float crouchTime)
    //{
    //    float timer = 0;
    //    Vector3 pivotSource = goLeanPivot.transform.position;
    //    Vector3 pivotTarget = new Vector3(goLeanPivot.transform.position.x, goLeanPivot.transform.position.y + crouchHeight, goLeanPivot.transform.position.z);
    //    Vector3 cameraSource = camera.transform.position;
    //    Vector3 cameraTarget = new Vector3(camera.transform.position.x, camera.transform.position.y + crouchHeight, camera.transform.position.z);
    //    while (timer < crouchTime)
    //    {
    //        timer += Time.deltaTime;
    //        fFracComplete = timer / crouchTime;
    //        goLeanPivot.transform.position = Vector3.Lerp(pivotSource, pivotTarget, fFracComplete);
    //        camera.transform.position = Vector3.Lerp(cameraSource, cameraTarget, fFracComplete);
    //        yield return new WaitForFixedUpdate();
    //    }
    //    goLeanPivot.transform.position = new Vector3(goLeanPivot.transform.position.x, goLeanPivot.transform.position.y + crouchHeight, goLeanPivot.transform.position.z);
    //    camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y + crouchHeight, camera.transform.position.z);
    //}

    private void OnCollisionStay(Collision collision)
    {
        if(collision.transform.tag == "Wood" || collision.transform.tag == "Metal" || collision.transform.tag == "Carpet")
        {
            m_bFloorCols = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.tag == "Wood" || collision.transform.tag == "Metal" || collision.transform.tag == "Carpet")
        {
            //m_iFloorCols;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Wood" || collision.transform.tag == "Metal" || collision.transform.tag == "Carpet")
        {
            m_bFloorCols = false;
        }
    }

    public bool GetInMotion()
    {
        return m_bInMotion;
    }

    public void TakeDamage()
    {
        a_goHealthIcons[iHealth - 1].SetActive(false);
        --iHealth;

        if (iHealth <= 0)
        {
            Time.timeScale = 0.0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            goDeathScreen.SetActive(true);
        }
    }

    public void KillPlayer()
    {
        iHealth = 0;

        Time.timeScale = 0.0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        goDeathScreen.SetActive(true);
    }



    public IEnumerator InteractWait()
    {
        yield return new WaitForSeconds(0.5f);

        bInteractWait = false;

        yield return null;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "WinZone" && inventory.GetHasObjective())
        {
            goWinScreen.SetActive(true);
            Time.timeScale = 0.0f;

            bHasWon = true;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;


        }
        if(other.gameObject.CompareTag("Tutorial"))
        {
            other.gameObject.GetComponent<Tutorials>().OpenTut();
        }
    }

    public float NegativeAngleClamp(float a_fClampVal, float a_min, float a_max)
    {
        float angleClampMin;

        if (a_min < 0)
        {
            angleClampMin = 360.0f + a_min;

            if (a_fClampVal < angleClampMin && a_fClampVal > 180.0f)
            {

                return angleClampMin;

            }

            if (a_fClampVal > a_max && a_fClampVal < 180.0f)
            {

                a_fClampVal = a_max;

            }
        }
        else
        {
            return Mathf.Clamp(a_fClampVal, a_min, a_max);
        }

        return a_fClampVal;

    }

    public void TraverseOffMesh()
    {

        //if grounded

        //Raycast to the ground in front to see if there is a ledge

        //take the raycast and sample the nearest navmesh position if hit point not already on navmesh

        //start coroutine for Hopoff

    }


    #endregion MISC FUNCTIONS

    #region UNITY INPUT FUNCTIONS

    public virtual void OnMove(InputAction.CallbackContext context)
    {

    }

    public virtual void OnCamera(InputAction.CallbackContext context)
    {

    }

    public virtual void OnSprint(InputAction.CallbackContext context)
    {

    }

    public virtual void OnCrouch(InputAction.CallbackContext context)
    {
        if (this != null)
        {
            //Find a way to make this smoother
            if (!bHiding || bCanCrouchWhenHiding)
            {
                if (!bIsCrouched)
                {
                    Crouch();
                }
                else
                {
                    Uncrouch();
                }
            }
        }
    }
    


    public virtual void OnLean(InputAction.CallbackContext context)
    {

    }

    public virtual void OnInteract(InputAction.CallbackContext context)
    {
        //raycast in front of the player
        //if hits an object, is it an interactable?
        //if so, interact (find a way to make it a hold option?)


        if (context.performed)
        {
            //Debug.Log("Interact Performed");
            bInteractHeld = true;
        }
        if (context.canceled)
        {
            bInteractHeld = false;
        }
    }

    public virtual void OnUseItem(InputAction.CallbackContext context)
    {
        if (this != null)
        {

            Inventory.CURRENT_ITEM currentItem = inventory.GetCurrentItem();
            if (context.performed)
            {

                bUseHeld = true;
                switch (currentItem)
                {
                    case Inventory.CURRENT_ITEM.MEDKIT:
                        //heal player
                        if (inventory.GetMedkitStock() > 0 /*&& bUseHeld*/)
                        {
                            if (iHealth != iMaxHealth)
                            {

                                if (a_goHealthIcons[iHealth] != null)
                                {
                                    a_goHealthIcons[iHealth].SetActive(true);
                                }

                                iHealth += inventory.GetHealAmount();
                                if (iHealth > iMaxHealth)
                                {
                                    iHealth = iMaxHealth;
                                }

                                inventory.SetMedkitStock(inventory.GetMedkitStock() - 1);

                                goItemTextBox.gameObject.GetComponent<Text>().text = inventory.GetMedkitStock().ToString();

                                if(inventory.GetMedkitStock() == 0)
                                {
                                    a_goItemsInHand[1].SetActive(false);
                                    a_goItemsInHand[0].SetActive(true);
                                }

                            }

                        }
                        break;
                    case Inventory.CURRENT_ITEM.SMOKEGRENADE:
                        //throw bomb
                        if (inventory.GetSmokeStock() > 0 && bUseHeld)
                        {
                            //grenade aiming
                            bGrenadeAiming = true;
                        }
                        break;
                    case Inventory.CURRENT_ITEM.STUNGRENADE:
                        if (inventory.GetStunStock() > 0 && bUseHeld)
                        {
                            bGrenadeAiming = true;

                        }
                        break;
                    case Inventory.CURRENT_ITEM.DECOY:
                        if(inventory.GetDecoyStock() > 0 && bUseHeld)
                        {
                            bGrenadeAiming = true;

                        }
                        break;
                    default:
                        Debug.LogError("Current Equipped Item Is Not In The Enum WTF?!");
                        break;
                }
            }
            if (context.canceled)
            {
                bUseHeld = false;
                //throw grenade
                if (bGrenadeAiming)
                {
                    bGrenadeAiming = false;
                    GameObject grenade;
                    Vector3 ThrowForce;
                    //yeet
                    switch (currentItem)
                    {
                        case Inventory.CURRENT_ITEM.MEDKIT:
                            break;
                        case Inventory.CURRENT_ITEM.SMOKEGRENADE:
                            grenade = Instantiate(goSmokeGrenadePrefab, goGrenadeStart.transform.position, Quaternion.identity);
                            ThrowForce = fGrenadeThrowForce * goGrenadeStart.transform.forward;
                            if (grenade.GetComponent<Rigidbody>())
                            {
                                grenade.GetComponent<Rigidbody>().AddForce(ThrowForce);
                                inventory.SetSmokeStock(inventory.GetSmokeStock() - 1);
                                goItemTextBox.gameObject.GetComponent<Text>().text = inventory.GetSmokeStock().ToString();
                            }
                            else
                            {
                                Debug.LogError("Grenade prefab needs a rigidbody.");
                            }

                            if(inventory.GetSmokeStock() == 0)
                            {
                                a_goItemsInHand[2].SetActive(false);
                                a_goItemsInHand[0].SetActive(true);
                            }

                            break;
                        case Inventory.CURRENT_ITEM.STUNGRENADE:
                            grenade = Instantiate(goStunGrenadePrefab, goGrenadeStart.transform.position, Quaternion.identity);
                            ThrowForce = fGrenadeThrowForce * goGrenadeStart.transform.forward;
                            if (grenade.GetComponent<Rigidbody>())
                            {
                                grenade.GetComponent<Rigidbody>().AddForce(ThrowForce);
                                inventory.SetStunStock(inventory.GetStunStock() - 1);
                                goItemTextBox.gameObject.GetComponent<Text>().text = inventory.GetStunStock().ToString();
                            }
                            else
                            {
                                Debug.LogError("Grenade prefab needs a rigidbody.");
                            }

                            if (inventory.GetStunStock() == 0)
                            {
                                a_goItemsInHand[3].SetActive(false);
                                a_goItemsInHand[0].SetActive(true);
                            }

                            break;

                        case Inventory.CURRENT_ITEM.DECOY:
                            grenade = Instantiate(goDecoyPrefab, goGrenadeStart.transform.position, Quaternion.identity);
                            ThrowForce = fGrenadeThrowForce * goGrenadeStart.transform.forward;
                            if(grenade.GetComponent<Rigidbody>())
                            {
                                grenade.GetComponent<Rigidbody>().AddForce(ThrowForce);
                                inventory.SetDecoyStock(inventory.GetDecoyStock() - 1);
                                goItemTextBox.gameObject.GetComponent<Text>().text = inventory.GetDecoyStock().ToString();
                            }
                            else
                            {
                                Debug.LogError("Grenade prefab needs a rigidbody.");
                            }

                            if (inventory.GetDecoyStock() == 0)
                            {
                                a_goItemsInHand[4].SetActive(false);
                                a_goItemsInHand[0].SetActive(true);
                            }

                            break;

                        default:
                            break;
                    }

                    m_LineRenderer.enabled = false;
                    //m_Aimline.enabled = false;               
                }
            }
        }
    }

    public void AimLine()
    {
        int maxIterations = 1000;
        int maxSegmentCount = 300;
        float segmentStepModulo = 2f;
        int numSegments = 0;

        Vector3[] Segments = new Vector3[maxSegmentCount];

        float Drag = 0f;
        float Mass = 0.01f;

        float Timestep = Time.fixedDeltaTime;

        float stepDrag = 1 - Timestep * Drag;

        Vector3 LaunchVelocity =  fGrenadeThrowForce * goGrenadeStart.transform.forward;
        Vector3 velocity = LaunchVelocity / 0.5f * Timestep;


        Vector3 gravity = Physics.gravity * Timestep * Timestep;
        Vector3 position = goGrenadeStart.transform.position;

        Segments[0] = position;
        //Segments[0] = position - goGrenadeStart.transform.forward;

        Vector3 LastPos = Vector3.zero;
        numSegments = 1;

        bool m_bPositionClear = true;

        for (int i = 0; i < maxIterations && numSegments < maxSegmentCount && m_bPositionClear == true; i++)
        {

            velocity += gravity;

            velocity *= stepDrag;

            Vector3 lastPos = position;

            position += velocity;

            float CheckDistance = Vector3.Distance(lastPos, position);

            RaycastHit hitObj;

            int LayerBitmask = LayerMask.GetMask("Grenade","Player");

            LayerBitmask = ~LayerBitmask;

            //Debug.DrawRay(lastPos,position-lastPos,Color.magenta);

            if(Physics.Raycast(lastPos, position - lastPos, out hitObj,CheckDistance,LayerBitmask))
            {
                //Debug.Log(hitObj.collider.gameObject.name.ToString() + " Blocking shot");

                m_bPositionClear = false;

                Segments[numSegments] = position;

                numSegments++;
            }


            if (i % segmentStepModulo == 0)
            {
                Segments[numSegments] = position;
                numSegments++;
            }

        }

        //m_Aimline.positionCount = numSegments;

        Vector3[] NewArray = new Vector3[numSegments];

        for (int i = 0; i < numSegments; i++)
        {
            NewArray[i] = Segments[i];
        }

        m_LineRenderer.material = m_LineMaterial;

        m_LineRenderer.SetPositions(NewArray);
        //m_Aimline.SetPositions(NewArray);
    }

    public virtual void OnEquipMedkit(InputAction.CallbackContext context)
    {

        OnCancelThrow();

        if (this != null)
        {
            inventory.SetCurrentItem(0);

            foreach (GameObject go in a_goItemIcons)
            {
                go.SetActive(false);
            }

            goItemTextBox.GetComponent<Text>().text = inventory.GetMedkitStock().ToString();

            a_goItemIcons[0].SetActive(true);

            if(inventory.GetMedkitStock() > 0)
            {
                foreach(GameObject go in a_goItemsInHand)
                {
                    go.SetActive(false);
                }
                a_goItemsInHand[1].SetActive(true);
            }
            else
            {
                foreach (GameObject go in a_goItemsInHand)
                {
                    go.SetActive(false);
                }
                a_goItemsInHand[0].SetActive(true);
            }

        }
    }

    public virtual void OnEquipSmokeBomb(InputAction.CallbackContext context)
    {

        OnCancelThrow();

        if (this != null)
        {
            inventory.SetCurrentItem(1);


            foreach (GameObject go in a_goItemIcons)
            {
                go.SetActive(false);
            }


            goItemTextBox.GetComponent<Text>().text = inventory.GetSmokeStock().ToString();

            a_goItemIcons[1].SetActive(true);

            if (inventory.GetSmokeStock() > 0)
            {
                foreach (GameObject go in a_goItemsInHand)
                {
                    go.SetActive(false);
                }
                a_goItemsInHand[2].SetActive(true);
            }
            else
            {
                foreach (GameObject go in a_goItemsInHand)
                {
                    go.SetActive(false);
                }
                a_goItemsInHand[0].SetActive(true);
            }
        }
    }

    public virtual void OnEquipStunBomb(InputAction.CallbackContext context)
    {
        OnCancelThrow();

        if (this != null)
        {
            inventory.SetCurrentItem(2);


            foreach (GameObject go in a_goItemIcons)
            {
                go.SetActive(false);
            }

            goItemTextBox.GetComponent<Text>().text = inventory.GetStunStock().ToString();

            a_goItemIcons[2].SetActive(true);

            if (inventory.GetStunStock() > 0)
            {
                foreach (GameObject go in a_goItemsInHand)
                {
                    go.SetActive(false);
                }
                a_goItemsInHand[3].SetActive(true);
            }
            else
            {
                foreach (GameObject go in a_goItemsInHand)
                {
                    go.SetActive(false);
                }
                a_goItemsInHand[0].SetActive(true);
            }
        }
    }

    public virtual void OnEquipDecoy(InputAction.CallbackContext context)
    {
        OnCancelThrow();

        if(this != null)
        {
            inventory.SetCurrentItem(3);

            foreach (GameObject go in a_goItemIcons)
            {
                go.SetActive(false);
            }

            goItemTextBox.GetComponent<Text>().text = inventory.GetDecoyStock().ToString();

            a_goItemIcons[3].SetActive(true);

            if (inventory.GetDecoyStock() > 0)
            {
                foreach (GameObject go in a_goItemsInHand)
                {
                    go.SetActive(false);
                }
                a_goItemsInHand[4].SetActive(true);
            }
            else
            {
                foreach (GameObject go in a_goItemsInHand)
                {
                    go.SetActive(false);
                }
                a_goItemsInHand[0].SetActive(true);
            }
        }
    }

    public virtual void OnBreakout(InputAction.CallbackContext context)
    {
        if(this != null)
        {
            if(fGrabBreakoutPresses > 0)
            {
                fGrabBreakoutPresses--;

                if(fGrabBreakoutPresses <= 0)
                {
                    goPressQReminder.SetActive(false);
                    bIsGrabbed = false;

                    this.transform.up = Vector3.up;

                    foreach(Collider col in Physics.OverlapSphere(transform.position, 10.0f, AILayer))
                    {
                        try
                        {
                            col.gameObject.GetComponent<AI_Guard_V2>().GlobalGrabCooldownReset();
                        }
                        catch
                        {
                            Debug.Log("No AI_Guard script found");
                        }
                    }
                }
            }
        }
    }

    public virtual void OnMousePos(InputAction.CallbackContext context)
    {

    }

    public virtual void OnPause(InputAction.CallbackContext context)
    {
        if (this != null)
        {

            if (!goPauseScreen.activeSelf)
            {

                Time.timeScale = 0.0f;

                goPauseScreen.SetActive(true);

                Cursor.lockState = CursorLockMode.None;

                Cursor.visible = true;
            }
        }
    }

    public virtual void OnCancelThrow(InputAction.CallbackContext context)
    {
        if (bGrenadeAiming)
        {
            //cancel throw
            m_LineRenderer.enabled = false;
            bGrenadeAiming = false;
        }
    }

    public virtual void OnCancelThrow()
    {
        if (bGrenadeAiming)
        {
            //cancel throw
            m_LineRenderer.enabled = false;
            bGrenadeAiming = false;
        }
    }

    public virtual void OnCameraStick(InputAction.CallbackContext context)
    {

    }

    public virtual void OnObjectiveShow(InputAction.CallbackContext context)
    {
        if (this != null)
        {
            if (context.started)
            {
                m_goGameManager.GetComponent<LevelDetails>().OpenObjectiveBox();
            }
        }
    }

    #endregion UNITY INPUT FUNCTIONS

    #region GET/SET FUNCTIONS

    public bool GetInSmoke()
    {
        return bIsInSmoke;
    }

    public bool GetIsCrouched()
    {
        return bIsCrouched;
    }
    
    public bool GetGrabbed()
    {
        return bIsGrabbed;
    }

    public void SetGrabbed(bool a_Grabbed)
    {
        bIsGrabbed = a_Grabbed;

        goPressQReminder.SetActive(true);

        if(bIsGrabbed)
        {
            fGrabBreakoutPresses = MAXBREAKOUTPRESSES;
        }
    }


    public bool GetIsSprinting()
    {
        return bSprinting;
    }

    public float GetStamina()
    {
        return fStamina;
    }

    public void SetStamina(float stamina)
    {
        fStamina = stamina;
    }

    public float GetMaxStamina()
    {
        return fMaxStamina;
    }

    public void SetMaxStamina(float max)
    {
        fMaxStamina = max;
    }

    public Vector2 GetHidingCameraMinClamp()
    {
        return v2HidingCameraMinClamp;
    }

    public void SetHidingCameraMinClamp(Vector2 clamp)
    {
        v2HidingCameraMinClamp = clamp;
    }

    public void SetHidingCameraMinClamp(float x, float y)
    {
        v2HidingCameraMinClamp = new Vector2(x, y);
    }

    public Vector2 GetHidingCameraMaxClamp()
    {
        return v2HidingCameraMaxClamp;
    }

    public void SetHidingCameraMaxClamp(Vector2 clamp)
    {
        v2HidingCameraMaxClamp = clamp;
    }

    public void SetHidingCameraMaxClamp(float x, float y)
    {
        v2HidingCameraMaxClamp = new Vector2(x, y);
    }

    public Vector2 GetHidingCameraLeanClamp()
    {
        return v2HidingCameraLeanClamp;
    }

    public void SetHidingCameraLeanClamp(Vector2 clamp)
    {
        v2HidingCameraLeanClamp = clamp;
    }

    public void SetHidingCameraLeanClamp(float x, float y)
    {
        v2HidingCameraLeanClamp = new Vector2(x, y);
    }

    public bool GetCanCrouchWhenHiding()
    {
        return bCanCrouchWhenHiding;
    }

    public void SetCanCrouchWhenHiding(bool canCrouch)
    {
        bCanCrouchWhenHiding = canCrouch;
    }

    public bool GetIsHiding()
    {
        return bHiding;
    }

    public void SetIsHiding(bool hiding)
    {
        bHiding = hiding;
    }

    public GameObject GetBoxView()
    {
        return goBoxView;
    }

    public void SetBoxView(GameObject boxView)
    {
        goBoxView = boxView;
    }

    public bool GetHasWon()
    {
        return bHasWon;
    }

    public void SetHasWon(bool won)
    {
        bHasWon = won;
    }

    public float GetInteractHoldTime()
    {
        return fInteractHoldTime;
    }

    public void SetInteractHoldTime(float hold)
    {
        fInteractHoldTime = hold;
    }

    public bool GetOnSpikes()
    {
        return bOnSpikes;
    }

    public void SetOnSpikes(bool spikes)
    {
        bOnSpikes = spikes;
    }

    public void SetInventory(Inventory a_Inventory)
    {
        inventory = a_Inventory;
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    public bool GetTut()
    {
        return m_bTut;
    }

    public void SetTut(bool aTut)
    {
        m_bTut = aTut;
    }

    #endregion GET/SET FUNCTIONS
}
