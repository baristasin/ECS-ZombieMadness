using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct ZombieDamageEmissionEffectSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbBSESingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecbBSE = ecbBSESingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (healthData, zombieEntity) in SystemAPI.Query<RefRW<HealthData>>().WithEntityAccess())
        {
            if (healthData.ValueRO.RecentlyHitValue > 0)
            {
                var childFromEntity = state.EntityManager.GetBuffer<Child>(zombieEntity);
                healthData.ValueRW.RecentlyHitValue -= deltaTime * 2.5f;

                if (healthData.ValueRO.RecentlyHitValue <= 0)
                {
                    healthData.ValueRW.RecentlyHitValue = 0;
                }
                state.EntityManager.SetComponentData(childFromEntity[0].Value, new ZombieEmissionXValueData { XValue = healthData.ValueRO.RecentlyHitValue });

                if(healthData.ValueRO.HealthAmount <= 0)
                {
                    ecbBSE.SetComponentEnabled<HealthData>(zombieEntity, false);
                }

            }

        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

}