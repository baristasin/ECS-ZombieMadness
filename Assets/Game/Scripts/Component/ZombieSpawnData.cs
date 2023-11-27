using Unity.Entities;
using Unity.Mathematics;

public struct ZombieSpawnData  : IComponentData
{
    public float2 SpawnCoordinage;
    public float ZombieMaxSpeed;
    public float ZombieMinSpeed;
}