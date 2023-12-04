using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public enum CollisionLayer
{
    Enemy = 1 << 1,
    Bullet = 1 << 2,
    Turret = 1 << 3
}

//[DisableAutoCreation]
[UpdateInGroup(typeof(PresentationSystemGroup))]
[BurstCompile]
public partial struct TurretTargetingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {        
        state.RequireForUpdate<TruckGrinderData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var truckGrinderSingleton = SystemAPI.GetSingletonEntity<TruckGrinderData>();
        var truckGrinderData = SystemAPI.GetComponent<TruckGrinderData>(truckGrinderSingleton);
        var deltaTime = SystemAPI.Time.DeltaTime;
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach (var (turretData,turretLocalTransform, entity) in SystemAPI.Query<TurretData,LocalTransform>().WithEntityAccess())
        {
            float2 desiredTargetingPosition = new float2(0, 0);
            LocalTransform desiredTargetLocalTransform;
            float currentDistance = 999;

            var turretPosition = new float2(turretLocalTransform.Position.x, turretLocalTransform.Position.z);

            float speedDelayValue = 1f;

            foreach (var (enemyTransform,enemyMovementData, enemyPositionData) in SystemAPI.Query<RefRO<LocalTransform>,RefRO<ZombieMovementData> ,RefRO<ZombiePositionData>>())
            {
                var dist = math.distance(turretPosition, enemyPositionData.ValueRO.ZombiePosition);
                if (dist < currentDistance)
                {
                    desiredTargetingPosition = enemyPositionData.ValueRO.ZombiePosition;
                    desiredTargetLocalTransform = enemyTransform.ValueRO;
                    speedDelayValue = enemyMovementData.ValueRO.ZombieMoveSpeed / 4f;
                    currentDistance = dist;
                }
            }

            if (speedDelayValue < 0) speedDelayValue = 1f;

            state.EntityManager.GetAspect<TurretAspect>(entity).SetTarget(desiredTargetingPosition, speedDelayValue);
            state.EntityManager.GetAspect<TurretAspect>(entity).Rotate(deltaTime);
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

}