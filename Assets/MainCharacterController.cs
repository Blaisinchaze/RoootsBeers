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

    Vector3 moveDirection;
    Vector3 lookDirection;
    public Rigidbody rb;
    public float movementSpeed = 5f;
    public ParticleSystem fizzPS;
    public Transform groundCheckTransform;
    public LayerMask ignoredColliders;


    [SerializeField]bool isGrounded = true;
    public bool infiniteFizz = false;
    bool requestedLaunch = false;
    bool isLaunching = false;

    //Fizz variables
    [Header("FIZZ PROPERTIES")]
    public AnimationCurve launchStrengthCurve;
    public float curveEvaluationValue = 0.0f;
    public float curveResultingValue = 0.0f;
    const float maxPossibleFizzValue = 1.0f;
    float currentMaxFizzValue = 0.5f;
    float currentFizzValue = 0f;
    [SerializeField] float fizzLaunchForce = 3f;
    float fizzFillPercent = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        currentFizzValue = currentMaxFizzValue;
    }

    public void Update()
    {
        //checking for Ground
        isGrounded = Physics.Raycast(groundCheckTransform.position, -Vector3.up, 0.5f, ignoredColliders);

        //Calculate percentage of fullness of the current fizz tank
        fizzFillPercent = currentFizzValue / currentMaxFizzValue;
        curveEvaluationValue = 1 - fizzFillPercent;
        curveResultingValue = launchStrengthCurve.Evaluate(curveEvaluationValue);


        isLaunching = (requestedLaunch && currentFizzValue > 0);

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


    }
        //Read movement input
    public void UpdateMovement(InputAction.CallbackContext context)
    {
        Vector2 vector2 = context.ReadValue<Vector2>();
        moveDirection.x = vector2.x;
        moveDirection.z = vector2.y;
    }
    public void UpdateLaunch(InputAction.CallbackContext context)
    {
         requestedLaunch = context.performed;
    }

    private void FixedUpdate()
    {
       rb.MovePosition(rb.position + moveDirection * movementSpeed * Time.deltaTime);
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
