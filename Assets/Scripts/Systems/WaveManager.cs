using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    public List<WaveData> waves;
    public List<Transform> spawnPoints;
    
    private GameManager gameManager;
    public int currentWaveIndex = 0;
    public bool waveInProgress = false;
    private List<CreepController> activeCreeps = new List<CreepController>();
    private int creepsSpawnedInCurrentWave = 0;
    private int totalCreepsInCurrentWave = 0;
    private List<Transform> shuffledSpawnPoints;
    private int currentSpawnIndex = 0;
    
    public bool IsWaveInProgress => waveInProgress;
    public int CurrentWaveIndex => currentWaveIndex;
    public int TotalWaves => waves.Count;
    public int ActiveCreepCount => activeCreeps.Count;
    
    public System.Action<int> OnWaveStarted;
    public System.Action<int> OnWaveCompleted;
    public System.Action OnAllWavesCompleted;
    
    private void Start()
    {
        gameManager = GameManager.Instance;
        FindSpawnPoints();
    }
    
    private void FindSpawnPoints()
    {
		spawnPoints = new List<Transform>();

        if (spawnPoints.Count == 0)
        {
            // Find all spawn points in the scene
            GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
            foreach (var spawnPoint in spawnPointObjects)
            {
                spawnPoints.Add(spawnPoint.transform);
            }
            
            // If no tagged spawn points found, look by name
            if (spawnPoints.Count == 0)
            {
                var allObjects = FindObjectsOfType<Transform>();
                foreach (var obj in allObjects)
                {
                    if (!obj.name.ToLower().Contains("spawnpoint") && !obj.name.ToLower().Contains("spawn")) continue;
                    spawnPoints.Add(obj);
                        
                    if (!obj.CompareTag("SpawnPoint"))
                    {
                        obj.tag = "SpawnPoint";
                    }
                }
            }
        }
        
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points found! Add objects with 'SpawnPoint' tag or 'SpawnPoint' in the name.");
        }
        else
        {
            Debug.Log($"Found {spawnPoints.Count} spawn points.");
        }
        
        shuffledSpawnPoints = new List<Transform>(spawnPoints);
        Shuffle(shuffledSpawnPoints);
    }
    
    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
    
    public void StartNextWave()
    {
        if (waveInProgress || currentWaveIndex >= waves.Count)
          return;

        WaveData currentWave = waves[currentWaveIndex];
        StartCoroutine(SpawnWave(currentWave));
    }
    
    private IEnumerator SpawnWave(WaveData waveData)
    {
        waveInProgress = true;
        creepsSpawnedInCurrentWave = 0;
        totalCreepsInCurrentWave = waveData.GetTotalCreepCount();
        
        OnWaveStarted?.Invoke(currentWaveIndex + 1);
        Debug.Log($"Starting wave {currentWaveIndex + 1} with {totalCreepsInCurrentWave} creeps");
        
        foreach (var spawnInfo in waveData.creepsToSpawn)
        {
            if (spawnInfo.delayBeforeSpawn > 0)
            {
                yield return new WaitForSeconds(spawnInfo.delayBeforeSpawn);
            }
            
            for (int i = 0; i < spawnInfo.count; i++)
            {
                if (gameManager.CurrentGameState != GameManager.GameState.Playing)
                    yield break;
                    
                SpawnCreep(spawnInfo.creepData);
                creepsSpawnedInCurrentWave++;
                
                if (i < spawnInfo.count - 1) // Don't wait after the last creep of this type
                {
                    yield return new WaitForSeconds(waveData.timeBetweenSpawns);
                }
            }
        }
        
        Debug.Log($"All {totalCreepsInCurrentWave} creeps from wave {currentWaveIndex + 1} have been spawned");
        
        yield return StartCoroutine(WaitForWaveCompletion());
    }
    
    private IEnumerator WaitForWaveCompletion()
    {
        while (activeCreeps.Count > 0 && gameManager.CurrentGameState == GameManager.GameState.Playing)
        {
            // Remove null references (destroyed creeps)
            activeCreeps.RemoveAll(creep => creep == null);
            yield return new WaitForSeconds(0.1f);
        }
        
        CompleteCurrentWave();
    }
    
    private void CompleteCurrentWave()
    {
        waveInProgress = false;
        OnWaveCompleted?.Invoke(currentWaveIndex + 1);
        Debug.Log($"Wave {currentWaveIndex + 1} completed!");
        
        currentWaveIndex++;
        
        if (currentWaveIndex >= waves.Count)
        {
            OnAllWavesCompleted?.Invoke();
            gameManager?.OnWaveCompleted(); // This will trigger game won
        }
        else
        {
            gameManager?.OnWaveCompleted(); // This will start the next wave after delay
        }
    }
    
    private void SpawnCreep(CreepData creepData)
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("Cannot spawn creep: spawn points or prefab not configured");
            return;
        }
        
        if (currentSpawnIndex >= shuffledSpawnPoints.Count)
        {
            Shuffle(shuffledSpawnPoints);
            currentSpawnIndex = 0;
        }

        Transform spawnPoint = shuffledSpawnPoints[currentSpawnIndex];
        currentSpawnIndex++;
        
        GameObject creepObject =
            ObjectPooler.Instance.SpawnFromPool(creepData.prefab.name, spawnPoint.position, Quaternion.identity);
        CreepController creep = creepObject.GetComponent<CreepController>();
        
        creep.Initialize(creepData);
        creep.OnDeath += OnCreepDeath;
        activeCreeps.Add(creep);
        
        Debug.Log($"Creep {creepData.creepName} spawned at {spawnPoint.name}");
    }
    
    private void OnCreepDeath(CreepController creep)
    {
        activeCreeps.Remove(creep);
        creep.OnDeath -= OnCreepDeath;
    }

    public void ResetWaves()
    {
        StopAllCoroutines();
        
        foreach (var creep in activeCreeps)
        {
            if (creep != null)
            {
                creep.OnDeath -= OnCreepDeath;
            }
        }
        activeCreeps.Clear();
        
        currentWaveIndex = 0;
        waveInProgress = false;
        creepsSpawnedInCurrentWave = 0;
        totalCreepsInCurrentWave = 0;
    }
    
    public WaveData GetCurrentWave() => currentWaveIndex < waves.Count ? waves[currentWaveIndex] : null;

    public WaveData GetWave(int index)
    {
        if (index >= 0 && index < waves.Count)
            return waves[index];
        return null;
    }
}
