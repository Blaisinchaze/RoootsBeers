using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFollow : MonoBehaviour
{

    public Transform Player;
    public float rotationSpeed = 10f;
    Vector3 inputDelta;

    void Update()
    {
        transform.position = Player.position;

        Vector3 rotationValue = (inputDelta * Time.deltaTime * rotationSpeed);
        Debug.Log("the rotation value: " + rotationValue);
        transform.Rotate(rotationValue);
    }

    public void UpdateRotation(InputAction.CallbackContext context)
    {
        Vector2 data = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);
        inputDelta = new Vector3(data.y, data.x, 0f);
    }
}
