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
    private float3 _playerMuzzleLocalPosition;

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
            _playerMuzzleLocalPosition = gunFactoryData.MinigunMuzzleLocalPosition;
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

            var rocketLauncherEntity = EntityManager.Instantiate(gunFactoryData.RocketLauncherEntity);
            _playerMuzzleLocalPosition = gunFactoryData.RocketLauncherMuzzleLocalPosition;
            EntityManager.AddComponent<LocalTransform>(rocketLauncherEntity);
            EntityManager.SetComponentData<LocalTransform>(rocketLauncherEntity, new LocalTransform
            {
                Position = _mainCameraGunHoldTransform.transform.position,
                Rotation = _mainCameraGunHoldTransform.transform.rotation,
                Scale = 1f
            });

            EntityManager.AddComponentData(rocketLauncherEntity, new GunData
            {
                GunName = GunName.RocketLauncher,
                GunShootingInterval = gunFactoryData.RocketLauncherShootingInterval
            });

            _playerTurretEntity = rocketLauncherEntity;
            var buffer = EntityManager.GetBuffer<LinkedEntityGroup>(rocketLauncherEntity);

            _playerGunEntity = buffer[1].Value;

            _shootingCooldown = gunFactoryData.RocketLauncherShootingInterval;
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
                var playerTurretTransform = EntityManager.GetComponentData<LocalTransform>(_playerTurretEntity);


                var playerGunTransform = EntityManager.GetComponentData<LocalTransform>(_playerGunEntity);


                EntityManager.SetComponentData<LocalTransform>(defaultBullet, new LocalTransform
                {
                    Position = playerTurretTransform.Position,
                    Rotation = playerGunTransform.Rotation,
                    Scale = 1f
                });

                EntityManager.AddComponentData(defaultBullet, new ProjectileMovementData { ProjectileSpeed = 60f, ProjectileLifeTime = 7f });
                EntityManager.AddComponentData(defaultBullet, new ProjectileDamageData { DamageType = DamageType.Bullet, DamageData = 25, ProjectilePiercingCountData = 1,BulletEffectEntity = bulletFactoryData.BulletEffectEntity });
            }
            else if (EntityManager.GetComponentData<GunData>(_playerTurretEntity).GunName == GunName.RocketLauncher)
            {
                var defaultBullet = EntityManager.Instantiate(bulletFactoryData.RocketLauncherBulletObject);
                var playerTurretTransform = EntityManager.GetComponentData<LocalTransform>(_playerTurretEntity);

                var playerGunTransform = EntityManager.GetComponentData<LocalTransform>(_playerGunEntity);


                EntityManager.SetComponentData<LocalTransform>(defaultBullet, new LocalTransform
                {
                    Position = playerTurretTransform.Position,
                    Rotation = playerGunTransform.Rotation,
                    Scale = 1f
                });

                EntityManager.AddComponentData(defaultBullet, new ProjectileMovementData { ProjectileSpeed = 30f, ProjectileLifeTime = 12f });
                EntityManager.AddComponentData(defaultBullet, new ProjectileDamageData { DamageType = DamageType.Explosive, DamageData = 100, ProjectilePiercingCountData = 1,ExplosiveEntity = bulletFactoryData.ExplosionPropObject,ExplosiveEffectEntity = bulletFactoryData.ExplosiveEffectEntity});
            }

            _currentcooldownValue += _shootingCooldown;

        }
    }

    [BurstCompile]
    protected override void OnDestroy()
    {

    }
}