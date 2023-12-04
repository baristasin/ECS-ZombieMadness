using System;
using Unity.Entities;
using Unity.Mathematics;

public struct ZombiePositionData : IComponentData, IEnableableComponent, IComparable<ZombiePositionData>
{
    public float2 ZombiePosition;

    public int CompareTo(ZombiePositionData other)
    {
        if(ZombiePosition.y > other.ZombiePosition.y)
        {
            return (int)ZombiePosition.y;
        }
        else
        {
            return (int)other.ZombiePosition.y;
        }
    }

}