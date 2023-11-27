using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
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

        _farZombieZValue = 20f;

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

        //var ecb = new EntityCommandBuffer(Allocator.Temp);
        //var ecb = new EntityCommandBuffer(Allocator.TempJob);

        zombieSpawnControllerAspect.SetZombieSpawnData(_farZombieZValue);

        for (int i = 0; i < buffer[_waveCount].WaveTotalZombieCount; i++)
        {
            Entity zombieEntityPrefab;

            zombieEntityPrefab = (zombieSpawnControllerAspect.GetRandomZombie());

            var zombieTransform = zombieSpawnControllerAspect.GetZombieTransformRandomPositioned(buffer[_waveCount].IsBossWave);

            if (zombieTransform.Position.z > _farZombieZValue)
            {
                _farZombieZValue = zombieTransform.Position.z;
            }

            var zombieEntity = ecb.Instantiate(zombieEntityPrefab);
            ecb.AddComponent(zombieEntity, zombieTransform);

            ecb.AddComponent(zombieEntity, new ZombieMovementData {
                ZombieMoveSpeed = Random.Range(zombieSpawnControllerAspect.ZombieSpawnData.ValueRO.ZombieMinSpeed,
                zombieSpawnControllerAspect.ZombieSpawnData.ValueRO.ZombieMaxSpeed) });

            ecb.AddComponent(zombieEntity, new HealthData { HealthAmount = 100 });

            ecb.AddComponent(zombieEntity, new ZombieDieAnimationData());

            ecb.SetComponentEnabled<ZombieDieAnimationData>(zombieEntity, false);

            //state.Dependency = new ZombieSpawnJob
            //{
            //    EntityCommandBuffer = ecb,
            //    RandomZombieEntity = zombieEntityPrefab,
            //    RandomZombieTransform = zombieTransform
            //}.Schedule(state.Dependency);

            //state.CompleteDependency();
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

//[BurstCompile]
//public partial struct ZombieSpawnJob : IJob
//{
//    public EntityCommandBuffer EntityCommandBuffer;
//    public Entity RandomZombieEntity;
//    public LocalTransform RandomZombieTransform;

//    [BurstCompile]
//    public void Execute()
//    {
//        var zombieEntity = EntityCommandBuffer.Instantiate(RandomZombieEntity);
//        EntityCommandBuffer.AddComponent(zombieEntity, RandomZombieTransform);
//        EntityCommandBuffer.AddComponent(zombieEntity, new ZombieMovementData { ZombieMoveSpeed = Random.Range(1f,3f) });
//        EntityCommandBuffer.AddComponent(zombieEntity, new HealthData { HealthAmount = 100 });
//    }
//}