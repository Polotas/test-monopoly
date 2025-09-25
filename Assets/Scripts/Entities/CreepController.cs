using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CreepController : MonoBehaviour
{
    private CreepModel model;
    private CreepView view;
    private Rigidbody rb;
    private BaseDefense baseDefense;

    private Vector3 targetPosition;
    private bool reachedBase = false;
    private Coroutine slowCoroutine;
    private Coroutine moveCoroutine;
    public System.Action<CreepController> OnDeath;

    public bool IsAlive => model.IsAlive;

    private void Awake()
    {
        baseDefense = FindObjectOfType<BaseDefense>();
        view = GetComponent<CreepView>();
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.freezeRotation = true;
    }
    
    public void Initialize(CreepData data)
    {
        model = new CreepModel(data);

        model.OnDeath += HandleDeath;
        model.OnDamaged += HandleDamaged;

        if (data.material != null)
            view.SetMaterial(data.material);

        gameObject.SetActive(true);
        targetPosition = baseDefense.transform.position;
        
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToBaseRoutine());
    }

    private IEnumerator MoveToBaseRoutine()
    {
        while (model.IsAlive && !reachedBase)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Vector3 movement = direction * model.CurrentMoveSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + movement);

            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);

            CheckIfReachedBase();
            yield return new WaitForFixedUpdate();
        }
    }

    private void CheckIfReachedBase()
    {
        if (!baseDefense.IsCreepAtBase(transform.position)) return;
        
        reachedBase = true;

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        baseDefense.TakeDamage(model.CurrentDamage);
        HandleDeath();
    }

    public void TakeDamage(int damage) =>  model.TakeDamage(damage);

    public void ApplySlow(float multiplier, float duration)
    {
        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);
        
        if(gameObject.activeSelf)
            slowCoroutine = StartCoroutine(SlowEffect(multiplier, duration));
    }

    private IEnumerator SlowEffect(float multiplier, float duration)
    {
        model.ApplySlow(multiplier);
        view.ShowSlowEffect();

        yield return new WaitForSeconds(duration);

        model.ResetSpeed();
        view.ResetColor();
        slowCoroutine = null;
    }

    private void HandleDamaged() =>  view.FlashDamage(model.CurrentHealth,model.Data.maxHealth);

    private void HandleDeath()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        if (model.Data != null && EconomySystem.Instance != null)
            EconomySystem.Instance.RewardCoins(model.Data.GetRandomCoinReward());

        OnDeath?.Invoke(this);
        gameObject.SetActive(false);
    }
}