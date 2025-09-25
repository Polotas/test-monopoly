using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Tower Defense/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Base Settings")]
    public int baseMaxHealth = 10;
    
    [Header("Economy Settings")]
    public int startingCoins = 20;
    public int turretCost = 5;
    public int coinsPerCreepKill = 1;
    
    [Header("Wave Settings")]
    public float timeBetweenWaves = 5f;
    public float timeBeforeFirstWave = 3f;
    
    [Header("UI Settings")]
    public float popupDisplayTime = 3f;
}
