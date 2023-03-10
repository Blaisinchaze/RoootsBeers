using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerStates
{
    GROUNDED,
    AIRBORNE,
    AIMING
}

public class MainCharacterController : MonoBehaviour
{
    //Debug stuff & resetting player
    Vector3 resetPosition;
    Quaternion resetRotation;
    [SerializeField]bool enableFizzBurst = false;

    //Required elements
    [SerializeField] Collider collider;
    public Rigidbody rb;
    public float groundMovementSpeed = 5f;
    public GameObject fizzPSParent;
    public Transform groundCheckTransform;
    public LayerMask ignoredColliders;
    public Animator playerAnimator;
    public Transform cameraFollowPoint;
    public Transform topLaunchPoint;
    public Transform bottomLaunchPoint;


    //Input Controls
    Vector2 rawMovementInput;
    Vector3 groundMoveDirection;
    Vector3 lookDirection;
    Vector2 rawMouseDeltaInput;
    Camera mainCam;

    public PlayerStates currentPlayerState = PlayerStates.GROUNDED;

    [SerializeField] private PlayerActionCollider ActionDataCollider;

    bool isGrounded = true;
    [SerializeField] bool infiniteFizz = false;
    public bool isLaunching = false;

    //areal controls properties
    [Header("Aerial properties")]
    public float airMovementSpeed = 2f;

    //Fizz variables
    [Space]
    [Header("FIZZ PROPERTIES")]
    public AnimationCurve launchStrengthCurve;
    public float curveEvaluationValue = 0.0f;
    public float curveResultingValue = 0.0f;
    const float maxPossibleFizzValue = 1.0f;
    [SerializeField]float currentMaxFizzValue = 0.5f;
    [SerializeField]float currentFizzValue = 0f;
    float fizzFillPercent = 0.0f;
    bool initialLaunchBurst = false;
    private Wobble wobbleRef;
    Vector3 launchDir = Vector3.zero;
    public FizzData FizzData;

    //Launch fizz propertie
    [Space]
    [Header("Launch Fizz properties")]
    [SerializeField] float fizzLaunchForce = 3f;
    [SerializeField] float fizzBurstMultiplier = 3f;
    [SerializeField] float fizzBurnPerSecond = 0.5f;

    //Liquid excitement properties
    [Space]
    [Header("Liquid excitement properties")]
    const float excitementPointOfNoReturn = 0.9f;
    const float maxExcitement = 1.0f;
    [SerializeField]float currentExcitement = 0.0f;
    [SerializeField] float excitementBuildupRate = 0.05f;


    //Aim variables
    [Space]
    [Header("AIM PROPERTIES")]
    bool requestedAim = false;
    bool isAiming = false;
    float aimingExcitementBuildupRate = 0.5f;

    [Space]
    [Header("AUDIO PLAYERS")]
    public AudioSource FizzLoopSource;
    public AudioSource BottleOpeningSource;
    public AudioSource SloshingSource;
    public AudioSource LeadGuitarSource;

