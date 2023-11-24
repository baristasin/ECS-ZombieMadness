using Unity.Entities;

public struct HealthData  : IComponentData
{
    public float RecentlyHitValue;
    public int HealthAmount;
}