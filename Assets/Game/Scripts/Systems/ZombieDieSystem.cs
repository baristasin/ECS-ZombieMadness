using Unity.Burst;
using Unity.Entities;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct ZombieDieSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (healthData,zombieEntity) in SystemAPI.Query<HealthData>().WithEntityAccess())
        {
            if(healthData.HealthAmount <= 0)
            {
                ecb.DestroyEntity(zombieEntity); // Burada bir tag işaretleyip, animasyon + die yapan bir query ye ihtiyaç var.
            }
        }

        ecb.Playback(state.EntityManager);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

}