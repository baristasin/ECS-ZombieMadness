using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct ZombieSpawnSystem : ISystem
{
    private int _waveCount;
    private int _zombieCount;

    private float _currentTickAmount;
    private float _totalTickAmount;

    public BufferLookup<ZombieWaveBufferElement> _zombieWaveBufferLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _waveCount = 0;
        _zombieCount = 0;

        state.RequireForUpdate<ZombieSpawnData>();

        _zombieWaveBufferLookup = state.GetBufferLookup<ZombieWaveBufferElement>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        if(_currentTickAmount < _totalTickAmount)
        {
            _currentTickAmount += deltaTime;
            return;
        }

        var zombieSpawnControllerAspectSingleton = SystemAPI.GetSingletonEntity<ZombieSpawnData>();
        var zombieSpawnControllerAspect = SystemAPI.GetAspect<ZombieSpawnControllerAspect>(zombieSpawnControllerAspectSingleton);

        _zombieWaveBufferLookup.Update(ref state);

        _zombieWaveBufferLookup.TryGetBuffer(zombieSpawnControllerAspect.Entity, out var buffer);

        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        for (int i = 0; i < buffer[_waveCount].WaveTotalZombieCount; i++)
        {
            var zombieEntity = ecb.Instantiate(zombieSpawnControllerAspect.GetRandomZombie());
            ecb.AddComponent(zombieEntity, zombieSpawnControllerAspect.GetZombieTransform(_zombieCount, _waveCount));

            _zombieCount++;
        }

        _zombieCount = 0;

        _currentTickAmount = 0;
        _totalTickAmount = buffer[_waveCount].WaitAfterSeconds;

        _waveCount++;

        if (_waveCount >= buffer.Length)
        {
            Debug.Log("All Waves Spawned");
            state.Enabled = false;
        }

        ecb.Playback(state.EntityManager);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

}