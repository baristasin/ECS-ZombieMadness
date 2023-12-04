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
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();


        foreach (var (turretData,turretLocalTransform, entity) in SystemAPI.Query<TurretData,LocalTransform>().WithEntityAccess())
        {
            float2 desiredTargetingPosition = new float2(0, 0);
            float currentDistance = 999;

            var turretPosition = new float2(turretLocalTransform.Position.x, turretLocalTransform.Position.z);

            foreach (var (enemyTransform, enemyMovementData) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<ZombiePositionData>>())
            {
                var dist = math.distance(turretPosition, enemyMovementData.ValueRO.ZombiePosition);
                if (dist < currentDistance)
                {
                    desiredTargetingPosition = enemyMovementData.ValueRO.ZombiePosition;
                    currentDistance = dist;
                }
            }

            state.EntityManager.GetAspect<TurretAspect>(entity).Target(desiredTargetingPosition);
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

}