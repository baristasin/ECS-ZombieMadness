using Unity.Entities;

public struct ExplosionPropData  : IComponentData
{
    public float LifeTime;
    public float GrowValue;
    public int DamageData;
}