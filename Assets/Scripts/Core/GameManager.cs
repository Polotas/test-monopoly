using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Configuration")]
    public GameConfig gameConfig;

    // Game State
    public int CurrentCoins { get; private set; }
    public int BaseHealth { get; private set; }
    public int CurrentWave { get; private set; }
    public GameState CurrentGameState { get; private set; }
    
    // Systems
    private WaveManager waveManager;
    private EconomySystem economySystem;
    private BaseDefense baseDefense;
    
    // Events
    public System.Action<int> OnCoinsChanged;
    public System.Action<int,int> OnBaseHealthChanged;
    public System.Action<int> OnWaveChanged;
    public System.Action<GameState> OnGameStateChanged;
    
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameWon,
        GameLost
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeSystems();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        StartNewGame();
    }
    
    private void InitializeSystems()
    {
        waveManager = FindObjectOfType<WaveManager>();
        economySystem = FindObjectOfType<EconomySystem>();          
        baseDefense = FindObjectOfType<BaseDefense>();
    }

    public void StartNewGame()
    {
        ResetGameState();
        ChangeGameState(GameState.Playing);
        
        waveManager.ResetWaves();
        // Initialize systems
        CurrentCoins = gameConfig.startingCoins;
        BaseHealth = gameConfig.baseMaxHealth;
        CurrentWave = 0;
        
        // Trigger UI updates
        OnCoinsChanged?.Invoke(CurrentCoins);
        OnBaseHealthChanged?.Invoke(BaseHealth,gameConfig.baseMaxHealth);
        OnWaveChanged?.Invoke(CurrentWave + 1);
        
        // Start first wave after delay
        StartCoroutine(StartFirstWave());
    }
    
    private IEnumerator StartFirstWave()
    {
        yield return new WaitForSeconds(gameConfig.timeBeforeFirstWave);
        waveManager.StartNextWave();
    }
    
    public void RestartGame()
    {
        ClearGameObjects();
        StartNewGame();
    }
    
    private void ClearGameObjects()
    {
        // Clear all creeps
        var creeps = FindObjectsOfType<CreepController>();
        foreach (var creep in creeps)
        {
            if (creep != null)
                creep.gameObject.SetActive(false);
        }
        
        // Clear all turrets
        var turrets = FindObjectsOfType<Turret>();
        foreach (var turret in turrets)
        {
            if (turret != null)
                turret.gameObject.SetActive(false);
        }
        
        // Clear all projectiles
        var projectiles = FindObjectsOfType<Projectile>();
        foreach (var projectile in projectiles)
        {
            if (projectile != null)
                projectile.gameObject.SetActive(false);
        }
    }
    
    private void ResetGameState()
    {
        CurrentCoins = 0;
        BaseHealth = 0;
        CurrentWave = 0;
        CurrentGameState = GameState.MainMenu;
    }
    
    public void ChangeGameState(GameState newState)
    {
        CurrentGameState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
    
    public bool SpendCoins(int amount)
    {
        if (CurrentCoins >= amount)
        {
            CurrentCoins -= amount;
            OnCoinsChanged?.Invoke(CurrentCoins);
            return true;
        }
        return false;
    }
    
    public void AddCoins(int amount)
    {
        CurrentCoins += amount;
        OnCoinsChanged?.Invoke(CurrentCoins);
    }
    
    public void DamageBase(int damage)
    {
        BaseHealth = Mathf.Max(0, BaseHealth - damage);
        OnBaseHealthChanged?.Invoke(BaseHealth,gameConfig.baseMaxHealth);
        
        if (BaseHealth <= 0)
        {
            GameLost();
        }
    }
    
    public void OnWaveCompleted()
    {
        CurrentWave++;
        OnWaveChanged?.Invoke(CurrentWave + 1);
        
        if (CurrentWave >= waveManager.waves.Count)
        {
            GameWon();
        }
        else
        {
            StartCoroutine(StartNextWaveAfterDelay());
        }
    }
    
    private IEnumerator StartNextWaveAfterDelay()
    {
        yield return new WaitForSeconds(gameConfig.timeBetweenWaves);
        if (CurrentGameState == GameState.Playing)
        {
            waveManager.StartNextWave();
        }
    }
    
    private void GameWon()
    {
        ChangeGameState(GameState.GameWon);
    }
    
    private void GameLost()
    {
        ChangeGameState(GameState.GameLost);
    }
    
    private void HandleGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                break;
            case GameState.GameWon:
            case GameState.GameLost:
                Time.timeScale = 0f;
                break;
        }
    }
}