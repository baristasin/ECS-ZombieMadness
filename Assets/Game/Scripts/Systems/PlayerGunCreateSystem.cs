using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial class PlayerGunCreateSystem : SystemBase
{
    private Camera _mainCamera;
    private Transform _mainCameraGunHoldTransform;
    private Entity _playerGunEntity;

    private float _shootingCooldown;
    private float _currentcooldownValue;

    [BurstCompile]
    protected override void OnCreate()
    {
        _mainCamera = Camera.main;
        _mainCameraGunHoldTransform = _mainCamera.transform.GetChild(0).transform;
        RequireForUpdate<GunFactoryData>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        var gunFactoryEntitySingleton = SystemAPI.GetSingletonEntity<GunFactoryData>();
        var gunFactoryData = SystemAPI.GetComponent<GunFactoryData>(gunFactoryEntitySingleton);

        var bulletFactoryData = SystemAPI.GetComponent<BulletFactoryData>(gunFactoryEntitySingleton);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_playerGunEntity != Entity.Null)
            {
                EntityManager.DestroyEntity(_playerGunEntity);
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

            _playerGunEntity = minigunEntity;

            _shootingCooldown = gunFactoryData.MinigunShootingInterval;
            _currentcooldownValue = _shootingCooldown;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_playerGunEntity != Entity.Null)
            {
                EntityManager.DestroyEntity(_playerGunEntity);
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

            _playerGunEntity = plasmaGunEntity;

            _shootingCooldown = gunFactoryData.PlasmaGunShootingInterval;
            _currentcooldownValue = _shootingCooldown;
        }

        if (!EntityManager.Exists(_playerGunEntity)) return;

        var deltaTime = SystemAPI.Time.DeltaTime;

        var newPosition = EntityManager.GetComponentData<LocalTransform>(_playerGunEntity).Position;
        newPosition = math.lerp(EntityManager.GetComponentData<LocalTransform>(_playerGunEntity).Position, _mainCameraGunHoldTransform.transform.position, deltaTime * 4f);

        EntityManager.SetComponentData<LocalTransform>(_playerGunEntity, new LocalTransform
        {
            Position = newPosition,
            //Position = _mainCameraGunHoldTransform.transform.position,
            Rotation = _mainCameraGunHoldTransform.transform.rotation,
            Scale = 1f
        });

        if(_currentcooldownValue > 0)
        {
            _currentcooldownValue -= deltaTime;
        }
        else
        {
            if(EntityManager.GetComponentData<GunData>(_playerGunEntity).GunName == GunName.Minigun)
            {

                var defaultBullet = EntityManager.Instantiate(bulletFactoryData.DefaultBulletObject);
                EntityManager.SetComponentData<LocalTransform>(defaultBullet, new LocalTransform
                {
                    Position = _mainCameraGunHoldTransform.transform.position + _mainCameraGunHoldTransform.transform.forward * 2f,
                    Rotation = _mainCameraGunHoldTransform.transform.rotation,
                    Scale = 1f
                });

                EntityManager.AddComponentData(defaultBullet, new ProjectileMovementData { ProjectileSpeed = 45f });
                EntityManager.AddComponentData(defaultBullet, new ProjectileDamageData { DamageData = 100, ProjectilePiercingCountData = 1 });
            }
            else if (EntityManager.GetComponentData<GunData>(_playerGunEntity).GunName == GunName.PlasmaGun)
            {
                var defaultBullet = EntityManager.Instantiate(bulletFactoryData.PlasmaGunBulletObject);
                EntityManager.SetComponentData<LocalTransform>(defaultBullet, new LocalTransform
                {
                    Position = _mainCameraGunHoldTransform.transform.position + _mainCameraGunHoldTransform.transform.forward * 2f,
                    Rotation = _mainCameraGunHoldTransform.transform.rotation,
                    Scale = 1f
                });

                EntityManager.AddComponentData(defaultBullet, new ProjectileMovementData { ProjectileSpeed = 10f });
                EntityManager.AddComponentData(defaultBullet, new ProjectileDamageData { DamageData = 100,ProjectilePiercingCountData = 100 });
            }

            _currentcooldownValue += _shootingCooldown;

        }
    }

    [BurstCompile]
    protected override void OnDestroy()
    {

    }
}