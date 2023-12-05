using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public struct TurretDesiredOrientationData  : IComponentData
{
    public float TurretRotationSpeed;
    public Quaternion TurretDesiredQuaternionValue;
}