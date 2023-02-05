using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogNPCController : MonoBehaviour
{
    public Transform[] transforms;
    int currentTarget;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relativePos = transforms[currentTarget].position - transform.position;

        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = rotation;

        transform.position = Vector3.MoveTowards(transform.position, transforms[currentTarget].position, Time.deltaTime * speed);

        if (Vector3.Distance(transform.position, transforms[currentTarget].position) < 1f)
        {
            currentTarget++;
            if (currentTarget > transforms.Length - 1)
            {
                currentTarget = 0;
            }
        }
    }
}
