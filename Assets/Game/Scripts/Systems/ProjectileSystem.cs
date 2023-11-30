using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct ProjectileSystem : ISystem
{
    private ComponentLookup<LocalTransform> _positions;
    private ComponentLookup<ProjectileDamageData> _projectileDamageDataLookup;
    private ComponentLookup<HealthData> _healthDataLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _positions = SystemAPI.GetComponentLookup<LocalTransform>();
        _projectileDamageDataLookup = SystemAPI.GetComponentLookup<ProjectileDamageData>();
        _healthDataLookup = SystemAPI.GetComponentLookup<HealthData>();

        state.RequireForUpdate<GunFactoryData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.CompleteDependency();

        var ecbESSSingleton = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
        var ecbESS = ecbESSSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var deltaTime = SystemAPI.Time.DeltaTime;

        var gunFactoryEntitySingleton = SystemAPI.GetSingletonEntity<GunFactoryData>();
        var bulletFactoryData = SystemAPI.GetComponent<BulletFactoryData>(gunFactoryEntitySingleton);

        foreach (var (projTransform, projMovementData,projEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<ProjectileMovementData>>().WithEntityAccess())
        {
            projMovementData.ValueRW.ProjectileLifeTime -= deltaTime;

            if(projMovementData.ValueRO.ProjectileLifeTime <= 0)
            {
                ecbESS.DestroyEntity(projEntity);
            }
            else
            {
                projTransform.ValueRW.Position += projTransform.ValueRO.Forward() * projMovementData.ValueRO.ProjectileSpeed * deltaTime;
            }
        }

        _positions.Update(ref state);
        _projectileDamageDataLookup.Update(ref state);
        _healthDataLookup.Update(ref state);
        SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        state.Dependency = new ProjectileHitJob
        {
            Positions = _positions,
            ProjectileDamageDataLookup = _projectileDamageDataLookup,
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
public struct ProjectileHitJob : ITriggerEventsJob
{
    public ComponentLookup<LocalTransform> Positions; // For VFX
     public ComponentLookup<ProjectileDamageData> ProjectileDamageDataLookup;
    public ComponentLookup<HealthData> HealthDataLookup;

    public EntityCommandBuffer EntityCommandBuffer;

    public void Execute(TriggerEvent triggerEvent)
    {
        Entity projectile = Entity.Null;
        Entity enemy = Entity.Null;

        if (ProjectileDamageDataLookup.HasComponent(triggerEvent.EntityA))
            projectile = triggerEvent.EntityA;
        if (ProjectileDamageDataLookup.HasComponent(triggerEvent.EntityB))
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
        healthData.HealthAmount -= ProjectileDamageDataLookup[projectile].DamageData;
        healthData.RecentlyHitValue = .3f;

        if(healthData.HealthAmount <= 0)
        {
            healthData.IsHealthDepleted = 1;
            DeadAnimationType deadAnimationType = DeadAnimationType.BulletDie;

            if (ProjectileDamageDataLookup[projectile].DamageType == DamageType.Bullet)
            {

            }
            else if(ProjectileDamageDataLookup[projectile].DamageType == DamageType.Explosive)
            {
                deadAnimationType = DeadAnimationType.ExplosionDie;
            }

            EntityCommandBuffer.SetComponent(enemy, new ZombieDieAnimationData
            {
                ProjectileHitDirection = Positions[projectile].Position,
                TimeBeforeDestroy = 4f,
                DeadAnimationType = deadAnimationType
            });
            EntityCommandBuffer.SetComponentEnabled<ZombieMovementData>(enemy, false);
            EntityCommandBuffer.SetComponentEnabled<ZombieDieAnimationData>(enemy, true);
        }

        HealthDataLookup[enemy] = healthData;


        if (ProjectileDamageDataLookup[projectile].ProjectilePiercingCountData <= 0)
        {
            if (ProjectileDamageDataLookup[projectile].DamageType == DamageType.Explosive)
            {
                var explosionProp = EntityCommandBuffer.Instantiate(ProjectileDamageDataLookup[projectile].ExplosiveEntity);

                EntityCommandBuffer.SetComponent(explosionProp,
                    LocalTransform.FromPosition(new float3(Positions[projectile].Position.x,0, Positions[projectile].Position.z)));

                EntityCommandBuffer.AddComponent(explosionProp, new ExplosionPropData {GrowValue = 0.2f,LifeTime = 2f,DamageData = 1000 });
            }

            EntityCommandBuffer.DestroyEntity(projectile);
        }
        else
        {
            ProjectileDamageData projDamageData = ProjectileDamageDataLookup[projectile];
            projDamageData.ProjectilePiercingCountData -= 1;
            ProjectileDamageDataLookup[projectile] = projDamageData;
        }
        //EntityCommandBuffer.DestroyEntity(enemy);

    }
}








