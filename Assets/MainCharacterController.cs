using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainCharacterController : MonoBehaviour
{
    Vector3 moveDirection;
    Vector3 lookDirection;
    public Rigidbody rb;
    public float movementSpeed = 5f;
    public ParticleSystem fizzPS;
    public Transform groundCheckTransform;
    public LayerMask ignoredColliders;


    [SerializeField]bool isGrounded = true;
    bool requestedLaunch = false;
    bool isLaunching = false;

    //Fizz variables
    const float maxPossibleFizzValue = 1.0f;
    float currentMaxFizzValue = 0.5f;
    float currentFizzValue = 0f;
    [SerializeField] float fizzLaunchForce = 3f;

    // Start is called before the first frame update
    void Start()
    {
        currentFizzValue = currentMaxFizzValue;
    }

    public void Update()
    {
        //checking for Ground
        isGrounded = Physics.Raycast(groundCheckTransform.position, -Vector3.up, 0.5f, ignoredColliders);

        isLaunching = (requestedLaunch && currentFizzValue > 0);

        if (!isLaunching)
        {
            fizzPS.Stop();
            if (isGrounded)
            {
                currentFizzValue += Time.deltaTime;
                currentFizzValue = Mathf.Clamp(currentFizzValue, 0, currentMaxFizzValue);
            }
        }

        if (isLaunching && currentFizzValue > 0)
        {
            fizzPS.Play();
            currentFizzValue -= Time.deltaTime;
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
        if (isLaunching && currentFizzValue > 0)
        {
            rb.AddForce(rb.transform.up * fizzLaunchForce, ForceMode.Acceleration);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(groundCheckTransform.position, -Vector3.up * 0.5f);
    }
}
