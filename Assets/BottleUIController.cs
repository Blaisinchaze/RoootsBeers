using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BottleUIController : MonoBehaviour
{
    PlayerManager player;
    public Image fillBottle;
    public Animator bottleAnimator;
    public TextMeshProUGUI bottlesCollected;
    bool fired = false;
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        fillBottle.fillAmount = player.playerController.FizzData.currentMaxFizzValue;
        bottlesCollected.text = ((int)(player.playerController.FizzData.currentMaxFizzValue * 100)).ToString();
        bottleAnimator.SetFloat("Speed", player.playerController.FizzData.currentExcitement * 5);

        if(player.playerController.isLaunching && fired == false)
        {
            fired = true;
            bottleAnimator.Play("Fire");
        }
        if(fired && !player.playerController.isLaunching)
        {
            bottleAnimator.Play("LidReturn");
            fired = false;
        }
    }
}
