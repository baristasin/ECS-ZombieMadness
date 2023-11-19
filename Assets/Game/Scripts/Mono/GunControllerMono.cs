using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public enum GunName
{
    Minigun,
    Pistol,
    PlasmaGun
}

[System.Serializable]
public class GunBase
{
    public GunName GunNames;
    public GameObject GunPrefab;
    public float GunShootingInterval;
}

public class GunControllerMono : MonoBehaviour
{
    public GunBase Minigun;
    public GameObject DefaultBulletObject;

    public GunBase PlasmaGun;
    public GameObject PlasmaGunBulletObject;
}

public class GunControllerMonoBaker : Baker<GunControllerMono>
{
    public override void Bake(GunControllerMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new GunFactoryData
        {
            MinigunEntity = GetEntity(authoring.Minigun.GunPrefab, TransformUsageFlags.Dynamic),
            MinigunShootingInterval = authoring.Minigun.GunShootingInterval,
            MiniGunName = authoring.Minigun.GunNames,

            PlasmaGunEntity = GetEntity(authoring.PlasmaGun.GunPrefab, TransformUsageFlags.Dynamic),
            PlasmaGunShootingInterval = authoring.PlasmaGun.GunShootingInterval,
            PlasmaGunName = authoring.PlasmaGun.GunNames
        });

        AddComponent(entity, new BulletFactoryData
        {
            DefaultBulletObject = GetEntity(authoring.DefaultBulletObject, TransformUsageFlags.Dynamic),
            PlasmaGunBulletObject = GetEntity(authoring.PlasmaGunBulletObject, TransformUsageFlags.Dynamic),
        });
    }
}