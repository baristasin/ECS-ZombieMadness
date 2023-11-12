using Unity.Entities;
using UnityEngine;


public class InputControllerMono : MonoBehaviour
{
    public float UpDownRange = 45f;
    public float RightLeftRange = 150f;
    public float MouseSensitivity = 1f;
}

public class InputControllerMonoBaker : Baker<InputControllerMono>
{
    public override void Bake(InputControllerMono authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent(entity, new InputData
        {
            MouseSensitivity = authoring.MouseSensitivity,
            UpDownRange = authoring.UpDownRange,
            RightLeftRange = authoring.RightLeftRange
        });
    }
}