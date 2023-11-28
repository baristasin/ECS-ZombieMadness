using Unity.Entities;
using UnityEngine;


public class RotationGunEntityMono  : MonoBehaviour
{

}

public class RotationGunEntityMonoBaker  : Baker<RotationGunEntityMono>
{
    public override void Bake(RotationGunEntityMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
    }
}