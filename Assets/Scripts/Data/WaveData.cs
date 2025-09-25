using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WaveData", menuName = "Tower Defense/Wave Data")]
public class WaveData : ScriptableObject
{
    [Header("Wave Settings")]
    public int waveNumber;
    public float timeBetweenSpawns = 1f;
    public List<CreepSpawnInfo> creepsToSpawn = new List<CreepSpawnInfo>();
    
    [System.Serializable]
    public class CreepSpawnInfo
    {
        public CreepData creepData;
        public int count;
        public float delayBeforeSpawn = 0f;
    }
    
    public int GetTotalCreepCount()
    {
        int total = 0;
        foreach (var spawn in creepsToSpawn)
        {
            total += spawn.count;
        }
        return total;
    }
}
