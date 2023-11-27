using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using UnityEngine;
using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Random = UnityEngine.Random;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct ZombieDieSystem : ISystem
{
    private int _lastDieAnimationNumber;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (zombieLocalTransform,
            zombiePhysicsVelocity,
            zombiePhysicsMass,
            zombieDieAnimationData,
            zombieEntity) in SystemAPI.Query<LocalTransform,
            RefRW<PhysicsVelocity>,
            PhysicsMass,
            RefRW<ZombieDieAnimationData>
            >().WithEntityAccess())
        {
            if (zombieDieAnimationData.ValueRO.TimeBeforeDestroy > 0)
            {
                //Unity.Physics.Extensions.PhysicsComponentExtensions.ApplyImpulse(
                //    ref zombiePhysicsVelocity.ValueRW,
                //    zombiePhysicsMass,
                //    new float3(5,5,5),
                //    quaternion.identity,
                //    new float3(500, 500, 500),
                //    new float3(0, 0, 0));

                if (zombieDieAnimationData.ValueRO.DeadAnimationType == DeadAnimationType.BulletDie)
                {
                    if (zombieDieAnimationData.ValueRO.IsDieAnimationStarted == 0)
                    {
                        var randomNum = Random.Range(1, 4);

                        while (_lastDieAnimationNumber == randomNum)
                        {
                            randomNum = Random.Range(1, 4);
                        }

                        _lastDieAnimationNumber = randomNum;

                        state.EntityManager.GetAspect<GpuEcsAnimatorAspect>(zombieEntity).RunAnimation(
            randomNum, 1, 1, 1, .5f);

                        zombieDieAnimationData.ValueRW.IsDieAnimationStarted = 1;
                    }
                }
                else
                {
                    zombiePhysicsVelocity.ValueRW.Linear += new float3(0, 10f, 0);

                }


                zombieDieAnimationData.ValueRW.TimeBeforeDestroy -= deltaTime;
            }
            else
            {
                ecb.DestroyEntity(zombieEntity);
            }

        }

        ecb.Playback(state.EntityManager);

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

}