using UnityEngine;

public class BaseDefense : MonoBehaviour
{
    [Header("Base Settings")]
    public Transform baseTransform;
    public float baseRadius = 2f;
    
    private GameManager gameManager;
    private int maxHealth;
    private int currentHealth;
    
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public float HealthPercentage => maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
    
    private void Start()
    {
        gameManager = GameManager.Instance;
        
        if (baseTransform == null)
            baseTransform = GameObject.FindWithTag("Base")?.transform;
            
        if (baseTransform == null)
        {
            Debug.LogError("Base transform not found! Make sure the Base object has the 'Base' tag.");
        }
        
        InitializeBase();
    }
    
    private void InitializeBase()
    {
        if (gameManager?.gameConfig != null)
        {
            maxHealth = gameManager.gameConfig.baseMaxHealth;
            currentHealth = maxHealth;
        }
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        gameManager?.DamageBase(damage);
        
        Debug.Log($"Base took {damage} damage. Health remaining: {currentHealth}/{maxHealth}");
    }
    
    public bool IsCreepAtBase(Vector3 creepPosition)
    {
        if (baseTransform == null) return false;
        
        float distance = Vector3.Distance(creepPosition, baseTransform.position);
        return distance <= baseRadius;
    }
    
    public Vector3 GetBasePosition()
    {
        return baseTransform != null ? baseTransform.position : Vector3.zero;
    }
    
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (baseTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(baseTransform.position, baseRadius);
        }
    }
}
