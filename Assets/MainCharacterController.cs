using KinematicCharacterController;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStates
{
    GROUNDED,
    AIRBORNE,
    AIMING
}


public struct PlayerCharacterInputs
{
    public Vector2 movementVector;
    public Quaternion CameraRotation;
    public bool aimDown;
    public bool aimUp;
}

public class MainCharacterController : MonoBehaviour , ICharacterController
{
    //Debug stuff & resetting player
    [SerializeField]bool enableFizzBurst = false;

    //Required elements
    public KinematicCharacterMotor Motor;
    [SerializeField] Collider playerCollider;
    public float groundMovementSpeed = 5f;
    public GameObject fizzPSParent;
    public Transform groundCheckTransform;
    public LayerMask ignoredColliders;
    public Animator playerAnimator;
    public Transform cameraFollowPoint;
    public Transform topLaunchPoint;


    //Input Controls
    Vector2 rawMovementInput;
    Vector3 groundMoveDirection;
    Vector3 lookDirection;
    Vector2 rawMouseDeltaInput;
    Camera mainCam;

    //new input controls
    private Vector3 _moveInputVector;
    private Vector3 _lookInputVector;
    public float OrientationSharpness = 10f;
    public float MaxStableMoveSpeed = 10f;
    public Vector3 Gravity = new Vector3(0f, -30f, 0f);
    public float AirAccelerationSpeed = 5f;
    public float Drag = 0.1f;
    private Vector3 _internalVelocityAdd = Vector3.zero;
    public List<Collider> IgnoredColliders = new List<Collider>();

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
    const float maxPossibleFizzValue = 1.0f;
    [Range(0f,1f)]
    [SerializeField]float currentMaxFizzValue = 0.5f;
    [SerializeField]float currentFizzValue = 0f;
    float fizzFillPercent = 0.0f;
    bool initialLaunchBurst = false;
    private Wobble wobbleRef;
    Vector3 launchDir = Vector3.zero;
    public FizzData FizzData;

    //Launch fizz properties
    [Space]
    [Header("Launch Fizz properties")]
    [SerializeField] float burstLaunchForce = 3f;
    [SerializeField] float fizzBurstMultiplier = 20f;
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
    bool _aimRequested = false;
    bool _aimReleased = false;

    [Space]
    [Header("AUDIO PLAYERS")]
    public AudioSource FizzLoopSource;
    public AudioSource BottleOpeningSource;
    public AudioSource SloshingSource;
    public AudioSource LeadGuitarSource;
    public AudioSource HighGuitarSource;

    //timers
    float sloshTimer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        Motor.CharacterController = this;

        TransitionToState(PlayerStates.GROUNDED);

