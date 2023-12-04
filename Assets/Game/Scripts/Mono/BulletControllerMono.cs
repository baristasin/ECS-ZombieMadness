using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

//public enum BulletType
//{
//    DefaultBullet,
//    RocketBullet
//}

public class BulletControllerMono : MonoBehaviour
{
    public DamageType BulletType;
    public GameObject BulletObject;
    public GameObject BulletEffectEntity;
    public int BulletPiercingCount;
    public int BulletDamage;
    public float ProjectileSpeed;
    public float ProjectileLifeTime;
}

public class BulletControllerMonoBaker : Baker<BulletControllerMono>
{
    public override void Bake(BulletControllerMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new ProjectileMovementData {
            ProjectileSpeed = authoring.ProjectileSpeed,
            ProjectileLifeTime = authoring.ProjectileLifeTime });

        AddComponent(entity, new ProjectileDamageData {
            DamageType = DamageType.Bullet,
            DamageData = authoring.BulletDamage,
            ProjectilePiercingCountData = authoring.BulletPiercingCount,
            BulletEffectEntity = GetEntity(authoring.BulletEffectEntity,
            TransformUsageFlags.Dynamic) });
    }
}