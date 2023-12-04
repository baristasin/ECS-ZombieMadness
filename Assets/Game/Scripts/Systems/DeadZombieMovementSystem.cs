using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct DeadZombieMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (deadZombieData,deadZombieLocalTransform) in SystemAPI.Query<DeadZombieMovementData,RefRW<LocalTransform>>())
        {
            deadZombieLocalTransform.ValueRW.Position.z -= 7f * deltaTime;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

}