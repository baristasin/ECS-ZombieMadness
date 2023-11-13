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
        }

        if (_playerGunEntity == Entity.Null) return;

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

        if (Input.GetMouseButtonDown(0))
        {
            var defaultBullet = EntityManager.Instantiate(bulletFactoryData.DefaultBulletObject);
            EntityManager.SetComponentData<LocalTransform>(defaultBullet, new LocalTransform
            {
                Position = _mainCameraGunHoldTransform.transform.position + _mainCameraGunHoldTransform.transform.forward * 2f,
                Rotation = _mainCameraGunHoldTransform.transform.rotation,
                Scale = 1f
            });

            EntityManager.AddComponentData(defaultBullet, new ProjectileMovementData { ProjectileSpeed = 30f });
            EntityManager.AddComponentData(defaultBullet, new ProjectileDamageData { DamageData = 25 });
        }
    }

    [BurstCompile]
    protected override void OnDestroy()
    {

    }

}