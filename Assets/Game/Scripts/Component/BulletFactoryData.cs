using Unity.Entities;

public struct BulletFactoryData  : IComponentData
{
    public Entity DefaultBulletObject;
    public Entity RocketLauncherBulletObject;
    public Entity ExplosionPropObject;
}