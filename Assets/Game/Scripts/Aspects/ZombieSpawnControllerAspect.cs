using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public readonly partial struct ZombieSpawnControllerAspect  : IAspect
{
    public readonly Entity Entity;

    private readonly RefRO<LocalTransform> _transform;
    private LocalTransform Transform => _transform.ValueRO;

    public readonly RefRO<ZombieSpawnData> ZombieSpawnData;

    public readonly RefRO<ZombieFactoryData> ZombieFactoryData;

    public readonly DynamicBuffer<ZombieWaveBufferElement> ZombieWaveBufferElements;

    public Entity GetRandomZombie()
    {
        return ZombieFactoryData.ValueRO.Zombie1Entity;
    }

    //public LocalTransform GetZombieTransform(int currentZombieCount, int waveCount)
    //{
    //    var firstSpawnCoordinate = ZombieSpawnData.ValueRO.FirstSpawnCoordinate;

    //    var spawnLineZombieCount = ZombieWaveBufferElements[waveCount].WaveSpawnLineCount;
    //    var lineIndex = currentZombieCount / ZombieWaveBufferElements[waveCount].WaveSpawnLineCount;

    //    return new LocalTransform
    //    {
    //        Position = new float3((firstSpawnCoordinate.x - (spawnLineZombieCount / 2)) + (currentZombieCount % spawnLineZombieCount), 0, firstSpawnCoordinate.y + lineIndex),
    //        Rotation = quaternion.identity,
    //        Scale = 1f
    //    };
    //}

    public LocalTransform GetZombieTransformRandomPositioned()
    {
        return new LocalTransform
        {
            Position = GetRandomPosition(),
            Rotation = quaternion.identity,
            Scale = 1f
        };
    }

    private float3 GetRandomPosition()
    {
        float3 randomPosition;

        do
        {
            randomPosition = new float3
            {
                x = Random.Range(-ZombieSpawnData.ValueRO.FirstSpawnCoordinate.x / 2,
                    ZombieSpawnData.ValueRO.FirstSpawnCoordinate.x / 2),
                y = 0.5f,
                z = Random.Range(-ZombieSpawnData.ValueRO.FirstSpawnCoordinate.y / 2,
                    ZombieSpawnData.ValueRO.FirstSpawnCoordinate.y / 2)
            };
        } while (math.distancesq(Transform.Position, randomPosition) <= 5);

        return randomPosition;


    }
}