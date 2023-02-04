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


    [SerializeField]bool isGrounded = true;
    public bool infiniteFizz = false;
    bool isLaunching = false;

    //areal controls properties
    [Header("Aerial properties")]
    public float airMovementSpeed = 2f;

    //Fizz variables
    [Header("FIZZ PROPERTIES")]
    public AnimationCurve launchStrengthCurve;
    public float curveEvaluationValue = 0.0f;
    public float curveResultingValue = 0.0f;
    const float maxPossibleFizzValue = 1.0f;
    [SerializeField]float currentMaxFizzValue = 0.5f;
    [SerializeField]float currentFizzValue = 0f;
    [SerializeField] float fizzBurnPerSecond = 0.5f;
    float maxExcitement = 1.0f;
    [SerializeField]float currentExcitement = 0.0f;
    [SerializeField] float excitementBuildupRate = 5f;
    [SerializeField] float fizzLaunchForce = 3f;
    [SerializeField] float fizzBurstMultiplier = 3f;

    float fizzFillPercent = 0.0f;
    bool initialLaunchBurst = false;

    public FizzData FizzData;
    //Aim variables
    [Header("AIM PROPERTIES")]
    bool requestedAim = false;
    bool isAiming = false;
    float aimingExcitementBuildupRate = 10f;

    // Start is called before the first frame update
    void Start()
    {
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

                }
                break;
            case PlayerStates.AIRBORNE:
                {
                    initialLaunchBurst = true;
                }
                break;
            case PlayerStates.AIMING:
                {
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
                    rb.transform.Rotate(new Vector3(rawMovementInput.y, rawMovementInput.x, 0f));

                    currentExcitement += Time.deltaTime * aimingExcitementBuildupRate;


                    //Aim button released
                    if (!requestedAim)
                    {
                        if (currentExcitement < 0.9f)
                        {
                            TransitionToState(PlayerStates.GROUNDED);
                        }
                        else
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
                currentFizzValue += Time.deltaTime * 5;

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

        //Excitement buildup when moving
        if (rb.velocity.magnitude > 0.1f && !isLaunching)
        {
            currentExcitement += Time.deltaTime * excitementBuildupRate;
        }

        //Checking whether player is touchnig the ground
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
        currentExcitement = Mathf.Clamp(currentExcitement, 0.0f, 1.0f);
        currentFizzValue = Mathf.Clamp(currentFizzValue, 0, currentMaxFizzValue);
        currentMaxFizzValue = Mathf.Clamp(currentMaxFizzValue, 0, maxPossibleFizzValue);


        #endregion
        FizzData = new FizzData(currentMaxFizzValue, currentFizzValue, maxExcitement,
            currentExcitement, fizzLaunchForce, fizzFillPercent);
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(groundCheckTransform.position, -Vector3.up * 0.5f);
    }

}