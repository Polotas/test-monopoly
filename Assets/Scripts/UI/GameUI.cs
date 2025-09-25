using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameUI : MonoBehaviour
{
    [Header("HUD Elements")]
    public Text coinsText;
    public Text waveText;
    public Text baseHealthText;
    public Text gameStatusText;
    public RectTransform baseHealthReact;
    
    [Header("Turret Selection")]
    public Transform turretButtonsParent;
    public GameObject turretButtonPrefab;
    
    [Header("Game Control")]
    public Button[] restartButton;

    [Header("Popups")]
    public GameObject gameUI;
    public GameObject winPopup;
    public GameObject losePopup;

    [Header("Info Panel")]
    public GameObject infoPanel;
    public Text selectedTurretInfo;

    private GameManager gameManager;
    private TurretPlacementSystem placementSystem;
    private readonly List<Button> turretButtons = new List<Button>();

    private void Start()
    {
        InitializeUI();
        SetupEventListeners();
        CreateTurretButtons();
    }
    
    private void InitializeUI()
    {
        gameManager = GameManager.Instance;
        placementSystem = TurretPlacementSystem.Instance;
        
        if (gameUI != null) gameUI.SetActive(true);
        if (winPopup != null) winPopup.SetActive(false);
        if (losePopup != null) losePopup.SetActive(false);
        if (infoPanel != null) infoPanel.SetActive(false);
    }
    
    private void SetupEventListeners()
    {
        if (gameManager != null)
        {
            gameManager.OnCoinsChanged += UpdateCoinsDisplay;
            gameManager.OnBaseHealthChanged += UpdateBaseHealthDisplay;
            gameManager.OnWaveChanged += UpdateWaveDisplay;
            gameManager.OnGameStateChanged += HandleGameStateChanged;
        }
        
        if (placementSystem != null)
        {
            placementSystem.OnTurretTypeSelected += ShowTurretInfo;
            placementSystem.OnPlacementModeExited += HideTurretInfo;
        }
        
        foreach (var t in restartButton)
            t.onClick.AddListener(RestartGame);
    }

    private void CreateTurretButtons()
    {
        if (placementSystem == null || turretButtonsParent == null || turretButtonPrefab == null)
            return;
        
        var availableTurrets = placementSystem.GetAvailableTurrets();
        
        for (int i = 0; i < availableTurrets.Count; i++)
        {
            TurretData turretData = availableTurrets[i];
            if (turretData == null) continue;
            
            GameObject buttonObj = Instantiate(turretButtonPrefab, turretButtonsParent);
            Button button = buttonObj.GetComponent<Button>();

            if (button == null) continue;
            
            Text buttonText = button.GetComponentInChildren<Text>();
            
            if (buttonText != null)
            {
                buttonText.text = $"{turretData.turretName} \n \n \n {turretData.cost} coins";
            }
                
            TurretData capturedData = turretData; 
            button.onClick.AddListener(() => SelectTurret(capturedData));
                
            turretButtons.Add(button);
        }
    }
    
    private void SelectTurret(TurretData turretData)
    {
        if (placementSystem != null && EconomySystem.Instance.CanAfford(turretData.cost))
        {
            placementSystem.SelectTurretType(turretData);
        }
        else if (!EconomySystem.Instance.CanAfford(turretData.cost))
        {
            ShowMessage("Not enough coins!", 2f);
        }
    }
    
    private void UpdateCoinsDisplay(int coins)
    {
        if (coinsText != null)
            coinsText.text = "Coins: " + coins;
        
        UpdateTurretButtonStates();
    }
    
    private void UpdateWaveDisplay(int wave)
    {
        if (waveText == null) return;
            waveText.text = "Wave: " + wave;
    }
    
    private void UpdateBaseHealthDisplay(int currentHp,int maxHealth)
    {
        if (baseHealthText == null) return;
        
        baseHealthText.text = currentHp + "/" + maxHealth;
        var current = (float)currentHp / (float)maxHealth;
        baseHealthReact.transform.localScale = new Vector3(current, 1, 1);
    }

    private void UpdateTurretButtonStates()
    {
        if (placementSystem == null) return;
        
        var availableTurrets = placementSystem.GetAvailableTurrets();
        
        for (int i = 0; i < turretButtons.Count && i < availableTurrets.Count; i++)
        {
            Button button = turretButtons[i];
            TurretData turretData = availableTurrets[i];

            if (button == null || turretData == null) continue;
            
            bool canAfford = EconomySystem.Instance.CanAfford(turretData.cost);
            button.interactable = canAfford;
                
            var colors = button.colors;
            colors.normalColor = canAfford ? Color.white : Color.gray;
            button.colors = colors;
        }
    }
    
    private void ShowTurretInfo(TurretData turretData)
    {
        if (infoPanel == null) return;
        
        infoPanel.SetActive(true);

        if (selectedTurretInfo == null) return;
        
        string info = $"Turret: {turretData.turretName} \n" +
                      $"Cost: {turretData.cost} coins \n" +
                      $"Damage: {turretData.damage} \n" +
                      $"Range: {turretData.range:F1} \n" +
                      $"Fire Rate: {turretData.fireRate:F1}/s";
                             
        if (turretData.hasSlowEffect)
        {
            info += $" \nEffect: Slow {(1f - turretData.slowAmount) * 100f}%";
        }
                
        selectedTurretInfo.text = info;
    }
    
    private void HideTurretInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }
    
    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        if (gameUI != null)
            gameUI.SetActive(false);
        
        switch (newState)
        {
            case GameManager.GameState.GameWon:
                if (winPopup != null)
                    winPopup.SetActive(true);
                break;
            case GameManager.GameState.GameLost:
                if (losePopup != null)
                    losePopup.SetActive(true);
                break;
            case GameManager.GameState.Playing:
                if (gameUI != null)
                    gameUI.SetActive(true);
                break;
        }
    }
    
    private void RestartGame()
    {
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
        
        if (winPopup != null) winPopup.SetActive(false);
        if (losePopup != null) losePopup.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
    }

    public void ShowMessage(string message, float duration = 3f)
    {
        if (gameStatusText == null) return;
        
        gameStatusText.text = message;
        gameStatusText.gameObject.SetActive(true);
            
        Invoke(nameof(HideMessage), duration);
    }
    
    private void HideMessage()
    {
        if (gameStatusText != null)
        {
            gameStatusText.gameObject.SetActive(false);
        }
    }
    
    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnCoinsChanged -= UpdateCoinsDisplay;
            gameManager.OnBaseHealthChanged -= UpdateBaseHealthDisplay;
            gameManager.OnWaveChanged -= UpdateWaveDisplay;
            gameManager.OnGameStateChanged -= HandleGameStateChanged;
        }

        if (placementSystem == null) return;
        
        placementSystem.OnTurretTypeSelected -= ShowTurretInfo;
        placementSystem.OnPlacementModeExited -= HideTurretInfo;
    }
}
