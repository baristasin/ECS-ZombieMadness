using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct TurretAspect  : IAspect
{
    public readonly Entity Entity;

    public readonly RefRW<LocalTransform> LocalTransform;

    public readonly RefRW<TurretData> TurretData;

    public void ShootCooldownTick(float deltaTime)
    {
        TurretData.ValueRW.CurrentGunShootingCounter -= deltaTime;
    }

    public void SetCooldown()
    {
        TurretData.ValueRW.CurrentGunShootingCounter = TurretData.ValueRO.GunShootingInterval;
    }

    public void Target(float2 zombiePositionData)
    {
        LocalTransform.ValueRW.Rotation = quaternion.RotateY(RotateTowards(LocalTransform.ValueRW.Position, new float3(zombiePositionData.x,0, zombiePositionData.y)));
    }

    private float RotateTowards(float3 objectsPosition, float3 targetPosition)
    {
        var x = objectsPosition.x - targetPosition.x;
        var y = objectsPosition.z - targetPosition.z;

        return math.atan2(x, y) + math.PI;
    }
}