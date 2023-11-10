using Unity.Entities;

public struct ZombieWaveBufferElement  : IBufferElementData
{
    public int WaveTotalZombieCount;
    public int WaveSpawnLineCount;
    public int IsRandomLocationSpawn;
    public float WaitAfterSeconds;
}