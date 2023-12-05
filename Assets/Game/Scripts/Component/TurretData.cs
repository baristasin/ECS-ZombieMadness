using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct TurretData  : IComponentData
{
    public GunName GunName;
    public float GunShootingInterval;
    public float CurrentGunShootingCounter;
    public float3 MuzzlePosDifferenceValue;
    public Entity GunBulletObject;
}