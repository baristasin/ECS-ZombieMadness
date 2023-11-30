using Unity.Entities;
using Unity.Mathematics;

public struct GunFactoryData  : IComponentData
{
    public GunName MiniGunName;
    public Entity MinigunEntity;
    public float MinigunShootingInterval;
    public float3 MinigunMuzzleLocalPosition;


    public GunName RocketLauncherName;
    public Entity RocketLauncherEntity;
    public float RocketLauncherShootingInterval;
    public float3 RocketLauncherMuzzleLocalPosition;
}