        currentFizzValue = currentMaxFizzValue;
        //resetPosition = rb.transform.position;
        //resetRotation = rb.transform.rotation;
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
                    isLaunching = false;
                }
                break;
            case PlayerStates.AIRBORNE:
                {
                    Motor.ForceUnground(0.1f);
                    float bottleOpendelay = 0f;

                    BottleOpeningSource.clip = SFX_Soundbank.instance.BottleOpening[UnityEngine.Random.Range(0, SFX_Soundbank.instance.BottleOpening.Count)];
                    HighGuitarSource.volume = 1f;
                    if (fromState == PlayerStates.GROUNDED)
                    {
                        isLaunching = true;
                        launchDir = Motor.CharacterUp;

                    }
                    BottleOpeningSource.Play();
                    FizzLoopSource.PlayDelayed(bottleOpendelay);
                    fizzPSParent.SetActive(true);

                    //rb.constraints = RigidbodyConstraints.None;
                    currentExcitement = 0f;
                    initialLaunchBurst = true;

                    Debug.Log(launchDir);
                }
                break;
            case PlayerStates.AIMING:
                {
                    _aimRequested = false;
                    playerCollider.enabled = false;
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
                    HighGuitarSource.volume = 0f;
                    fizzPSParent.SetActive(false);
                    //rb.transform.rotation = resetRotation;
                }
                break;
            case PlayerStates.AIMING:
                {

                    if (toState == PlayerStates.AIRBORNE)
                    {
                        launchDir = topLaunchPoint.up.normalized;
                        isLaunching = true;
                        playerAnimator.Play("InAir");
                    }
                    playerCollider.enabled = true;
                    playerAnimator.SetBool("Aiming", false);
                }
                break;
            default:
                break;
        }
    }
    /*
    public void Update()
    {

        #region STATE DEPENDANT LOGIC
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

        //Clamping meter values
        currentExcitement = Mathf.Clamp(currentExcitement, 0.0f, maxExcitement);
        currentFizzValue = Mathf.Clamp(currentFizzValue, 0.0f, currentMaxFizzValue);
        currentMaxFizzValue = Mathf.Clamp(currentMaxFizzValue, 0, maxPossibleFizzValue);


        LeadGuitarSource.volume = currentExcitement;
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
    */

    /*
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
    */

    /*
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
    */
    public void ReceiveFizzData(FizzData incData)
    {

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

    public void SetInputs( ref PlayerCharacterInputs inputs)
    {
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.movementVector.x, 0f, inputs.movementVector.y), 1f);

        //Calculate camera direction and rotation on the character plane
        Vector3 cameraDirection = inputs.CameraRotation * Vector3.forward;
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
        if (cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
        }
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection,Motor.CharacterUp);
        switch (currentPlayerState)
        {
            case PlayerStates.GROUNDED:
                _moveInputVector = cameraPlanarRotation * moveInputVector;
                _lookInputVector = cameraPlanarRotation * moveInputVector;

                if (inputs.aimDown)
                {
                    _aimRequested = true;
                }
                break;
            case PlayerStates.AIRBORNE:
                _lookInputVector = cameraPlanarRotation * moveInputVector;

                break;
            case PlayerStates.AIMING:
                if (inputs.aimUp)
                {
                    _aimReleased = true;
                }
                _lookInputVector = new Vector3(cameraDirection.x,cameraPlanarDirection.y,cameraDirection.z);
                break;
            default:
                break;
        }

    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        switch (currentPlayerState)
        {
            case PlayerStates.GROUNDED:
                {
                    //tranisition to aim state if aim is pressed
                    if ( _aimRequested && isGrounded)
                    {
                        TransitionToState(PlayerStates.AIMING);
                    }

                    //excitement buildup before point of no return
                    if (currentExcitement < excitementPointOfNoReturn)
                    {

                        if (Motor.Velocity.sqrMagnitude > 0)
                        {
                            currentExcitement += excitementBuildupRate * Time.deltaTime;

                        }
                    }
                    //excitement buildup after point of no return
                    else
                    {
                        currentExcitement += excitementBuildupRate * Time.deltaTime;

                        if (Motor.Velocity.sqrMagnitude > 0)
                        {
                            currentExcitement += excitementBuildupRate * 2 * Time.deltaTime;
                        }
                    }
                    //Walking sounds based on movement
                    if (Motor.Velocity.sqrMagnitude > 0)
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

                    if (Motor.GroundingStatus.IsStableOnGround && currentFizzValue <= 0)
                    {
                        TransitionToState(PlayerStates.GROUNDED);
                    }
                }
                break;
            case PlayerStates.AIMING:
                {
                    //Fizz buildup while aiming
                    if (currentExcitement < excitementPointOfNoReturn)
                    {
                        currentExcitement += Time.deltaTime * excitementBuildupRate;

                    }
                    else
                    {
                        currentExcitement += excitementBuildupRate / 4 * Time.deltaTime;
                    }

                    //Logic of releasing aim while in aim state
                    if (_aimReleased)
                    {
                        if (currentExcitement < excitementPointOfNoReturn)
                        {
                            TransitionToState(PlayerStates.GROUNDED);
                        }
                        else
                        {
                            TransitionToState(PlayerStates.AIRBORNE);
                        }
                        _aimReleased = false;
                    }

                }
                break;
            default:
                { }
                break;

        }

        //STATE INDEPENDANT CALCULATIONS

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
        isGrounded = Motor.GroundingStatus.IsStableOnGround;
        playerAnimator.SetBool("IsGrounded", Motor.GroundingStatus.IsStableOnGround);

        //Calculate percentage of fullness of the current fizz tank
        fizzFillPercent = currentFizzValue / currentMaxFizzValue;
        curveEvaluationValue = 1 - fizzFillPercent;

        //Clamping meter values
        currentExcitement = Mathf.Clamp(currentExcitement, 0.0f, maxExcitement);
        currentFizzValue = Mathf.Clamp(currentFizzValue, 0.0f, currentMaxFizzValue);
        currentMaxFizzValue = Mathf.Clamp(currentMaxFizzValue, 0, maxPossibleFizzValue);


        LeadGuitarSource.volume = currentExcitement;
        wobbleRef.updateBottleFill(currentMaxFizzValue);
        playerAnimator.SetFloat("LiquidAmount", currentMaxFizzValue);
        FizzData = new FizzData(currentMaxFizzValue, currentExcitement, burstLaunchForce);
        ActionDataCollider.actionData = new PlayerActionData(FizzData, currentPlayerState, isGrounded);

        //updating timers
        if (groundMoveDirection.magnitude > 0)
        {
            sloshTimer += Time.deltaTime;
        }
    }
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        switch (currentPlayerState)
        {
            case PlayerStates.GROUNDED:
                {
                    if (_lookInputVector != Vector3.zero && OrientationSharpness > 0f)
                    {
                        // Smoothly interpolate from current to target look direction
                        Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                        // Set the current rotation (which will be used by the KinematicCharacterMotor)
                        currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                    }
                }
                break;
            case PlayerStates.AIRBORNE:
                {

                }
                break;
            case PlayerStates.AIMING:
                {
                    if (_lookInputVector != Vector3.zero && OrientationSharpness > 0f)
                    {
                        // Smoothly interpolate from current to target look direction
                        Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterUp, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * 100f * deltaTime)).normalized;

                        // Set the current rotation (which will be used by the KinematicCharacterMotor)
                        currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                    }

                }
                break;
            default:
                { }
                break;
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        switch (currentPlayerState)
        {
            case PlayerStates.GROUNDED:
                {
                    Vector3 targetMovementVelocity = Vector3.zero;
                    if (Motor.GroundingStatus.IsStableOnGround)
                    {
                        //Reorient velocity on slope
                        currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                        //Calculate target velocity
                        Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                        Vector3 reorientedInput = Vector3.Cross(Motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                        targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

                        // Smooth movement velocity
                        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-OrientationSharpness * deltaTime));
                    }
                    else
                    {
                        if (_moveInputVector.sqrMagnitude > 0f)
                        {
                            targetMovementVelocity = _moveInputVector * airMovementSpeed;

                            if (Motor.GroundingStatus.FoundAnyGround)
                            {
                                //Do this later
                            }

                            Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                            currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
                        }

                        // Gravity
                        currentVelocity += Gravity * deltaTime;

                        //Drag
                        //currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                    }

                    //Handle Jumping
                    if (_internalVelocityAdd.sqrMagnitude > 0f)
                    {
                        currentVelocity += _internalVelocityAdd;
                        _internalVelocityAdd = Vector3.zero;
                    }
                }
                break;
            case PlayerStates.AIRBORNE:
                {
                    Vector3 targetMovementVelocity = Vector3.zero;
                    if (currentFizzValue > 0)
                    {
                        //DO THE MATH FOR LAUNCH ARC STUPID
                        //targetMovementVelocity = launchDir.normalized * launchStrengthCurve.Evaluate(curveEvaluationValue) * 20f * deltaTime  + ;
                        //targetMovementVelocity += Gravity * deltaTime;
                        //currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-OrientationSharpness * deltaTime));
                        if (initialLaunchBurst)
                        {
                            currentVelocity += launchDir.normalized * fizzBurstMultiplier * burstLaunchForce;
                            Debug.Log("BURST");
                            initialLaunchBurst = false;
                        }
                        currentVelocity += launchDir.normalized * launchStrengthCurve.Evaluate(curveEvaluationValue) * fizzBurstMultiplier * deltaTime;
                    }
                    else
                    {
                        currentVelocity += Gravity * deltaTime;
                    }
                }
                break;
            case PlayerStates.AIMING:
                {
                    currentVelocity = Vector3.zero;
                }
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is called after the character has finished its movement update
    /// </summary>
    public void AfterCharacterUpdate(float deltaTime)
    {
        switch (currentPlayerState)
        {
            case PlayerStates.GROUNDED:
                {
                    playerAnimator.SetBool("IsGrounded", Motor.GroundingStatus.IsStableOnGround);
                    playerAnimator.SetBool("Moving", Motor.Velocity.sqrMagnitude > 0.1f);
                }
                break;
            case PlayerStates.AIRBORNE:
                {

                }
                break;
            case PlayerStates.AIMING:
                {

                }
                break;
            default:
                break;
        }
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        if (IgnoredColliders.Contains(coll))
        {
            return false;
        }
        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {

    }
    public void PostGroundingUpdate(float deltaTime)
    {

    }

    public void AddVelocity(Vector3 velocity)
    {
        switch (currentPlayerState)
        {
            case PlayerStates.GROUNDED:
                {
                   _internalVelocityAdd += velocity;
                }
                break;
            case PlayerStates.AIRBORNE:
                _internalVelocityAdd += velocity;
                break;
            case PlayerStates.AIMING:
                break;
            default:
                break;
        }
    }
    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {

    }
}