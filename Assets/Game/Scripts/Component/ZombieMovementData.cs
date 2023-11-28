using Unity.Entities;

public struct ZombieMovementData  : IComponentData,IEnableableComponent
{
    public float ZombieMoveSpeed;
    public int ZombieMovementAnimationId;
}