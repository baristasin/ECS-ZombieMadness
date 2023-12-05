using System;
using Unity.Entities;
using Unity.Mathematics;

public struct ZombiePositionData : IComponentData, IEnableableComponent
{
    public float3 ZombiePosition;

}