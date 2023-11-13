using Unity.Entities;

public struct GunFactoryData  : IComponentData
{
    public GunName GunName;
    public Entity MinigunEntity;
    public float MinigunShootingInterval;
}