using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : Singleton<SceneHandler>
{
    [Header("Transition Settings")]
    public Image transitionScreen;
    public float transitionSpeed;
    private float transitionPercent = 1;
    private int transitionDir = 0;
    private bool inTransition = false;

    private string targetScene;

    private void Start()
    {
        EventManager.Instance.EnterScene.AddListener(OnEnterScene);
        EventManager.Instance.LeaveScene.AddListener(OnExitScene);
        OnEnterScene();
    }

    private void Update()
    {
        transitionScreen.color = new Color(transitionScreen.color.r, transitionScreen.color.g, transitionScreen.color.b, transitionPercent);
    }

    public void LoadLevel(string levelName) 
    {
        targetScene = levelName;
        EventManager.Instance.LeaveScene.Invoke();
    }

    public void FauxTransitionScreen() 
    {
        StartCoroutine(OnOffTransition());
    }

    private IEnumerator OnOffTransition() 
    {
        OnExitScene();
        yield return new WaitForSeconds(1.5f);
        OnEnterScene();
    }

    private void OnEnterScene() 
    {
        transitionDir = -1;
        if(!inTransition)StartCoroutine(TransitionScreenSwitch(false));
    }

    private void OnExitScene() 
    {
        transitionDir = 1;
        if (!inTransition) StartCoroutine(TransitionScreenSwitch(true));
    }

    private IEnumerator TransitionScreenSwitch(bool loadOnComplete)
    {
        inTransition = true;

        while ((transitionPercent < 1 && transitionDir == 1) || (transitionPercent > 0 && transitionDir == -1))
        {
            transitionPercent += Time.deltaTime * transitionDir * transitionSpeed;
            yield return new WaitForEndOfFrame();
        }

        transitionPercent = Mathf.Clamp(transitionPercent, 0, 1);
        inTransition = false;
        transitionDir = 0;


        if (loadOnComplete && targetScene != "")
        {
            SceneManager.LoadScene(targetScene);
            targetScene = "";
            yield return new WaitForSeconds(0.5f);
            EventManager.Instance.EnterScene.Invoke();
        }
    }
}
