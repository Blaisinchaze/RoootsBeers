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

    private void Start()
    {
        EventManager.Instance.SceneChange.AddListener(OnSceneChange);

        OnSceneChange(false);
    }

    private void Update()
    {
        transitionScreen.color = new Color(transitionScreen.color.r, transitionScreen.color.g, transitionScreen.color.b, transitionPercent);

        if (Input.GetKeyDown(KeyCode.Space)) OnSceneChange(false);
        if (Input.GetKeyDown(KeyCode.L)) OnSceneChange(true);
    }

    //Call this
    public void LoadLevel(string levelName) 
    {
        SceneManager.LoadScene(levelName);
        EventManager.Instance.SceneChange.Invoke(false);
    }

    private void OnSceneChange(bool isLoading) 
    {
        transitionDir = isLoading ? 1 : -1;
        if(!inTransition) StartCoroutine(TransitionScreenSwitch());
    }

    private IEnumerator TransitionScreenSwitch() 
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
    }
}
