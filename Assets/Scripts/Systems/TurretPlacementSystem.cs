using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TurretPlacementSystem : MonoBehaviour
{
    public static TurretPlacementSystem Instance { get; private set; }
    
    [Header("Placement Settings")]
    public LayerMask groundLayer = 1;
    public Material previewMaterial;
    public float minPlacementDistance = 2f;
    
    [Header("Turret Types")]
    public List<TurretData> availableTurrets = new List<TurretData>();
    
    // Runtime variables
    private Camera playerCamera;
    private TurretData selectedTurretType;
    private GameObject previewTurret;
    private bool isPlacementMode = false;
    private List<Vector3> placedTurretPositions = new List<Vector3>();
    
    // Properties
    public bool IsInPlacementMode => isPlacementMode;
    public TurretData SelectedTurretType => selectedTurretType;
    
    // Events
    public System.Action<TurretData> OnTurretTypeSelected;
    public System.Action OnPlacementModeExited;
    public System.Action<Turret> OnTurretPlaced;
    
    private WaveManager waveManager;
    private BaseDefense baseDefense;
    
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
        playerCamera = Camera.main;
        
        waveManager = FindObjectOfType<WaveManager>();
        baseDefense = FindObjectOfType<BaseDefense>();
        
        if (playerCamera == null)
            playerCamera = FindObjectOfType<Camera>();
    }
    
    private void Update()
    {
        if (isPlacementMode)
            HandlePlacementMode();
        
        HandleInput();
    }
    
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            ExitPlacementMode();
        
        if (Input.GetKeyDown(KeyCode.Alpha1) && availableTurrets.Count > 0)
            SelectTurretType(availableTurrets[0]);
        if (Input.GetKeyDown(KeyCode.Alpha2) && availableTurrets.Count > 1)
            SelectTurretType(availableTurrets[1]);
    }
    
    private void HandlePlacementMode()
    {
        if (playerCamera == null) return;
        
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 placementPosition = hit.point;
            UpdatePreviewTurret(placementPosition);

            if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
                TryPlaceTurret(placementPosition);
        }
        else
        {
            if (previewTurret != null)
                previewTurret.SetActive(false);
        }
    }
    
    public void SelectTurretType(TurretData turretData)
    {
        if (turretData == null) return;
        
        selectedTurretType = turretData;
        EnterPlacementMode();
        OnTurretTypeSelected?.Invoke(turretData);
    }
    
    private void EnterPlacementMode()
    {
        isPlacementMode = true;
        CreatePreviewTurret();
    }
    
    public void ExitPlacementMode()
    {
        isPlacementMode = false;
        selectedTurretType = null;
        
        if (previewTurret != null)
        {
            Destroy(previewTurret);
            previewTurret = null;
        }
        
        OnPlacementModeExited?.Invoke();
    }
    
    private void CreatePreviewTurret()
    {
        if (selectedTurretType?.prefab == null) return;
        
        if (previewTurret != null)
            Destroy(previewTurret);
        
        previewTurret = ObjectPooler.Instance.SpawnFromPool(selectedTurretType.prefab.name,transform.position,Quaternion.identity);
        
        // Disable all functional components
        var turretComponent = previewTurret.GetComponent<Turret>();
        if (turretComponent != null)
            turretComponent.enabled = false;
        
        var colliders = previewTurret.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
            collider.enabled = false;

        turretComponent.UpdateRange(selectedTurretType);
        
        // Make it semi-transparent
        var renderers = previewTurret.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            if (previewMaterial != null)
            {
                renderer.material = previewMaterial;
            }
            else
            {
                // Fallback: make current materials semi-transparent
                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = new Material(materials[i]);
                    if (materials[i].HasProperty("_Color"))
                    {
                        Color color = materials[i].color;
                        color.a = 0.5f;
                        materials[i].color = color;
                    }
                }
                renderer.materials = materials;
            }
        }
    }
    
    private void UpdatePreviewTurret(Vector3 position)
    {
        if (previewTurret == null) return;
        
        previewTurret.SetActive(true);
        previewTurret.transform.position = position;
        
        // Change color based on whether placement is valid
        bool canPlace = CanPlaceTurretAt(position);
        Color previewColor = canPlace ? Color.green : Color.red;
        previewColor.a = 0.5f;
        
        var renderers = previewTurret.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                if (material.HasProperty("_Color"))
                {
                    material.color = previewColor;
                }
            }
        }
    }
    
    private void TryPlaceTurret(Vector3 position)
    {
        if (!CanPlaceTurretAt(position)) return;
        
        if (!EconomySystem.Instance.CanAfford(selectedTurretType.cost))
        {
            Debug.Log("Not enough coins to buy this turret!");
            return;
        }
        
        if (!EconomySystem.Instance.TryPurchase(selectedTurretType.cost))
        {
            return;
        }
        
        GameObject turretObject = ObjectPooler.Instance.SpawnFromPool(selectedTurretType.prefab.name, position, Quaternion.identity);
        Turret turret = turretObject.GetComponent<Turret>();

        turret.Initialize(selectedTurretType);
        placedTurretPositions.Add(position);
        OnTurretPlaced?.Invoke(turret);
        
        Debug.Log($"Turret {selectedTurretType.turretName} placed at {position}");
    }
    
    private bool CanPlaceTurretAt(Vector3 position)
    {
        foreach (Vector3 turretPos in placedTurretPositions)
        {
            if (Vector3.Distance(position, turretPos) < minPlacementDistance)
            {
                return false;
            }
        }
        
        if (baseDefense != null)
        {
            Vector3 basePosition = baseDefense.GetBasePosition();
            if (Vector3.Distance(position, basePosition) < minPlacementDistance)
            {
                return false;
            }
        }

        if (waveManager == null) return true;
        
        foreach (Transform spawnPoint in waveManager.spawnPoints)
        {
            if (Vector3.Distance(position, spawnPoint.position) < minPlacementDistance)
            {
                return false;
            }
        }

        return true;
    }
    
    private bool IsPointerOverUI() => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

    public void ClearAllTurrets()
    {
        placedTurretPositions.Clear();
        
        Turret[] turrets = FindObjectsOfType<Turret>();
        foreach (Turret turret in turrets)
        {
            if (turret != null)
                turret.gameObject.SetActive(false);
        }
    }
    
    public List<TurretData> GetAvailableTurrets() => new List<TurretData>(availableTurrets);

    private void OnDrawGizmosSelected()
    {
        // Draw placement positions
        Gizmos.color = Color.yellow;
        foreach (Vector3 pos in placedTurretPositions)
        {
            Gizmos.DrawWireSphere(pos, minPlacementDistance);
        }
    }
}
