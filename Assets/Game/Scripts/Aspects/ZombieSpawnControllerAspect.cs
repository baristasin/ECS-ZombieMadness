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

    public readonly RefRW<ZombieSpawnData> ZombieSpawnData;

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

    public LocalTransform GetZombieTransformRandomPositioned(int waveCount)
    {
        var randomPos = GetRandomPosition(waveCount);

        return new LocalTransform
        {
            Position = randomPos,
            //Rotation = quaternion.RotateY(RotateTowards(randomPos,new float3(math.clamp(randomPos.x,-8f,8f),0,64f))),
            Rotation = quaternion.identity,
            Scale = 1f
        };
    }

    public void SetZombieSpawnData(float spawnCoordinate)
    {
        //ZombieSpawnData.ValueRW.SpawnCoordinage.y = spawnCoordinate;
    }

    private float3 GetRandomPosition(int waveCount)
    {
        float3 randomPosition;

        //float centerAwayValue = 5f;

        //if (isBossWave == 1) centerAwayValue = 50f;

        do
        {
            randomPosition = new float3
            {
                x = Random.Range(-ZombieSpawnData.ValueRO.SpawnCoordinage.x / 2,
                    ZombieSpawnData.ValueRO.SpawnCoordinage.x / 2),
                y = 0.5f,
                z = Random.Range(-ZombieSpawnData.ValueRO.SpawnCoordinage.y * waveCount,
                    35f - (waveCount * 15f))
            };
        } while (math.distancesq(Transform.Position, randomPosition) <= 1200f - (math.clamp(waveCount,0,1) * 1000));

        return randomPosition;
    }


    private float RotateTowards(float3 objectsPosition, float3 targetPosition)
    {
        var x = objectsPosition.x - targetPosition.x;
        var y = objectsPosition.z - targetPosition.z;

        return math.atan2(x, y) + math.PI;
    }
}