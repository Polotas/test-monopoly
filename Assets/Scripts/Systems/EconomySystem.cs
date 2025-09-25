using UnityEngine;

public class EconomySystem : MonoBehaviour
{
    public static EconomySystem Instance { get; private set; }
    
    private GameManager gameManager;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    
    public bool CanAfford(int cost)
    {
        return gameManager.CurrentCoins >= cost;
    }
    
    public bool TryPurchase(int cost)
    {
        if (CanAfford(cost))
        {
            return gameManager.SpendCoins(cost);
        }
        return false;
    }
    
    public void RewardCoins(int amount)
    {
        gameManager.AddCoins(amount);
    }
    
    public int GetCurrentCoins()
    {
        return gameManager.CurrentCoins;
    }
}
