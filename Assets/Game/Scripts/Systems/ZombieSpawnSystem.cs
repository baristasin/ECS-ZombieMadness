using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct ZombieSpawnSystem : ISystem
{
    private int _waveCount;

    private float _currentTickAmount;
    private float _totalTickAmount;
    private float _farZombieZValue;

    public BufferLookup<ZombieWaveBufferElement> _zombieWaveBufferLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _waveCount = 0;

        state.RequireForUpdate<ZombieSpawnData>();

        _zombieWaveBufferLookup = state.GetBufferLookup<ZombieWaveBufferElement>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        if (_currentTickAmount < _totalTickAmount)
        {
            _currentTickAmount += deltaTime;
            return;
        }

        var zombieSpawnControllerAspectSingleton = SystemAPI.GetSingletonEntity<ZombieSpawnData>();
        var zombieSpawnControllerAspect = SystemAPI.GetAspect<ZombieSpawnControllerAspect>(zombieSpawnControllerAspectSingleton);

        _zombieWaveBufferLookup.Update(ref state);

        _zombieWaveBufferLookup.TryGetBuffer(zombieSpawnControllerAspect.Entity, out var buffer);

        var ecbSingleton = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        _farZombieZValue = 1000f;

        foreach (var (localTransform,zombieMovementData,zombieEntity) in SystemAPI.Query<LocalTransform,ZombieMovementData>().WithEntityAccess())
        {

            if (localTransform.Position.z < _farZombieZValue)
            {
                _farZombieZValue = localTransform.Position.z;
            }
        }

        if(_farZombieZValue == 1000f)
        {
            zombieSpawnControllerAspect.SetZombieSpawnData(0);

        }
        else
        {
            zombieSpawnControllerAspect.SetZombieSpawnData(_farZombieZValue);

        }
        for (int i = 0; i < buffer[_waveCount].WaveTotalZombieCount; i++)
        {
            Entity zombieEntityPrefab;

            zombieEntityPrefab = (zombieSpawnControllerAspect.GetRandomZombie());

            var zombieTransform = zombieSpawnControllerAspect.GetZombieTransformRandomPositioned(buffer[_waveCount].IsBossWave);

            var zombieEntity = ecb.Instantiate(zombieEntityPrefab);

            ecb.AddComponent(zombieEntity, zombieTransform);

            var randomNum = Random.Range(1,11);

            var animId = 0;

            if(randomNum > 9)
            {
                animId = 1;
            }

            else if(randomNum > 5)
            {
                animId = 2;

            }

            else
            {
                animId = 0;

            }

            ecb.AddComponent(zombieEntity, new ZombieMovementData {
                ZombieMoveSpeed = Random.Range(zombieSpawnControllerAspect.ZombieSpawnData.ValueRO.ZombieMinSpeed,
                zombieSpawnControllerAspect.ZombieSpawnData.ValueRO.ZombieMaxSpeed),
            ZombieMovementAnimationId = animId});

            ecb.AddComponent(zombieEntity, new ZombiePositionData());

            ecb.AddComponent(zombieEntity, new HealthData { HealthAmount = 100 });

            ecb.AddComponent(zombieEntity, new ZombieDieAnimationData());            

            ecb.SetComponentEnabled<ZombieDieAnimationData>(zombieEntity, false);

        }

        _currentTickAmount = 0;
        _totalTickAmount = buffer[_waveCount].WaitAfterSeconds;

        _waveCount++;

        if (_waveCount >= buffer.Length)
        {
            Debug.Log("All Waves Spawned");
            state.Enabled = false;
        }

        //ecb.Playback(state.EntityManager);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}

