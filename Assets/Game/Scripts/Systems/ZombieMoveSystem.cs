using System.Drawing;
using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
[UpdateAfter(typeof(PlayerGunCreateSystem))]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct ZombieMoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ZombieMovementData>();
        state.RequireForUpdate<TruckGrinderData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var zombieSpawnControllerAspectSingleton = SystemAPI.GetSingletonEntity<ZombieSpawnData>();
        var zombieSpawnControllerAspect = SystemAPI.GetAspect<ZombieSpawnControllerAspect>(zombieSpawnControllerAspectSingleton);

        var truckGrinderSingleton = SystemAPI.GetSingletonEntity<TruckGrinderData>();
        var truckGrinderData = SystemAPI.GetComponent<TruckGrinderData>(truckGrinderSingleton);

        foreach (var (zombieLocalTransform, zombieHealthData, zombieMovementData, zombieEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<HealthData>,ZombieMovementData>().WithEntityAccess())
        {
            if (truckGrinderData.TruckGrinderPosValue.z - zombieLocalTransform.ValueRO.Position.z < 8f)
            {
                var grinderClosestX = math.clamp(zombieLocalTransform.ValueRO.Position.x, -14f, 14f);
                var grinderPosWithoutY = new float3(grinderClosestX, zombieLocalTransform.ValueRO.Position.y, truckGrinderData.TruckGrinderPosValue.z); // 7.5f

                if(truckGrinderData.TruckGrinderPosValue.z - zombieLocalTransform.ValueRO.Position.z < 1f)
                {

                    zombieHealthData.ValueRW.RecentlyHitValue = 1f;

                    state.EntityManager.SetComponentData(zombieEntity, new ZombieDieAnimationData
                    {
                        //ProjectileHitDirection = PositionLookup[projectile].Position,
                        TimeBeforeDestroy = 4f,
                        DeadAnimationType = DeadAnimationType.BulletDie
                    });
                    state.EntityManager.SetComponentEnabled<ZombieMovementData>(zombieEntity, false);
                    state.EntityManager.SetComponentEnabled<ZombieDieAnimationData>(zombieEntity, true);
                }

                zombieLocalTransform.ValueRW.Position += math.normalize(grinderPosWithoutY - zombieLocalTransform.ValueRW.Position) * zombieMovementData.ZombieMoveSpeed * deltaTime;
                zombieLocalTransform.ValueRW.Rotation = quaternion.RotateY(RotateTowards(zombieLocalTransform.ValueRO.Position, grinderPosWithoutY));
            }
            else
            {
                zombieLocalTransform.ValueRW.Position.z += zombieMovementData.ZombieMoveSpeed * deltaTime;
            }

            var scaledValue = (zombieMovementData.ZombieMoveSpeed - zombieSpawnControllerAspect.ZombieSpawnData.ValueRO.ZombieMinSpeed) / (zombieSpawnControllerAspect.ZombieSpawnData.ValueRO.ZombieMaxSpeed - zombieSpawnControllerAspect.ZombieSpawnData.ValueRO.ZombieMinSpeed);

            state.EntityManager.GetAspect<GpuEcsAnimatorAspect>(zombieEntity).RunAnimation(
                zombieMovementData.ZombieMovementAnimationId, scaledValue);
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    private float RotateTowards(float3 objectsPosition, float3 targetPosition)
    {
        var x = objectsPosition.x - targetPosition.x;
        var y = objectsPosition.z - targetPosition.z;

        return math.atan2(x, y) + math.PI;
    }

}

