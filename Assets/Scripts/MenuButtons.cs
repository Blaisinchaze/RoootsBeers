using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    public float gizmoSize;
    [Space]
    public Vector3[] menuPositions;
    public GameObject[] buttons;
    public List<Renderer>[] renderers;
    public UnityEvent[] buttonActions;
    public float buttonMoveSpeed;
    public Color buttonColour;
    private int selectedOption;
    private int[] targetIndex;

    void Start()
    {
        if (buttons.Length != menuPositions.Length) Destroy(gameObject);

        targetIndex = new int[buttons.Length];
        renderers = new List<Renderer>[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            renderers[i] = new List<Renderer>(buttons[i].GetComponentsInChildren<Renderer>());
        }

        selectedOption = 1;
        ChangeOption(false);
    }

    // Update is called once per frame
    void Update()
    {
        ButtonUpdates();

        if (Input.GetKeyDown(KeyCode.UpArrow)) ChangeOption(true);
        if (Input.GetKeyDown(KeyCode.DownArrow)) ChangeOption(false);
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) SelectOption();
    }

    void ChangeOption(bool down) 
    {
        selectedOption += down ? -1 : 1;

        if (selectedOption >= menuPositions.Length) selectedOption = 0;
        if (selectedOption < 0) selectedOption = menuPositions.Length - 1;

        int next = selectedOption;

        for (int i = 0; i < menuPositions.Length; i++, next++)
        {
            if (next >= buttons.Length) next = 0;
            targetIndex[next] = i;
        }

    }

    void SelectOption() 
    {
        if (selectedOption >= buttonActions.Length) return;

        buttonActions[selectedOption].Invoke();
    }

    void ButtonUpdates()
    {
        for (int i = 0; i < menuPositions.Length; i++)
        {
            if (targetIndex[i] == 0) { UpdateRenderers(i, buttonColour); }
            else { UpdateRenderers(i, new Color(buttonColour.r, buttonColour.g, buttonColour.b, buttonColour.a * GetAlphaForButton(targetIndex[i]))); }

            if (Vector3.Distance(buttons[i].transform.localPosition, menuPositions[targetIndex[i]]) > 0.05f)
            { buttons[i].transform.localPosition = Vector3.Lerp(buttons[i].transform.localPosition, menuPositions[targetIndex[i]], buttonMoveSpeed * Time.deltaTime); }

            //buttons[i].transform.LookAt(Camera.main.transform.position, Vector3.up);
        }
    }

    void UpdateRenderers(int idx, Color colour) 
    {
        foreach (Renderer item in renderers[idx])
        {
            if (item.GetType() == typeof(SpriteRenderer))
            {
                SpriteRenderer sr = (SpriteRenderer)item;
                sr.color = colour;
            }
            else if (item.GetType() == typeof(MeshRenderer))
            {

                if (item.TryGetComponent<TextMeshPro>(out TextMeshPro tmp))
                {
                    tmp.color = colour;
                }
                else
                {
                    MeshRenderer mr = (MeshRenderer)item;
                    mr.material.SetColor(0, colour);
                }
            }
            else
            {
                Debug.Log("Implement renderer type " + item.GetType());
            }
        }
    }

    float GetAlphaForButton(int idx) 
    {
        float maxDistanceFromZero = ((buttons.Length-1) * 0.5f);

        float alpha = 0;

        if (idx > maxDistanceFromZero) alpha = menuPositions.Length - idx;
        else alpha = idx;

        alpha = 1 / (alpha * 3);

        return alpha;
    }

    private void OnDrawGizmosSelected()
    {
        if (menuPositions != null && menuPositions.Length > 0)
        {
            foreach (Vector3 item in menuPositions)
            {
                Gizmos.color = Color.red;

                Gizmos.DrawCube(transform.position + item, Vector3.one * gizmoSize);
            }
        }
    }
}
