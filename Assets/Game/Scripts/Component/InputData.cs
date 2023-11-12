using Unity.Entities;

public struct InputData  : IComponentData
{
    public float UpDownRange;
    public float RightLeftRange;
    public float MouseSensitivity;
}