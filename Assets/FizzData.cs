[System.Serializable]
public struct FizzData
{
    public FizzDataBehaviour behaviour { get; private set; }
    public float currentMaxFizzValue;
    public float currentFizzValue;
    public float maxExcitement;
    public float currentExcitement;
    public float fizzLaunchForce;
    public float fizzFillPercent;

    public FizzData(float currentMaxFizzValue, float currentFizzValue, float maxExcitement, float currentExcitement, float fizzLaunchForce, float fizzFillPercent, FizzDataBehaviour bhvr = FizzDataBehaviour.Readonly)
    {
        this.currentMaxFizzValue = currentMaxFizzValue;
        this.currentFizzValue = currentFizzValue;
        this.maxExcitement = maxExcitement;
        this.currentExcitement = currentExcitement;
        this.fizzLaunchForce = fizzLaunchForce;
        this.fizzFillPercent = fizzFillPercent;
        this.behaviour = bhvr;
    }
}
public enum FizzDataBehaviour
{
   Readonly = 1,
   Increment
}