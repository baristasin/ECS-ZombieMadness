using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

//[DisableAutoCreation]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct TurretAutoFiringSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        var ecbBSSSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecbBSS = ecbBSSSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        new TurretShootJob
        {
            EcbBSS = ecbBSS,
            DeltaTime = deltaTime
        }.ScheduleParallel();

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

}

[BurstCompile]
public partial struct TurretShootJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter EcbBSS;
    public float DeltaTime;

    [BurstCompile]
    public void Execute(TurretAspect turretAspect, [EntityIndexInQuery] int sortKey)
    {
        if(turretAspect.TurretData.ValueRO.CurrentGunShootingCounter > 0)
        {
            turretAspect.ShootCooldownTick(DeltaTime);
            return;
        }

        var bulletEntity = EcbBSS.Instantiate(sortKey, turretAspect.TurretData.ValueRO.GunBulletObject);

        EcbBSS.SetComponent(sortKey,bulletEntity, new LocalTransform
        {
            Position = turretAspect.LocalTransform.ValueRO.Position + turretAspect.TurretData.ValueRO.MuzzlePosDifferenceValue,
            Rotation = turretAspect.LocalTransform.ValueRO.Rotation,
            Scale = 1f
            });

        turretAspect.SetCooldown();
    }
}