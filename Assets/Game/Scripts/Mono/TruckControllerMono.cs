using Unity.Entities;
using UnityEngine;


public class TruckControllerMono  : MonoBehaviour
{

}

public class TruckControllerMonoBaker  : Baker<TruckControllerMono>
{
    public override void Bake(TruckControllerMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new TruckGrinderData() { TruckGrinderPosValue = new Unity.Mathematics.float3(0,0,57.5f)});
    }
}