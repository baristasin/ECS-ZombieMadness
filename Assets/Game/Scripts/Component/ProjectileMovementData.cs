using Unity.Entities;

public struct ProjectileMovementData  : IComponentData
{
    public float ProjectileLifeTime;
    public float ProjectileSpeed;
}