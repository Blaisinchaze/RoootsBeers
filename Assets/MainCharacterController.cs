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


    bool isGrounded;
    bool requestedLaunch;



    // Start is called before the first frame update
    void Start()
    {
    }

    public void Update()
    {
        //checking for Ground
        // if (Physics.Raycast(transform.position,-Vector3.up,0.5f))

        if (requestedLaunch && isGrounded)
        {
            fizzPS.Play();
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
       rb.MovePosition(rb.position + moveDirection * movementSpeed);
    }
}
