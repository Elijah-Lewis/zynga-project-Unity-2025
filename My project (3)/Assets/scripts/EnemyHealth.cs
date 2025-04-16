using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, 10f, 0); // Position above enemy

    private GameObject healthBarInstance;
    private Image healthBarFill;
    private Canvas healthBarCanvas;

    void Start()
    {
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity);
            healthBarInstance.transform.SetParent(transform); // Make it a child of the enemy

            // Get the fill image component
            healthBarFill = healthBarInstance.GetComponentInChildren<Image>();

            // Get or add Canvas component
            healthBarCanvas = healthBarInstance.GetComponent<Canvas>();
            if (healthBarCanvas == null)
            {
                healthBarCanvas = healthBarInstance.AddComponent<Canvas>();
            }
            healthBarCanvas.worldCamera = Camera.main;

            UpdateHealthBar();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    void LateUpdate()
    {
        if (healthBarInstance != null)
        {
            // Keep healthbar above enemy and facing camera
            healthBarInstance.transform.position = transform.position + healthBarOffset;
            healthBarInstance.transform.rotation = Camera.main.transform.rotation;
        }
    }
}
