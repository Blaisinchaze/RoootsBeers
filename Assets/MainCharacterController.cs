using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCharacterController : MonoBehaviour
{

    public enum PlayerStates
    {
        GROUNDED,
        AIRBORNE,
        AIMING
    }
    Vector2 rawMovementInput;
    Vector3 groundMoveDirection;
    Vector3 lookDirection;
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
    float currentMaxFizzValue = 0.5f;
    float currentFizzValue = 0f;
    float maxExcitement = 1.0f;
    float currentExcitement = 0.0f;
    [SerializeField] float fizzLaunchForce = 3f;
    float fizzFillPercent = 0.0f;

    //Aim variables
    [Header("AIM PROPERTIES")]
    bool requestedAim = false;
    bool isAiming = false;

    // Start is called before the first frame update
    void Start()
    {
        currentFizzValue = currentMaxFizzValue;

    }



    public void TransitionToState(PlayerStates newState)
    {
        PlayerStates tmpInitialState = currentPlayerState;
        OnStateExit(tmpInitialState, newState);
        currentPlayerState = newState;
        OnStateEnter(newState, tmpInitialState);

    }

    private void OnStateEnter(PlayerStates state, PlayerStates fromState)
    {
        switch (state)
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

                }
                break;
            default:
                break;
        }
    }

    private void OnStateExit(PlayerStates state, PlayerStates toState)
    {
        switch (state)
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
                    }
                    else
                    {
                        isLaunching = false;
                    }
                }
                break;
            case PlayerStates.AIMING:
                {


                    //Switch camera
                    transform.Rotate(new Vector3(rawMovementInput.x, rawMovementInput.y, 0f));


                    //Aim button released
                    if (!requestedAim)
                    {
                        if (currentExcitement < 0.9f)
                        {
                            TransitionToState(PlayerStates.GROUNDED);
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
                currentFizzValue = Mathf.Clamp(currentFizzValue, 0, currentMaxFizzValue);
            }
        }

        if (isLaunching && currentFizzValue > 0)
        {
            fizzPS.Play();
            if (!infiniteFizz)
            {
                currentFizzValue -= Time.deltaTime * 0.3f;
            }
        }

        //Excitement buildup when moving
        if (rb.velocity.magnitude > 0 && !isLaunching)
        {
            currentExcitement += Time.deltaTime;
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

    private void FixedUpdate()
    {
        //Player Movement when walking around
        if (currentPlayerState == PlayerStates.GROUNDED && isGrounded)
        {
            rb.MovePosition(rb.position + groundMoveDirection * groundMovementSpeed * Time.deltaTime);
        }

        //Player movement when airborne
        if (currentPlayerState == PlayerStates.AIRBORNE)
        {
            if (isLaunching)
            {

            }
            else
            {
                rb.MovePosition(rb.position + groundMoveDirection * airMovementSpeed *  Time.deltaTime);
            }
        }
        if (isLaunching)
        {
            if (fizzFillPercent > 0.99)
            {
                rb.AddForce(rb.transform.up * fizzLaunchForce, ForceMode.Impulse);
            }
            rb.AddForce(rb.transform.up * fizzLaunchForce * launchStrengthCurve.Evaluate(curveEvaluationValue), ForceMode.Acceleration);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(groundCheckTransform.position, -Vector3.up * 0.5f);
    }
}
