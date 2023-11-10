using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct ZombieSpawnControllerAspect  : IAspect
{
    public readonly Entity Entity;

    public readonly RefRO<ZombieSpawnData> ZombieSpawnData;

    public readonly RefRO<ZombieFactoryData> ZombieFactoryData;

    public readonly DynamicBuffer<ZombieWaveBufferElement> ZombieWaveBufferElements;

    public Entity GetRandomZombie()
    {
        return ZombieFactoryData.ValueRO.Zombie1Entity;
    }

    public LocalTransform GetZombieTransform(int currentZombieCount, int waveCount)
    {
        var firstSpawnCoordinate = ZombieSpawnData.ValueRO.FirstSpawnCoordinate;

        var spawnLineZombieCount = ZombieWaveBufferElements[waveCount].WaveSpawnLineCount;
        var lineIndex = currentZombieCount / ZombieWaveBufferElements[waveCount].WaveSpawnLineCount;

        return new LocalTransform
        {
            Position = new float3((firstSpawnCoordinate.x - (spawnLineZombieCount / 2)) + (currentZombieCount % spawnLineZombieCount), 0, firstSpawnCoordinate.y + lineIndex),
            Rotation = quaternion.identity,
            Scale = 1f
        };
    }
}