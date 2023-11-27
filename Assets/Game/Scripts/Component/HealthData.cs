using Unity.Entities;

public struct HealthData  : IComponentData,IEnableableComponent
{
    public byte IsHealthDepleted;
    public float RecentlyHitValue;
    public int HealthAmount;
}