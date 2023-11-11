using Unity.Entities;

public readonly partial struct ZombieAspect  : IAspect
{
    public readonly Entity Entity;

    public readonly RefRO<ZombieMovementData> ZombieMovementData;
}