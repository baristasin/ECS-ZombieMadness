using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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

    public void SetTarget(float2 zombiePositionData,float delayValue)
    {
        TurretDesiredOrientationData.ValueRW.TurretDesiredQuaternionValue = quaternion.RotateY(RotateTowards(LocalTransform.ValueRW.Position, new float3(zombiePositionData.x,0, zombiePositionData.y + (0.8f * delayValue))));
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