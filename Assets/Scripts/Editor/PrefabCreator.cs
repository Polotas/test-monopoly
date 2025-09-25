using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class PrefabCreator : EditorWindow
{
    [MenuItem("Tower Defense/Create Basic Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<PrefabCreator>("Prefab Creator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Basic Prefab Creator", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Create Projectile Prefab"))
        {
            CreateProjectilePrefab();
        }
        
        if (GUILayout.Button("Create Creep Prefabs"))
        {
            CreateCreepPrefabs();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "This tool creates basic prefabs for testing.\\n" +
            "You should customize materials and models later.", 
            MessageType.Info);
    }
    
    private void CreateProjectilePrefab()
    {
        string path = "Assets/Prefabs";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        
        // Create basic projectile
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.name = "BasicProjectile";
        projectile.transform.localScale = Vector3.one * 0.2f;
        
        // Configurar componentes
        Destroy(projectile.GetComponent<Collider>());
        SphereCollider trigger = projectile.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        
        Rigidbody rb = projectile.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;
        
        projectile.AddComponent<Projectile>();
        
        // Make it yellow
        projectile.GetComponent<Renderer>().material.color = Color.yellow;
        
        // Salvar como prefab
        string prefabPath = path + "/BasicProjectile.prefab";
        PrefabUtility.SaveAsPrefabAsset(projectile, prefabPath);
        DestroyImmediate(projectile);
        
        Debug.Log("Projectile prefab created at: " + prefabPath);
    }
    
    private void CreateCreepPrefabs()
    {
        string path = "Assets/Prefabs";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        
        // Creep Pequeno
        GameObject smallCreep = GameObject.CreatePrimitive(PrimitiveType.Cube);
        smallCreep.name = "CreepSmall";
        smallCreep.transform.localScale = Vector3.one * 0.8f;
        
        // Configurar componentes
        Rigidbody rbSmall = smallCreep.AddComponent<Rigidbody>();
        rbSmall.useGravity = false;
        rbSmall.freezeRotation = true;
        
        smallCreep.AddComponent<CreepController>();
        smallCreep.GetComponent<Renderer>().material.color = Color.red;
        
        // Salvar prefab
        string smallPrefabPath = path + "/CreepSmall.prefab";
        PrefabUtility.SaveAsPrefabAsset(smallCreep, smallPrefabPath);
        DestroyImmediate(smallCreep);
        
        // Creep Grande
        GameObject bigCreep = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bigCreep.name = "CreepBig";
        bigCreep.transform.localScale = Vector3.one * 1.2f;
        
        // Configurar componentes
        Rigidbody rbBig = bigCreep.AddComponent<Rigidbody>();
        rbBig.useGravity = false;
        rbBig.freezeRotation = true;
        
        bigCreep.AddComponent<CreepController>();
        bigCreep.GetComponent<Renderer>().material.color = Color.magenta;
        
        // Salvar prefab
        string bigPrefabPath = path + "/CreepBig.prefab";
        PrefabUtility.SaveAsPrefabAsset(bigCreep, bigPrefabPath);
        DestroyImmediate(bigCreep);
        
        Debug.Log("Creep prefabs created at: " + path);
    }
}
#endif
