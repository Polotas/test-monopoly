using UnityEngine;

[System.Serializable]
public class CreepModel 
{
    public CreepData Data { get; private set; }
    public int CurrentDamage { get; private set; }
    public int CurrentHealth { get; private set; }
    public float CurrentMoveSpeed { get; private set; }
    public bool IsAlive => CurrentHealth > 0;

    public System.Action OnDeath;
    public System.Action OnDamaged;

    public CreepModel(CreepData data)
    {
        Data = data;
        CurrentHealth = data.maxHealth;
        CurrentMoveSpeed = data.moveSpeed;
        CurrentDamage = data.damage;
    }

    public void TakeDamage(int amount)
    {
        if (!IsAlive) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        OnDamaged?.Invoke();

        if (CurrentHealth <= 0)
            OnDeath?.Invoke();
    }

    public void ApplySlow(float multiplier) => CurrentMoveSpeed = Data.moveSpeed * multiplier;
    public void ResetSpeed() => CurrentMoveSpeed = Data.moveSpeed;
}