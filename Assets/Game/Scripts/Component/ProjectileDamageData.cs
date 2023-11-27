using Unity.Entities;

public enum DamageType
{
    Bullet,
    Explosive
}

public struct ProjectileDamageData  : IComponentData
{
    public DamageType DamageType;
    public int DamageData;
    public int ProjectilePiercingCountData;
}