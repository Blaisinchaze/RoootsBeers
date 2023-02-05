using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GentleSpin : MonoBehaviour
{
    public float spinRate = 0.5f;

    private void Update()
    {
        transform.Rotate(Vector3.up * spinRate * Time.deltaTime);
    }
}
