using Unity.Entities;
using UnityEngine;

public struct TurretDesiredOrientationData  : IComponentData
{
    public float TurretRotationSpeed;
    public Quaternion TurretDesiredQuaternionValue;
}