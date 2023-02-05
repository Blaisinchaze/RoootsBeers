using UnityEngine;

public class RootCollectible : Collectible
{
    public AudioSource pickupSoundSource;
    protected override void Behaviour(PlayerActionData playerData)
    {
        var fizzIncrease = new FizzData(collectibleValue,0,0, FizzDataBehaviour.Increment);

        PlayerManager.Instance.playerController.ReceiveFizzData(fizzIncrease);
        base.Behaviour(playerData);
    }
}