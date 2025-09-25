using UnityEngine;

[CreateAssetMenu(fileName = "TurretData", menuName = "Tower Defense/Turret Data")]
public class TurretData : ScriptableObject
{
    [Header("Basic Info")]
    public string turretName;
    public int cost = 5;
    public GameObject prefab;
    
    [Header("Combat Stats")]
    public int damage = 1;
    public float range = 5f;
    public float fireRate = 1f; // Shots per second
    public float projectileSpeed = 10f;
    
    [Header("Special Effects")]
    public bool hasSlowEffect = false;
    public float slowAmount = 0.5f; // Multiplier (0.5 = 50% speed)
    public float slowDuration = 2f;
    
    [Header("Projectile")]
    public GameObject projectilePrefab;
}
