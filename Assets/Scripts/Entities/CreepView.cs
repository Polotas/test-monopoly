using UnityEngine;
using System.Collections;

public class CreepView : MonoBehaviour
{
    public GameObject lifeBarObject;
    public Transform lifeBar;
    public MeshRenderer[] meshRend;
    public Material originalMaterial;

    private void Awake()
    {
        originalMaterial = meshRend[0].material;
    }

    public void SetMaterial(Material mat)
    {
        foreach (var t in meshRend)
        {
            t.material = mat;
        }
    }

    public void FlashDamage(int currentHealth, int maxHealth)
    {
        lifeBarObject.SetActive(currentHealth != maxHealth);
        UpdateBaseHealthDisplay(currentHealth, maxHealth);
        StopAllCoroutines();
        StartCoroutine(DamageFlash());
    }
    
    private void UpdateBaseHealthDisplay(int currentHp,int maxHealth)
    {
        var current = (float)currentHp / (float)maxHealth;
        lifeBar.localScale = new Vector3(current, 1, 1);
    }

    public void ShowSlowEffect()
    {
        foreach (var t in meshRend)
        {
            t.material.color = Color.cyan;
        }
    }

    public void ResetColor()
    {
        foreach (var t in meshRend)
        {
            t.material = originalMaterial;
        }
    }

    private IEnumerator DamageFlash()
    {
        Color originalColor = meshRend[0].material.color;
        
        foreach (var t in meshRend)
        {
            t.material.color = Color.red;
        }
        
        yield return new WaitForSeconds(0.1f);
        foreach (var t in meshRend)
        {
            t.material.color = originalColor;
        }
    }
}