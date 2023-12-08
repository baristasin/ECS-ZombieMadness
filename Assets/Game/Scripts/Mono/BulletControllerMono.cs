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
    public DamageType DamageType;
    public GameObject BulletObject;
    public GameObject BulletEffectEntity;
    public GameObject ExplosiveEntity;
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

        if (authoring.DamageType != DamageType.Flame)
        {
            AddComponent(entity, new ProjectileMovementData
            {
                ProjectileSpeed = authoring.ProjectileSpeed,
                ProjectileLifeTime = authoring.ProjectileLifeTime
            });
        }

        AddComponent(entity, new ProjectileDamageData
        {
            DamageType = authoring.DamageType,
            DamageData = authoring.BulletDamage,
            ProjectilePiercingCountData = authoring.BulletPiercingCount,
            ExplosiveEntity = GetEntity(authoring.ExplosiveEntity, TransformUsageFlags.Renderable),
            BulletEffectEntity = GetEntity(authoring.BulletEffectEntity,
            TransformUsageFlags.Dynamic)
        });
    }
}