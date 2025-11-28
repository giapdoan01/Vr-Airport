using UnityEngine;

public class Oil : MonoBehaviour
{
    [Header("Material Settings")]
    [SerializeField] private Material originalMaterial; // Material gốc (tự động lấy)
    [SerializeField] private Material powderMaterial; // Material bột (kéo vào inspector)
    
    [Header("Spray Settings")]
    [SerializeField] private float timeToChange = 3f; // 3 giây
    
    private Renderer objectRenderer;
    private float sprayTime = 0f;
    private bool isChanged = false;

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        
        if (objectRenderer != null && originalMaterial == null)
        {
            // Tự động lưu material gốc
            originalMaterial = objectRenderer.material;
        }
    }

    public void ApplySpray(float deltaTime)
    {
        if (isChanged) return; 
        
        sprayTime += deltaTime;
        
        if (sprayTime >= timeToChange)
        {
            ChangeToPowderMaterial();
            GameManage.Instance.SetActiveByIndex(17);
        }
        
    }

    private void ChangeToPowderMaterial()
    {
        if (objectRenderer == null || powderMaterial == null) return;
        
        objectRenderer.material = powderMaterial;
        isChanged = true;
    }

    public void ResetMaterial()
    {
        if (objectRenderer != null && originalMaterial != null)
        {
            objectRenderer.material = originalMaterial;
            sprayTime = 0f;
            isChanged = false;
        }
    }

    public bool IsChanged()
    {
        return isChanged;
    }

    public float GetSprayProgress()
    {
        return Mathf.Clamp01(sprayTime / timeToChange) * 100f;
    }
}
