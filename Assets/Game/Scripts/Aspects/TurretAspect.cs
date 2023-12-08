using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public readonly partial struct TurretAspect  : IAspect
{
    public readonly Entity Entity;

    public readonly RefRW<LocalTransform> LocalTransform;

    public readonly RefRW<TurretData> TurretData;

    public readonly RefRW<TurretDesiredOrientationData> TurretDesiredOrientationData;

    public void ShootCooldownTick(float deltaTime)
    {
        TurretData.ValueRW.CurrentGunShootingCounter -= deltaTime;
    }

    public void SetCooldown()
    {
        TurretData.ValueRW.CurrentGunShootingCounter = TurretData.ValueRO.GunShootingInterval;
    }

    public void SetTarget(float3 zombiePositionData)
    {
        //TurretDesiredOrientationData.ValueRW.TurretDesiredQuaternionValue = quaternion.RotateY(RotateTowards(LocalTransform.ValueRW.Position, zombiePositionData));
        TurretDesiredOrientationData.ValueRW.TurretDesiredQuaternionValue = Quaternion.LookRotation(zombiePositionData - LocalTransform.ValueRO.Position, Vector3.up);       
    }

    private float RotateTowards(float3 objectsPosition, float3 targetPosition)
    {
        var x = objectsPosition.x - targetPosition.x;
        var y = objectsPosition.z - targetPosition.z;

        return math.atan2(x, y) + math.PI;
    }

    public void Rotate(float deltaTime)
    {
        LocalTransform.ValueRW.Rotation = Quaternion.RotateTowards(LocalTransform.ValueRW.Rotation, TurretDesiredOrientationData.ValueRO.TurretDesiredQuaternionValue,TurretDesiredOrientationData.ValueRO.TurretRotationSpeed * deltaTime);        
    }
}