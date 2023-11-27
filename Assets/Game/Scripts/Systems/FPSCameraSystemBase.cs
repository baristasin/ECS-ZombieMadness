using Unity.Burst;
using Unity.Entities;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial class FPSCameraSystemBase : SystemBase
{
    private Transform _gameCamera;

    private float _rotationX;
    private float _rotationY;

    private float _firstCamY;
    private float _firstCamX;

    private InputData _inputData;

    private bool _isInitialized;

    [BurstCompile]
    protected override void OnCreate()
    {
        RequireForUpdate<InputData>();
        RequireForUpdate<ZombieSpawnData>();
        _gameCamera = Camera.main.GetComponent<CameraBehaviour>().CamTransform;
        _firstCamY = _gameCamera.transform.localEulerAngles.x;
        _firstCamX = _gameCamera.transform.localEulerAngles.y;
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        if (_gameCamera == null) Enabled = false;

        if (Input.GetKeyDown("space"))
        {
            Enabled = false;
        }

        if (!_isInitialized)
        {
            var inputDataSingleton = SystemAPI.GetSingletonEntity<InputData>();
            var inputData = EntityManager.GetComponentData<InputData>(inputDataSingleton);


            var zombieSpawnControllerAspectSingleton = SystemAPI.GetSingletonEntity<ZombieSpawnData>();
            var zombieSpawnControllerAspect = SystemAPI.GetAspect<ZombieSpawnControllerAspect>(zombieSpawnControllerAspectSingleton);

            Camera.main.GetComponent<CameraBehaviour>().CamSpeed = zombieSpawnControllerAspect.ZombieSpawnData.ValueRO.ZombieMinSpeed - .2f;

            _inputData = inputData;
            _isInitialized = true;
        }

        _rotationX += Input.GetAxis("Mouse X") * _inputData.MouseSensitivity;
        _rotationY -= Input.GetAxis("Mouse Y") * _inputData.MouseSensitivity;

        _rotationY = Mathf.Clamp(_rotationY, -_inputData.UpDownRange, _inputData.UpDownRange);
        _rotationX = Mathf.Clamp(_rotationX, -_inputData.RightLeftRange, _inputData.RightLeftRange);
        _gameCamera.transform.localRotation = Quaternion.Euler(_rotationY + _firstCamY, _rotationX + _firstCamX, 0);

    }

    [BurstCompile]
    protected override void OnDestroy()
    {

    }

}