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

        //Vector3 rotationValue = (inputDelta * Time.deltaTime * rotationSpeed);
        //rotationValue.x = Mathf.Clamp(rotationValue.x, 0, 45f);
        //Debug.Log("the rotation value: " + rotationValue);
        //transform.Rotate(rotationValue);

        transform.rotation *= Quaternion.AngleAxis(inputDelta.x * rotationSpeed, Vector3.up);
    }

    public void UpdateRotation(InputAction.CallbackContext context)
    {
        Vector2 data = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);
        inputDelta = new Vector3(data.x, data.y, 0f);
    }
}
