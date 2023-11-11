using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class ZombieWave
{
    public int WaveTotalZombieCount;
    public int WaveSpawnLineCount;
    public float WaitAfterSeconds;
    public bool IsRandomLocationSpawn;
}

public class ZombieSpawnControllerMono  : MonoBehaviour
{
    public GameObject Zombie1Object;
    public float2 SpawnDimensions;
    public List<ZombieWave> ZombieWaves;
}

public class ZombieControllerMonoBaker  : Baker<ZombieSpawnControllerMono>
{
    public override void Bake(ZombieSpawnControllerMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        var waveBuffer = AddBuffer<ZombieWaveBufferElement>(entity);

        for (int i = 0; i < authoring.ZombieWaves.Count; i++)
        {
            waveBuffer.Add(new ZombieWaveBufferElement
            {
                IsRandomLocationSpawn = authoring.ZombieWaves[i].IsRandomLocationSpawn == true ? 1 : 0,
                WaveSpawnLineCount = authoring.ZombieWaves[i].WaveSpawnLineCount,
                WaveTotalZombieCount = authoring.ZombieWaves[i].WaveTotalZombieCount,
                WaitAfterSeconds = authoring.ZombieWaves[i].WaitAfterSeconds
            });
        }

        AddComponent(entity, new ZombieFactoryData
        {
            Zombie1Entity = GetEntity(authoring.Zombie1Object,TransformUsageFlags.Dynamic)
        });

        AddComponent(entity, new ZombieSpawnData
        {
            FirstSpawnCoordinate = authoring.SpawnDimensions,
        });

    }
}