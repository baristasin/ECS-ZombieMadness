using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct GunData  : IComponentData
{
    public GunName GunName;
    public float GunShootingInterval;
    public LocalTransform OwnerLocalTransform;
}