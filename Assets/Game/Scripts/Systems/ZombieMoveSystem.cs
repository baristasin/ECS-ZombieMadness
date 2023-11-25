using System.Drawing;
using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct ZombieMoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ZombieMovementData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (zombieLocalTransform, zombieMovementData, zombieEntity) in SystemAPI.Query<RefRW<LocalTransform>, ZombieMovementData>().WithEntityAccess())
        {
            zombieLocalTransform.ValueRW.Position.z += zombieMovementData.ZombieMoveSpeed * deltaTime;

                float runAnimationValue = zombieMovementData.ZombieMoveSpeed / 3f;

            state.EntityManager.GetAspect<GpuEcsAnimatorAspect>(zombieEntity).RunAnimation(
                (int)AnimationIdsZombieMovement.ZombieMovement,  runAnimationValue);
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

}