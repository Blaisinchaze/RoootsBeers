using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadyNPCController : MonoBehaviour
{
    public bool panicked = true;
    public Animator animator;
    public Transform[] transforms;
    int currentTarget;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        currentTarget = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (!panicked) return;
        Vector3 relativePos = transforms[currentTarget].position - transform.position;

        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = rotation;

        transform.position = Vector3.MoveTowards(transform.position, transforms[currentTarget].position, Time.deltaTime * speed);

        if(Vector3.Distance(transform.position, transforms[currentTarget].position) < 1f)
        {
            currentTarget++;
            if(currentTarget > transforms.Length - 1 )
            {
                currentTarget = 0;
            }
        }
    }

    public void UnPanic()
    {
        if (!panicked) return;
        panicked = false;
        animator.SetBool("Happy", true);
    }
}
