using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public enum GunName
{
    Minigun,
    Pistol,
    RocketLauncher,
    FlameThrower
}

[System.Serializable]
public class GunBase
{
    public GunName GunNames;
    public GameObject GunPrefab;
    public float GunShootingInterval;
    public float3 MuzzleLocalPosition;
    public float TurretRotateValue;
}

public class GunAndBulletFactoryControllerMono : MonoBehaviour
{
    public GunBase Minigun;
    public GameObject DefaultBulletObject;

    public GunBase RocketLauncher;
    public GameObject RocketLauncherBulletObject;

    public GameObject ExplosionPropObject;
    public GameObject ExplosionEffectObject;

    public GameObject BulletEffectEntity;
}

public class GunControllerMonoBaker : Baker<GunAndBulletFactoryControllerMono>
{
    public override void Bake(GunAndBulletFactoryControllerMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new GunFactoryData
        {
            MinigunEntity = GetEntity(authoring.Minigun.GunPrefab, TransformUsageFlags.Dynamic),
            MinigunShootingInterval = authoring.Minigun.GunShootingInterval,
            MiniGunName = authoring.Minigun.GunNames,
            MinigunMuzzleLocalPosition = authoring.Minigun.MuzzleLocalPosition,

            RocketLauncherEntity = GetEntity(authoring.RocketLauncher.GunPrefab, TransformUsageFlags.Dynamic),
            RocketLauncherShootingInterval = authoring.RocketLauncher.GunShootingInterval,
            RocketLauncherName = authoring.RocketLauncher.GunNames,
            RocketLauncherMuzzleLocalPosition = authoring.RocketLauncher.MuzzleLocalPosition
        });

        AddComponent(entity, new BulletFactoryData
        {
            DefaultBulletObject = GetEntity(authoring.DefaultBulletObject, TransformUsageFlags.Dynamic),
            RocketLauncherBulletObject = GetEntity(authoring.RocketLauncherBulletObject, TransformUsageFlags.Dynamic),
            ExplosionPropObject = GetEntity(authoring.ExplosionPropObject,TransformUsageFlags.Dynamic),
            ExplosiveEffectEntity = GetEntity(authoring.ExplosionEffectObject, TransformUsageFlags.Dynamic),
            BulletEffectEntity = GetEntity(authoring.BulletEffectEntity, TransformUsageFlags.Dynamic)
        });
    }
}