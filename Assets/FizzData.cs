[System.Serializable]
public struct FizzData
{
    public FizzDataBehaviour behaviour { get; private set; }
    public float currentMaxFizzValue;
    public float currentExcitement;
    public float fizzLaunchForce;

    public FizzData(float currentFizzValue,float currentExcitement, float fizzLaunchForce, FizzDataBehaviour bhvr = FizzDataBehaviour.Readonly)
    {
        this.currentMaxFizzValue = currentFizzValue;
        this.currentExcitement = currentExcitement;
        this.fizzLaunchForce = fizzLaunchForce;
        this.behaviour = bhvr;
    }
}
public enum FizzDataBehaviour
{
   Readonly = 1,
   Increment
}