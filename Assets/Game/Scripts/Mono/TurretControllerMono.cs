using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class TurretControllerMono  : MonoBehaviour
{
    public GunBase GunBase;
    public GameObject DefaultBulletObject;
}

public class TurretControllerMonoBaker  : Baker<TurretControllerMono>
{
    public override void Bake(TurretControllerMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity,new LocalTransform
        {
            Position = new float3(0, 0, 0),
            Rotation = quaternion.identity,
            Scale = 1f
        });

        AddComponent(entity, new TurretData
        {
            GunName = authoring.GunBase.GunNames,
            GunShootingInterval = authoring.GunBase.GunShootingInterval,
            CurrentGunShootingCounter = authoring.GunBase.GunShootingInterval,
            MuzzlePosDifferenceValue = authoring.GunBase.MuzzleLocalPosition,
            GunBulletObject = GetEntity(authoring.DefaultBulletObject,TransformUsageFlags.Dynamic)
        });
    }
}