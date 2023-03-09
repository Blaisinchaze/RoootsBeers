using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{

    PlayerControls playerControls;
    public MainCharacterController character;
    PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();
    private Camera mainCam;


    //local variables
    bool _jumpDown = false;

    private void OnEnable()
    {
        playerControls = new PlayerControls();
        mainCam = Camera.main;
        playerControls.Player.Enable();
        playerControls.Player.Fire.performed += Fire_performed;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Fire_performed(InputAction.CallbackContext context)
    {
        _jumpDown = context.performed;
        Debug.Log("JUMP");
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleCharacterInput();
    }

    private void HandleCharacterInput()
    {
        playerControls.Player.Fire.performed += _ => {characterInputs.aimDown = true; characterInputs.aimUp = false;};
        playerControls.Player.Fire.canceled += _ =>{ characterInputs.aimDown = false; characterInputs.aimUp = true;};
        characterInputs.movementVector = playerControls.Player.Move.ReadValue<Vector2>();
        characterInputs.CameraRotation = mainCam.transform.rotation;
        character.SetInputs(ref characterInputs);

        //Reset local values
        _jumpDown = false;
    }
}
