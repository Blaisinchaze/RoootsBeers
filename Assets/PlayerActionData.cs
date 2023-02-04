public abstract class PlayerActionData
{
    protected PlayerStates activeState;
    protected bool isGrounded;
    protected FizzData fizzData;

    public PlayerStates State 
    { 
        get => activeState; 
        private set => activeState = value; 
    }
    public bool IsGrounded 
    { 
        get => isGrounded; 
        private set => isGrounded = value; 
    }

    public PlayerActionData(FizzData fizzData, PlayerStates currentPlayerState = PlayerStates.GROUNDED, bool isPlayerGrounded = false)
    {
        this.isGrounded = isPlayerGrounded;
        this.activeState = currentPlayerState;
        this.fizzData = fizzData;
    }
}
