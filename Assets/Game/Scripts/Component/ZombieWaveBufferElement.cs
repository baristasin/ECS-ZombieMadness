using Unity.Entities;

public struct ZombieWaveBufferElement  : IBufferElementData
{
    public int WaveTotalZombieCount;
    public int WaveSpawnLineCount;
    public float WaitAfterSeconds;
    public int IsBossWave;
}