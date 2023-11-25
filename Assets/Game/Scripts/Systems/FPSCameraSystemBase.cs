using Unity.Burst;
using Unity.Entities;
using UnityEngine;

//[DisableAutoCreation]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial class FPSCameraSystemBase : SystemBase
{
    private Camera _mainCamera;

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

        _mainCamera = Camera.main;

        _firstCamY = _mainCamera.transform.localEulerAngles.x;
        _firstCamX = _mainCamera.transform.localEulerAngles.y;
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        if (!_isInitialized)
        {
            var inputDataSingleton = SystemAPI.GetSingletonEntity<InputData>();
            var inputData = EntityManager.GetComponentData<InputData>(inputDataSingleton);

            _inputData = inputData;
            _isInitialized = true;
        }

        _rotationX += Input.GetAxis("Mouse X") * _inputData.MouseSensitivity;
        _rotationY -= Input.GetAxis("Mouse Y") * _inputData.MouseSensitivity;

        _rotationY = Mathf.Clamp(_rotationY, -_inputData.UpDownRange, _inputData.UpDownRange);
        _rotationX = Mathf.Clamp(_rotationX, -_inputData.RightLeftRange, _inputData.RightLeftRange);
        _mainCamera.transform.localRotation = Quaternion.Euler(_rotationY + _firstCamY, _rotationX + _firstCamX, 0);

    }

    [BurstCompile]
    protected override void OnDestroy()
    {

    }

}