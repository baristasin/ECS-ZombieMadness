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
                if (zombieDieAnimationData.ValueRO.DeadAnimationType == DeadAnimationType.BulletDie)
                {

                    if (zombieDieAnimationData.ValueRO.IsDieAnimationStarted == 0)
                    {
                        var randomNum = Random.Range(3, 7);

                        while (_lastDieAnimationNumber == randomNum)
                        {
                            randomNum = Random.Range(3, 7);
                        }

                        _lastDieAnimationNumber = randomNum;

                        state.EntityManager.GetAspect<GpuEcsAnimatorAspect>(zombieEntity).RunAnimation(
            randomNum, 1, 1, 1, .5f);

                        zombieDieAnimationData.ValueRW.IsDieAnimationStarted = 1;
                    }
                }
                else
                {
                    if (zombieDieAnimationData.ValueRO.IsDieAnimationStarted == 0)
                    {
                        state.EntityManager.GetAspect<GpuEcsAnimatorAspect>(zombieEntity).RunAnimation(
            6, 1, 1, 1, .5f);

                        zombieDieAnimationData.ValueRW.IsDieAnimationStarted = 1;
                    }

                    if(zombieDieAnimationData.ValueRO.TimeBeforeDestroy > 3.5f)
                    {
                        zombiePhysicsVelocity.ValueRW.Linear += new float3(0, 2f, -1f) * deltaTime;
                    }
                    else if(zombieDieAnimationData.ValueRO.TimeBeforeDestroy > 3)
                    {
                    zombiePhysicsVelocity.ValueRW.Linear += new float3(0, -2f,-1f) * deltaTime;
                    }
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