using Unity.Entities;

public struct HealthData  : IComponentData,IEnableableComponent
{
    public float RecentlyHitValue;
    public int HealthAmount;
}