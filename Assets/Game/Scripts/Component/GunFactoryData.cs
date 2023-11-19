using Unity.Entities;

public struct GunFactoryData  : IComponentData
{
    public GunName MiniGunName;
    public Entity MinigunEntity;
    public float MinigunShootingInterval;


    public GunName PlasmaGunName;
    public Entity PlasmaGunEntity;
    public float PlasmaGunShootingInterval;
}