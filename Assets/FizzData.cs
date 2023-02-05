[System.Serializable]
public struct FizzData
{
    
    float currentMaxFizzValue;
    float currentFizzValue;
    float maxExcitement;
    float currentExcitement;
    float fizzLaunchForce;
    float fizzFillPercent;

    public FizzData(float currentMaxFizzValue, float currentFizzValue, float maxExcitement, float currentExcitement, float fizzLaunchForce, float fizzFillPercent)
    {
        this.currentMaxFizzValue = currentMaxFizzValue;
        this.currentFizzValue = currentFizzValue;
        this.maxExcitement = maxExcitement;
        this.currentExcitement = currentExcitement;
        this.fizzLaunchForce = fizzLaunchForce;
        this.fizzFillPercent = fizzFillPercent;
    }
}
