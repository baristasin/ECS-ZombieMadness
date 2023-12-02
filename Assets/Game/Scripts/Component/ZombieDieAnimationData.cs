using Unity.Entities;
using Unity.Mathematics;

public enum DeadAnimationType
{
    BulletDie,
    ExplosionDie
}

public struct ZombieDieAnimationData  : IComponentData,IEnableableComponent
{
    public byte IsDieAnimationStarted;
    public float TimeBeforeDestroy;
    public float3 ProjectilePoint;
    public float3 ProjectileHitDirection;
    public DeadAnimationType DeadAnimationType;
}