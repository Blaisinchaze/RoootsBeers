using UnityEngine;

public class Collectible : Interactible
{
    [Header("Collectible Components")]
    [SerializeField] protected float collectibleValue;
    protected override void Behaviour(PlayerActionData playerData)
    {
        //  Behaviour goes here
        OnCollected();
    }
    protected virtual void OnCollected()
    {
        Destroy(gameObject);
    }
}
