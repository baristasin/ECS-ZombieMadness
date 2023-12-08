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
    private float _detectionCooldown;

    private Quaternion _newQuaternion;

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


        foreach (var (turretData, turretLocalTransform, entity) in SystemAPI.Query<TurretData, LocalTransform>().WithEntityAccess())
        {

            if (_detectionCooldown <= 0)
            {
                float3 desiredTargetingPosition = new float3(0,0,0);
                float currentDistance = 999;

                var turretPosition = turretLocalTransform.Position;

                float speedDelayValue = 1f;

                foreach (var (enemyTransform, enemyMovementData, enemyPositionData) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<ZombieMovementData>, RefRO<ZombiePositionData>>())
                {
                    var dist = math.distance(turretPosition, enemyPositionData.ValueRO.ZombiePosition);
                    if (dist < currentDistance)
                    {
                        desiredTargetingPosition = enemyPositionData.ValueRO.ZombiePosition + new float3(0,3.5f,0);
                        speedDelayValue = enemyMovementData.ValueRO.ZombieMoveSpeed / 4f;
                        currentDistance = dist;
                    }
                }

                if (speedDelayValue < 0) speedDelayValue = 1f;
                state.EntityManager.GetAspect<TurretAspect>(entity).SetTarget(desiredTargetingPosition);
                _detectionCooldown += .1f;

                if (turretData.GunName == GunName.FlameThrower)
                {
                    var buffer2 = state.EntityManager.GetBuffer<Child>(entity);
                    var currentTransform2 = state.EntityManager.GetComponentData<LocalTransform>(buffer2[0].Value);

                    var desiredNewTransform = new LocalTransform
                    { Position = currentTransform2.Position,
                        Rotation = Quaternion.LookRotation(desiredTargetingPosition - currentTransform2.Position, Vector3.up),
                        Scale = currentTransform2.Scale };

                    _newQuaternion = Quaternion.RotateTowards(currentTransform2.Rotation, desiredNewTransform.Rotation, 50 * deltaTime);
                }

            }
            else
            {
                _detectionCooldown -= deltaTime;
            }

            state.EntityManager.GetAspect<TurretAspect>(entity).Rotate(deltaTime);


            var buffer = state.EntityManager.GetBuffer<Child>(entity);
            var currentTransform = state.EntityManager.GetComponentData<LocalTransform>(buffer[0].Value);

            state.EntityManager.SetComponentData<LocalTransform>(buffer[0].Value, new LocalTransform
            {
                Position = currentTransform.Position,
                Rotation = _newQuaternion,
                Scale = currentTransform.Scale
            });

        }


    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

}