    //timers
    float sloshTimer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        TransitionToState(PlayerStates.GROUNDED);
        currentFizzValue = currentMaxFizzValue;
        resetPosition = rb.transform.position;
        resetRotation = rb.transform.rotation;
        wobbleRef = GetComponentInChildren<Wobble>();
        mainCam = Camera.main;

    }



    public void TransitionToState(PlayerStates newState)
    {
        PlayerStates tmpInitialState = currentPlayerState;
        OnStateExit(tmpInitialState, newState);
        currentPlayerState = newState;
        OnStateEnter(newState, tmpInitialState);

    }

    private void OnStateEnter(PlayerStates newState, PlayerStates fromState)
    {
        switch (newState)
        {
            case PlayerStates.GROUNDED:
                {
                    rb.constraints = RigidbodyConstraints.None;
                    rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
                    rb.useGravity = true;
                }
                break;
            case PlayerStates.AIRBORNE:
                {
                    float bottleOpendelay = 0f;

                    BottleOpeningSource.clip = SFX_Soundbank.instance.BottleOpening[UnityEngine.Random.RandomRange(0, SFX_Soundbank.instance.BottleOpening.Count)];
                    LeadGuitarSource.volume = 1f;
                    if (fromState == PlayerStates.GROUNDED)
                    {
                        isLaunching = true;
                        launchDir = Vector3.up;

                    }
                    BottleOpeningSource.Play();
                    FizzLoopSource.PlayDelayed(bottleOpendelay);
                    fizzPSParent.SetActive(true);

                    rb.constraints = RigidbodyConstraints.None;
                    currentExcitement = 0f;
                    initialLaunchBurst = true;
                }
                break;
            case PlayerStates.AIMING:
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                    rb.useGravity = false;
                    collider.enabled = false;
                    playerAnimator.SetBool("Aiming", true);
                }
                break;
            default:
                break;
        }
    }

    private void OnStateExit(PlayerStates previousState, PlayerStates toState)
    {
        switch (previousState)
        {
            case PlayerStates.GROUNDED:
                {
                    if (toState == PlayerStates.AIRBORNE)
                    {
                        playerAnimator.Play("Launch");
                    }
                }
                break;
            case PlayerStates.AIRBORNE:
                {
                    FizzLoopSource.Stop();
                    LeadGuitarSource.volume = 0f;
                    fizzPSParent.SetActive(false);
                    rb.transform.rotation = resetRotation;
                }
                break;
            case PlayerStates.AIMING:
                {

                    if (toState == PlayerStates.AIRBORNE)
                    {
                        rb.useGravity = false;
                        launchDir = topLaunchPoint.up.normalized;
                        isLaunching = true;
                        playerAnimator.Play("InAir");
                    }
                    collider.enabled = true;
                    playerAnimator.SetBool("Aiming", false);
                }
                break;
            default:
                break;
        }
    }

    public void Update()
    {

        #region STATE DEPENDANT LOGIC
        //State changes
        switch (currentPlayerState)
        {
            case PlayerStates.GROUNDED:
                {

                    //Update playerbody rotation
                    //Vector3 smoothedLookInputDirection = Vector3.Slerp(rb.transform.rotation, lookDirection, 1 - Mathf.Exp(-10 * Time.deltaTime)).normalized;
                    playerAnimator.SetBool("Moving", groundMoveDirection.sqrMagnitude > 0);
                    if (requestedAim && isGrounded)
                    {
                        TransitionToState(PlayerStates.AIMING);
                    }

                    //excitement buildup before point of no return
                    if (currentExcitement < excitementPointOfNoReturn)
                    {

                        if (groundMoveDirection.sqrMagnitude > 0)
                        {
                            currentExcitement += excitementBuildupRate * Time.deltaTime;

                        }
                    }
                    //excitement buildup after point of no return
                    else
                    {
                        currentExcitement += excitementBuildupRate * Time.deltaTime;

                        if (groundMoveDirection.sqrMagnitude > 0)
                        {
                            currentExcitement += excitementBuildupRate * 2 * Time.deltaTime;
                        }
                    }

                    if (groundMoveDirection.sqrMagnitude > 0)
                    {
                        if (sloshTimer >= 0.6f)
                        {
                            SloshingSource.clip = SFX_Soundbank.instance.SloshingLiquid[UnityEngine.Random.Range(0, SFX_Soundbank.instance.SloshingLiquid.Count)];
                            SloshingSource.Play();
                            sloshTimer = 0f;
                        }

                    }
                    //Spontaneous burst after max excitement reached
                    if (currentExcitement >= maxExcitement)
                    {

                        TransitionToState(PlayerStates.AIRBORNE);
                    }

                }
                break;
            case PlayerStates.AIRBORNE:
                {
                    if (currentFizzValue > 0)
                    {
                        isLaunching = true;
                        rb.useGravity = false;
                    }
                    else
                    {
                        isLaunching = false;
                        rb.useGravity = true;
                    }
                }
                break;
            case PlayerStates.AIMING:
                {

                    //Switch camera


                    //rotate player to aim the launch
                    rb.transform.Rotate(new Vector3(rawMovementInput.y, rawMovementInput.x, 0f));


                    //logic behind buildup and button release based on current level of excitement
                    if (currentExcitement < excitementPointOfNoReturn)
                    {
                        currentExcitement += Time.deltaTime * excitementBuildupRate;

                        if (!requestedAim)
                        {
                            TransitionToState(PlayerStates.GROUNDED);
                        }
                    }
                    else
                    {
                        currentExcitement += excitementBuildupRate / 4 * Time.deltaTime;

                        if (!requestedAim)
                        {
                            TransitionToState(PlayerStates.AIRBORNE);
                        }
                    }
                }
                break;
            default:
                {

                }
                break;
        }

        #endregion

        #region STATE INDEPENDANT LOGIC

        //Handling fizz particle and fizz recharge based on current state
        if (!isLaunching && isGrounded)
        {
                currentFizzValue += Time.deltaTime * 10;
        }

        if (isLaunching && currentFizzValue > 0 && !infiniteFizz)
        {
                currentFizzValue -= Time.deltaTime * fizzBurnPerSecond;
        }

        //Checking whether player is touching the ground
        isGrounded = Physics.BoxCast(groundCheckTransform.position,new Vector3(0.3f,0.3f),Vector3.down, Quaternion.identity, 0.5f,ignoredColliders);
        playerAnimator.SetBool("IsGrounded", isGrounded);
        if (currentPlayerState == PlayerStates.AIRBORNE && isGrounded && !isLaunching)
        {
            TransitionToState(PlayerStates.GROUNDED);
        }

        //Calculate percentage of fullness of the current fizz tank
        fizzFillPercent = currentFizzValue / currentMaxFizzValue;
        curveEvaluationValue = 1 - fizzFillPercent;
        curveResultingValue = launchStrengthCurve.Evaluate(curveEvaluationValue);

        //Clamping meter values
        currentExcitement = Mathf.Clamp(currentExcitement, 0.0f, maxExcitement);
        currentFizzValue = Mathf.Clamp(currentFizzValue, 0.0f, currentMaxFizzValue);
        currentMaxFizzValue = Mathf.Clamp(currentMaxFizzValue, 0, maxPossibleFizzValue);
        wobbleRef.updateBottleFill(currentMaxFizzValue);



        playerAnimator.SetFloat("LiquidAmount", currentMaxFizzValue);
        FizzData = new FizzData(currentMaxFizzValue, currentExcitement, fizzLaunchForce);
        ActionDataCollider.actionData = new PlayerActionData(FizzData, currentPlayerState, isGrounded);

        //updating timers
        if (groundMoveDirection.magnitude > 0)
        {
            sloshTimer += Time.deltaTime;
        }

        #endregion

    }
    public void UpdateMovement(InputAction.CallbackContext context)
    {
        rawMovementInput = context.ReadValue<Vector2>();
        groundMoveDirection.x = rawMovementInput.x;
        groundMoveDirection.z = rawMovementInput.y;
        groundMoveDirection = Vector3.ClampMagnitude(new Vector3(rawMovementInput.y, 0f, -rawMovementInput.x),1f);
    }

    public void UpdateLookInput(InputAction.CallbackContext context)
    {
        rawMouseDeltaInput = context.ReadValue<Vector2>();
        Quaternion cameraRot = mainCam.transform.rotation;
        lookDirection = Vector3.ProjectOnPlane(cameraRot * Vector3.forward, Vector3.up).normalized;
        //lookDirection = new Vector3(rawMouseDeltaInput.x, rawMouseDeltaInput.y, 0f);
    }

    public void UpdateAim(InputAction.CallbackContext context)
    {
        requestedAim = context.performed;
    }

    public void ResetPlayer(InputAction.CallbackContext context)
    {
        rb.transform.position = resetPosition;
        rb.transform.rotation = resetRotation;
        rb.Sleep();
    }

    public void DebugFizzAdjustment(InputAction.CallbackContext context)
    {
        currentMaxFizzValue +=  0.05f *context.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        //Player Movement when walking around
        if (currentPlayerState == PlayerStates.GROUNDED)
        {
            Vector3 inputRight = Vector3.Cross(lookDirection, Vector3.up).normalized;
            Vector3 reorientedInput = Vector3.Cross(Vector3.up, inputRight).normalized * groundMoveDirection.magnitude;
            Vector3 forwardRelativeVerticalinput = lookDirection * groundMoveDirection.x;
            Vector3 rightRelativeHorizontalInput = inputRight * groundMoveDirection.z;
            Vector3 CameraRelativeMovement = forwardRelativeVerticalinput + rightRelativeHorizontalInput;
            //Vector3 flatCamRot = mainCam.transform.forward;
            //flatCamRot.y = 0;
            if (groundMoveDirection.magnitude > 0)
            {
                rb.transform.rotation = Quaternion.LookRotation(CameraRelativeMovement, Vector3.up);
            }
            rb.MovePosition(rb.position + CameraRelativeMovement * groundMovementSpeed * Time.fixedDeltaTime);
        }

        //Player movement when airborne
        if (currentPlayerState == PlayerStates.AIRBORNE)
        {
            if (isLaunching)
            {
            }
            else
            {
                rb.MovePosition(rb.position + groundMoveDirection * airMovementSpeed *  Time.fixedDeltaTime);
            }
        }
        if (isLaunching)
        {
            if (initialLaunchBurst && enableFizzBurst)
            {
                //rb.AddForce(rb.transform.up * fizzLaunchForce * fizzBurstMultiplier * currentMaxFizzValue, ForceMode.Impulse);

                rb.AddForce(launchDir.normalized * fizzLaunchForce * fizzBurstMultiplier, ForceMode.VelocityChange);
                //rb.velocity = rb.transform.up * fizzLaunchForce * launchStrengthCurve.Evaluate(curveEvaluationValue);
                initialLaunchBurst = false;
            }
            rb.AddForce(launchDir.normalized * fizzLaunchForce * launchStrengthCurve.Evaluate(curveEvaluationValue) * Time.fixedDeltaTime, ForceMode.Force);
        }
    }
    public void ReceiveFizzData(FizzData incData)
    {
        Debug.Log("Received fizz data. Current stats are :");
        Debug.Log("Fizz: " + currentMaxFizzValue.ToString());
        switch (incData.behaviour)
        {
            case FizzDataBehaviour.Readonly:
                break;
            case FizzDataBehaviour.Increment:
                currentMaxFizzValue += incData.currentMaxFizzValue;
                break;
            default:
                break;

        }
        AudioClip clip = SFX_Soundbank.instance.CollectPickup[UnityEngine.Random.Range(0, SFX_Soundbank.instance.CollectPickup.Count)];
        BottleOpeningSource.PlayOneShot(clip);
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(groundCheckTransform.position, -Vector3.up * 0.5f);
    }

}