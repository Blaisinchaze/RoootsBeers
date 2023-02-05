using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinConditionScript : MonoBehaviour
{
    float endgameTimer = 0f;
    bool endingTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            endingTriggered = true;
            //TRIGGER CUTSCENE
        }
    }

    private void Update()
    {
        if (endingTriggered)
        {
            endgameTimer += Time.deltaTime;
        }

        if (endgameTimer >= 3f)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
