using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial class PlayerGunCreateSystem : SystemBase
{
    private Camera _mainCamera;
    private Transform _mainCameraGunHoldTransform;
    private Transform _grinderTransform;
    private Entity _playerTurretEntity;
    private Entity _playerGunEntity;

    private float _shootingCooldown;
    private float _currentcooldownValue;

    [BurstCompile]
    protected override void OnCreate()
    {
        _mainCamera = Camera.main;
        _mainCameraGunHoldTransform = _mainCamera.GetComponent<CameraBehaviour>().GunHoldTransform;
        _grinderTransform = _mainCamera.GetComponent<CameraBehaviour>().GrinderTransform;
        RequireForUpdate<GunFactoryData>();
        RequireForUpdate<TruckGrinderData>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        var gunFactoryEntitySingleton = SystemAPI.GetSingletonEntity<GunFactoryData>();
        var gunFactoryData = SystemAPI.GetComponent<GunFactoryData>(gunFactoryEntitySingleton);

        var truckGrinderSingleton = SystemAPI.GetSingletonEntity<TruckGrinderData>();
        var truckGrinderData = SystemAPI.GetComponent<TruckGrinderData>(truckGrinderSingleton);

        EntityManager.SetComponentData<TruckGrinderData>(truckGrinderSingleton, new TruckGrinderData { TruckGrinderPosValue = _grinderTransform.transform.position });

        var bulletFactoryData = SystemAPI.GetComponent<BulletFactoryData>(gunFactoryEntitySingleton);
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_playerTurretEntity != Entity.Null)
            {
                EntityManager.DestroyEntity(_playerTurretEntity);
            }
            var minigunEntity = EntityManager.Instantiate(gunFactoryData.MinigunEntity);
            EntityManager.AddComponent<LocalTransform>(minigunEntity);
            EntityManager.SetComponentData<LocalTransform>(minigunEntity, new LocalTransform
            {
                Position = _mainCameraGunHoldTransform.transform.position,
                Rotation = _mainCameraGunHoldTransform.transform.rotation,
                Scale = 1f
            });

            EntityManager.AddComponentData(minigunEntity, new GunData
            {
                GunName = GunName.Minigun,
                GunShootingInterval = gunFactoryData.MinigunShootingInterval
            });

            _playerTurretEntity = minigunEntity;
            var buffer = EntityManager.GetBuffer<LinkedEntityGroup>(minigunEntity);

            _playerGunEntity = buffer[1].Value;

            _shootingCooldown = gunFactoryData.MinigunShootingInterval;
            _currentcooldownValue = _shootingCooldown;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_playerTurretEntity != Entity.Null)
            {
                EntityManager.DestroyEntity(_playerTurretEntity);
            }

            var plasmaGunEntity = EntityManager.Instantiate(gunFactoryData.PlasmaGunEntity);
            EntityManager.AddComponent<LocalTransform>(plasmaGunEntity);
            EntityManager.SetComponentData<LocalTransform>(plasmaGunEntity, new LocalTransform
            {
                Position = _mainCameraGunHoldTransform.transform.position,
                Rotation = _mainCameraGunHoldTransform.transform.rotation,
                Scale = 1f
            });

            EntityManager.AddComponentData(plasmaGunEntity, new GunData
            {
                GunName = GunName.PlasmaGun,
                GunShootingInterval = gunFactoryData.PlasmaGunShootingInterval
            });

            _playerTurretEntity = plasmaGunEntity;
            var buffer = EntityManager.GetBuffer<LinkedEntityGroup>(plasmaGunEntity);

            _playerGunEntity = buffer[1].Value;

            _shootingCooldown = gunFactoryData.PlasmaGunShootingInterval;
            _currentcooldownValue = _shootingCooldown;
        }

        if (!EntityManager.Exists(_playerTurretEntity)) return;

        var deltaTime = UnityEngine.Time.deltaTime;

        var newPosition = math.lerp(EntityManager.GetComponentData<LocalTransform>(_playerTurretEntity).Position, _mainCameraGunHoldTransform.transform.position, deltaTime * 30f);

        EntityManager.SetComponentData<LocalTransform>(_playerTurretEntity, new LocalTransform
        {
            Position = newPosition,
            //Position = _mainCameraGunHoldTransform.transform.position,
            //Rotation = _mainCameraGunHoldTransform.transform.rotation,
            Scale = 1f
        });

        var currentTransform = EntityManager.GetComponentData<LocalTransform>(_playerGunEntity);

        EntityManager.SetComponentData<LocalTransform>(_playerGunEntity, new LocalTransform
        {
            //Position = newPosition,
            //Position = _mainCameraGunHoldTransform.transform.position,
            Position = currentTransform.Position,
            Rotation = _mainCameraGunHoldTransform.transform.rotation,
            Scale = currentTransform.Scale
            //Scale = 1f
        });

        if (_currentcooldownValue > 0)
        {
            _currentcooldownValue -= deltaTime;
        }
        else
        {
            if (EntityManager.GetComponentData<GunData>(_playerTurretEntity).GunName == GunName.Minigun)
            {

                var defaultBullet = EntityManager.Instantiate(bulletFactoryData.DefaultBulletObject);
                var gunEntityTransform = EntityManager.GetComponentData<LocalTransform>(_playerTurretEntity);
                EntityManager.SetComponentData<LocalTransform>(defaultBullet, new LocalTransform
                {
                    Position = gunEntityTransform.Position + new float3(0,1.2f,0),
                    Rotation = _mainCameraGunHoldTransform.transform.rotation,
                    Scale = 1f
                });

                EntityManager.AddComponentData(defaultBullet, new ProjectileMovementData { ProjectileSpeed = 70f, ProjectileLifeTime = 7f });
                EntityManager.AddComponentData(defaultBullet, new ProjectileDamageData { DamageType = DamageType.Bullet, DamageData = 50, ProjectilePiercingCountData = 1 });
            }
            else if (EntityManager.GetComponentData<GunData>(_playerTurretEntity).GunName == GunName.PlasmaGun)
            {
                var defaultBullet = EntityManager.Instantiate(bulletFactoryData.PlasmaGunBulletObject);
                var gunEntityTransform = EntityManager.GetComponentData<LocalTransform>(_playerTurretEntity);

                EntityManager.SetComponentData<LocalTransform>(defaultBullet, new LocalTransform
                {
                    Position = gunEntityTransform.Position + new float3(0, 1.2f, 0),
                    Rotation = _mainCameraGunHoldTransform.transform.rotation,
                    Scale = 1f
                });

                EntityManager.AddComponentData(defaultBullet, new ProjectileMovementData { ProjectileSpeed = 20f, ProjectileLifeTime = 12f });
                EntityManager.AddComponentData(defaultBullet, new ProjectileDamageData { DamageType = DamageType.Explosive, DamageData = 100, ProjectilePiercingCountData = 100 });
            }

            _currentcooldownValue += _shootingCooldown;

        }
    }

    [BurstCompile]
    protected override void OnDestroy()
    {

    }
}