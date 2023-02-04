using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public void Awake()
    {
        if (Instance is null) Instance = this;
        if (Instance != this)
        {
            Destroy(this);
        }
    }
    [SerializeField] public MainCharacterController playerController;
    [SerializeField] public GameObject playerObj;
}