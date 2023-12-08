using Unity.Entities;

public enum DamageType
{
    Bullet,
    Explosive,
    Flame
}

public struct ProjectileDamageData  : IComponentData
{
    public DamageType DamageType;
    public int DamageData;
    public int ProjectilePiercingCountData;

    public Entity ExplosiveEntity;

    public Entity BulletEffectEntity;
}