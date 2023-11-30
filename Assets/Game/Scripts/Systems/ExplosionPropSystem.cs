using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct ExplosionPropSystem : ISystem
{
    private ComponentLookup<LocalTransform> _positionLookup;
    private ComponentLookup<ExplosionPropData> _explosionPropDataLookup;
    private ComponentLookup<HealthData> _healthDataLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _positionLookup = SystemAPI.GetComponentLookup<LocalTransform>();
        _explosionPropDataLookup = SystemAPI.GetComponentLookup<ExplosionPropData>();
        _healthDataLookup = SystemAPI.GetComponentLookup<HealthData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.CompleteDependency();

        var ecbESSSingleton = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
        var ecbESS = ecbESSSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (explosionProp,explosionPropEntity) in SystemAPI.Query<RefRW<ExplosionPropData>>().WithEntityAccess())
        {
            if (explosionProp.ValueRO.LifeTime > 0)
            {
                explosionProp.ValueRW.LifeTime -= deltaTime;
            }
            else
            {
                ecbESS.DestroyEntity(explosionPropEntity);
            }
        }

        _positionLookup.Update(ref state);
        _explosionPropDataLookup.Update(ref state);
        _healthDataLookup.Update(ref state);
        SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        state.Dependency = new ExplosionPropHitJob
        {
            PositionLookup = _positionLookup,
            ExplosionPropData = _explosionPropDataLookup,
            HealthDataLookup = _healthDataLookup,
            EntityCommandBuffer = ecbESS
        }.Schedule(simulation, state.Dependency);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

}

[BurstCompile]
public struct ExplosionPropHitJob : ITriggerEventsJob
{
    public ComponentLookup<ExplosionPropData> ExplosionPropData;
    public ComponentLookup<LocalTransform> PositionLookup; // For VFX
    public ComponentLookup<HealthData> HealthDataLookup;

    public EntityCommandBuffer EntityCommandBuffer;

    public void Execute(TriggerEvent triggerEvent)
    {
        Entity projectile = Entity.Null;
        Entity enemy = Entity.Null;

        if (ExplosionPropData.HasComponent(triggerEvent.EntityA))
            projectile = triggerEvent.EntityA;
        if (ExplosionPropData.HasComponent(triggerEvent.EntityB))
            projectile = triggerEvent.EntityB;
        if (HealthDataLookup.HasComponent(triggerEvent.EntityA))
            enemy = triggerEvent.EntityA;
        if (HealthDataLookup.HasComponent(triggerEvent.EntityB))
            enemy = triggerEvent.EntityB;

        if (Entity.Null.Equals(projectile) || Entity.Null.Equals(enemy))
            return;

        if (HealthDataLookup[enemy].HealthAmount <= 0 && HealthDataLookup[enemy].IsHealthDepleted != 0)
            return;

        HealthData healthData = HealthDataLookup[enemy];
        healthData.HealthAmount -= ExplosionPropData[projectile].DamageData;
        healthData.RecentlyHitValue = .3f;

        if (healthData.HealthAmount <= 0)
        {
            healthData.IsHealthDepleted = 1;
            DeadAnimationType deadAnimationType = DeadAnimationType.ExplosionDie;

            EntityCommandBuffer.SetComponent(enemy, new ZombieDieAnimationData
            {
                ProjectileHitDirection = PositionLookup[projectile].Position,
                TimeBeforeDestroy = 4f,
                DeadAnimationType = deadAnimationType
            });
            EntityCommandBuffer.SetComponentEnabled<ZombieMovementData>(enemy, false);
            EntityCommandBuffer.SetComponentEnabled<ZombieDieAnimationData>(enemy, true);
        }

        HealthDataLookup[enemy] = healthData;
    }
}