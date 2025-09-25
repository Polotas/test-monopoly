using UnityEngine;

[CreateAssetMenu(fileName = "CreepData", menuName = "Tower Defense/Creep Data")]
public class CreepData : ScriptableObject
{
    [Header("Basic Stats")]
    public string creepName;
    public int damage = 1;
    public int maxHealth = 3;
    public float moveSpeed = 2f;
    
    
    [Header("Rewards")]
    private int minCoinReward = 1;
    [Range(1,30)]
    public int maxCoinReward = 10;
    
    [Header("Visual")]
    public GameObject prefab;
    public Material material;
    
    public int GetRandomCoinReward() => Random.Range(minCoinReward, maxCoinReward + 1);
}
