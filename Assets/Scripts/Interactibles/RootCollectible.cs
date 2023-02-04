public class RootCollectible : Collectible
{
    protected override void Behaviour(PlayerActionData playerData)
    {
        var fizzIncrease = new FizzData(0,0,collectibleValue,0,0,0, FizzDataBehaviour.Increment);

        PlayerManager.Instance.playerController.ReceiveFizzData(fizzIncrease);
        base.Behaviour(playerData);
    }
}