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

    Vector3 resetPosition;
    Quaternion resetRotation;
    [SerializeField]bool enableFizzBurst = false;
    Vector2 rawMovementInput;
    Vector3 groundMoveDirection;
    Vector3 lookDirection;
    [SerializeField] Collider collider;
    public Rigidbody rb;
    public float groundMovementSpeed = 5f;
    PlayerStates currentPlayerState = PlayerStates.GROUNDED;
    public ParticleSystem fizzPS;
    public Transform groundCheckTransform;
    public LayerMask ignoredColliders;

    [SerializeField] private PlayerActionCollider ActionDataCollider;

    [SerializeField]bool isGrounded = true;
    public bool infiniteFizz = false;
    bool isLaunching = false;

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

    // Start is called before the first frame update
    void Start()
    {
        TransitionToState(PlayerStates.GROUNDED);
        currentFizzValue = currentMaxFizzValue;
        resetPosition = rb.transform.position;
        resetRotation = rb.transform.rotation;

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
                    rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                }
                break;
            case PlayerStates.AIRBORNE:
                {
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

                }
                break;
            case PlayerStates.AIRBORNE:
                {

                }
                break;
            case PlayerStates.AIMING:
                {
                    if (toState == PlayerStates.AIRBORNE)
                    {
                        rb.useGravity = false;
                        isLaunching = true;
                    }
                    collider.enabled = true;
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
        if (!isLaunching)
        {
            fizzPS.Stop();
            if (isGrounded)
            {
                currentFizzValue += Time.deltaTime * 10;

            }
        }

        if (isLaunching && currentFizzValue > 0)
        {
            fizzPS.Play();
            if (!infiniteFizz)
            {
                currentFizzValue -= Time.deltaTime * fizzBurnPerSecond;
            }
        }

        //Checking whether player is touching the ground
        isGrounded = Physics.Raycast(groundCheckTransform.position, -Vector3.up, 0.5f, ignoredColliders);
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




        FizzData = new FizzData(currentMaxFizzValue, currentExcitement, fizzLaunchForce);
        ActionDataCollider.actionData = new PlayerActionData(FizzData, currentPlayerState, isGrounded);
        #endregion

    }
    public void UpdateMovement(InputAction.CallbackContext context)
    {
        rawMovementInput = context.ReadValue<Vector2>();
        groundMoveDirection.x = rawMovementInput.x;
        groundMoveDirection.z = rawMovementInput.y;
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
        if (currentPlayerState == PlayerStates.GROUNDED && isGrounded)
        {
            rb.MovePosition(rb.position + groundMoveDirection * groundMovementSpeed * Time.fixedDeltaTime);
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
                rb.AddForce(rb.transform.up * fizzLaunchForce * fizzBurstMultiplier, ForceMode.VelocityChange);
                //rb.velocity = rb.transform.up * fizzLaunchForce * launchStrengthCurve.Evaluate(curveEvaluationValue);
                initialLaunchBurst = false;
            }
            rb.AddForce(rb.transform.up * fizzLaunchForce * launchStrengthCurve.Evaluate(curveEvaluationValue) * Time.fixedDeltaTime, ForceMode.Force);
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
        Debug.Log("Updated Fizz Data. New stats are :");
        Debug.Log("Fizz: " + currentMaxFizzValue.ToString());
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(groundCheckTransform.position, -Vector3.up * 0.5f);
    }

}