using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManNPCController : MonoBehaviour
{
    public Animator animator;
    public ParticleSystem snore;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) { WakeUp(); }
    }


    public void WakeUp()
    {
        snore.gameObject.SetActive( false);
        animator.SetBool("Awake", true);
    }
}
