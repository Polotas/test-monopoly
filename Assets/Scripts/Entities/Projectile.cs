using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Configuration")]
    private TurretData turretData;
    private CreepController target;
    private Vector3 targetPosition;
    
    [Header("Runtime")]
    private float speed;
    private int damage;
    private bool hasSlowEffect;
    private float slowAmount;
    private float slowDuration;
    private float lifeTime = 3f; 
    
    private Rigidbody rb;
    private bool hasHit = false;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        
        rb.useGravity = false;
        rb.freezeRotation = true;
    }
    
    private void Update()
    {
        if (hasHit) return;
        MoveTowardsTarget();
    }
    
    public void Initialize(TurretData data, CreepController targetCreep)
    {
        turretData = data;
        target = targetCreep;
        Invoke("DisableGameObject",lifeTime);
        
        if (data != null)
        {
            speed = data.projectileSpeed;
            damage = data.damage;
            hasSlowEffect = data.hasSlowEffect;
            slowAmount = data.slowAmount;
            slowDuration = data.slowDuration;
        }
        
        if (target != null)
        {
            targetPosition = target.transform.position;
        }
        else
        {
            targetPosition = transform.position + transform.forward * 10f;
        }
    }
    
    private void DisableGameObject() => gameObject.SetActive(false);
    
    private void MoveTowardsTarget()
    {
        if (target != null && target.IsAlive)
        {
            targetPosition = target.transform.position;
        }
        
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 movement = direction * speed * Time.deltaTime;
        
        rb.MovePosition(transform.position + movement);
        
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget < 0.5f)
        {
            HitTarget();
        }
    }
    
    private void HitTarget()
    {
        if (hasHit) return;
        hasHit = true;
        
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);
        
        foreach (Collider hitCollider in hitColliders)
        {
            CreepController creep = hitCollider.GetComponent<CreepController>();
            if (creep == null || !creep.IsAlive) continue;
            creep.TakeDamage(damage);
                
            if (hasSlowEffect)
            {
                creep.ApplySlow(slowAmount, slowDuration);
            }
                
            break;
        }
        
        gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        
        CreepController creep = other.GetComponent<CreepController>();
        if (creep != null && creep.IsAlive)
        {
            HitTarget();
        }
    }
}
