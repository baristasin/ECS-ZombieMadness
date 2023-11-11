using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct ProjectileSystem  : ISystem
{
    private ComponentLookup<LocalTransform> _positionLookup;
    private ComponentLookup<ProjectileMovementData> _projectileMovementDataLookup;
    private ComponentLookup<ZombieMovementData> _zombieMovementDataLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _positionLookup = SystemAPI.GetComponentLookup<LocalTransform>();
        _projectileMovementDataLookup = SystemAPI.GetComponentLookup<ProjectileMovementData>();
        _zombieMovementDataLookup = SystemAPI.GetComponentLookup<ZombieMovementData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.CompleteDependency();

        var ecbESSSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecbESS = ecbESSSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (projTransform,projMovementData) in SystemAPI.Query<RefRW<LocalTransform>,ProjectileMovementData>())
        {
            projTransform.ValueRW.Position -= projTransform.ValueRO.Forward() * projMovementData.ProjectileSpeed * deltaTime;
        }

        _positionLookup.Update(ref state);
        _projectileMovementDataLookup.Update(ref state);
        _zombieMovementDataLookup.Update(ref state);
        SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        state.Dependency = new ProjectileHitJob
        {
            PositionLookup = _positionLookup,
            ProjectileMovementDataLookup = _projectileMovementDataLookup,
            ZombieMovementDataLookup = _zombieMovementDataLookup,
            EntityCommandBuffer = ecbESS
        }.Schedule(simulation,state.Dependency);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

}

[BurstCompile]
public struct ProjectileHitJob : ITriggerEventsJob
{
    public ComponentLookup<LocalTransform> PositionLookup;
    public ComponentLookup<ProjectileMovementData> ProjectileMovementDataLookup;
    public ComponentLookup<ZombieMovementData> ZombieMovementDataLookup;

    public EntityCommandBuffer EntityCommandBuffer;

    public void Execute(TriggerEvent triggerEvent)
    {
        Entity projectile = Entity.Null;
        Entity enemy = Entity.Null;

        if (ProjectileMovementDataLookup.HasComponent(triggerEvent.EntityA))
            projectile = triggerEvent.EntityA;
        if (ProjectileMovementDataLookup.HasComponent(triggerEvent.EntityB))
            projectile = triggerEvent.EntityB;
        if (ZombieMovementDataLookup.HasComponent(triggerEvent.EntityA))
            enemy = triggerEvent.EntityA;
        if (ZombieMovementDataLookup.HasComponent(triggerEvent.EntityB))
            enemy = triggerEvent.EntityB;

        if (Entity.Null.Equals(projectile) || Entity.Null.Equals(enemy))
            return;

        EntityCommandBuffer.DestroyEntity(projectile);
        EntityCommandBuffer.DestroyEntity(enemy);

    }
}








