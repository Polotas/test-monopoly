using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turret : MonoBehaviour
{
    [Header("Configuration")]
    public TurretData turretData;

    [Header("Components")] 
    public Transform areaRange;
    public Transform firePoint;
    public Transform turretHead;
    
    // Runtime variables
    private CreepController currentTarget;
    private float lastFireTime;
    private List<CreepController> crepsInRange = new List<CreepController>();
    private Coroutine targetingCoroutine;
    
    public TurretData Data => turretData;
    public bool HasTarget => currentTarget != null;
    public float Range => turretData?.range ?? 0f;
    private BaseDefense baseDefense;
    
    private void Awake()
    {
        baseDefense = FindObjectOfType<BaseDefense>();
    }
    
    private void Start()
    {
        InitializeTurret();
        StartTargeting();
    }
    
    private void Update()
    {
        UpdateTargeting();
        RotateTowardsTarget();
        TryFire();
    }
    
    public void Initialize(TurretData data)
    {
        turretData = data;
        InitializeTurret();
    }
    
    public void UpdateRange(TurretData data) =>  areaRange.transform.localScale = new Vector3(data.range, data.range, data.range);
    
    private void InitializeTurret()
    {
        if (turretData == null) return;
        
        // Setup fire point if not assigned
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = Vector3.up * 0.5f;
            firePoint = firePointObj.transform;
        }
        
        // Setup turret head if not assigned
        if (turretHead == null)
        {
            turretHead = transform;
        }

        UpdateRange(turretData);
        lastFireTime = -turretData.fireRate; // Allow immediate first shot
    }
    
    private void StartTargeting()
    {
        if (targetingCoroutine != null)
            StopCoroutine(targetingCoroutine);
            
        targetingCoroutine = StartCoroutine(TargetingLoop());
    }
    
    private IEnumerator TargetingLoop()
    {
        while (true)
        {
            FindCreepsInRange();
            SelectBestTarget();
            yield return new WaitForSeconds(0.1f); // Update targeting 10 times per second
        }
    }
    
    private void FindCreepsInRange()
    {
        crepsInRange.Clear();
        
        CreepController[] allCreeps = FindObjectsOfType<CreepController>();
        foreach (CreepController creep in allCreeps)
        {
            if (creep == null || !creep.IsAlive) continue;
            var distance = Vector3.Distance(transform.position, creep.transform.position);
            if (distance <= turretData.range)
            {
                crepsInRange.Add(creep);
            }
        }
    }
    
    private void SelectBestTarget()
    {
        if (crepsInRange.Count == 0)
        {
            currentTarget = null;
            return;
        }
        
        CreepController bestTarget = null;
        float shortestDistanceToBase = float.MaxValue;

        Vector3 basePosition = baseDefense.GetBasePosition();
        
        foreach (CreepController creep in crepsInRange)
        {
            if (creep == null || !creep.IsAlive) continue;
            
            float distanceToBase = Vector3.Distance(creep.transform.position, basePosition);
            if (!(distanceToBase < shortestDistanceToBase)) continue;
            shortestDistanceToBase = distanceToBase;
            bestTarget = creep;
        }
        
        currentTarget = bestTarget;
    }
    
    private void UpdateTargeting()
    {
        if (currentTarget == null) return;
        if (!currentTarget.IsAlive || 
            Vector3.Distance(transform.position, currentTarget.transform.position) > turretData.range)
        {
            currentTarget = null;
        }
    }
    
    private void RotateTowardsTarget()
    {
        if (currentTarget == null || turretHead == null) return;
        
        Vector3 direction = (currentTarget.transform.position - turretHead.position).normalized;
        direction.y = 0; // Keep rotation on horizontal plane only
        
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            turretHead.rotation = Quaternion.Slerp(turretHead.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    
    private void TryFire()
    {
        if (currentTarget == null || turretData == null) return;
        
        float timeSinceLastFire = Time.time - lastFireTime;
        float fireInterval = 1f / turretData.fireRate;

        if (!(timeSinceLastFire >= fireInterval)) return;
        Fire();
        lastFireTime = Time.time;
    }
    
    private void Fire()
    {
        if (currentTarget == null || firePoint == null) return;
        
        if (turretData.projectilePrefab != null)
        {
            GameObject projectileObj = ObjectPooler.Instance.SpawnFromPool(turretData.projectilePrefab.name, firePoint.position, firePoint.rotation);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(turretData, currentTarget);
        }
        else
        {
            InstantHit();
        }
    }
    
    private void InstantHit()
    {
        if (currentTarget == null) return;
        
        currentTarget.TakeDamage(turretData.damage);
        
        if (turretData.hasSlowEffect)
        {
            currentTarget.ApplySlow(turretData.slowAmount, turretData.slowDuration);
        }
        
        Debug.Log($"Turret {turretData.turretName} hit {currentTarget.name} dealing {turretData.damage} damage");
    }
    
    private void OnDrawGizmosSelected()
    {
        if (turretData != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, turretData.range);
            areaRange.transform.localScale = new Vector3(turretData.range, turretData.range, turretData.range);
            if (currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, currentTarget.transform.position);
            }
        }
    }
    
    private void OnDestroy()
    {
        if (targetingCoroutine != null)
        {
            StopCoroutine(targetingCoroutine);
        }
    }
}
