using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    public float healthAmount = 100f;
    public float maxHealth = 100f;
    public float smoothSpeed = 5f; 
    private float displayHealth; 
    public DungeonGenerator dungeonGenerator;

    void Start()
    {
        dungeonGenerator = FindObjectOfType<DungeonGenerator>();
        displayHealth = healthAmount; 
    }

    void Update()
    {
        if (healthAmount <= 0)
        {
            ReloadLevel();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            TakeDamage(20);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Heal(5);
        }

        
        displayHealth = Mathf.Lerp(displayHealth, healthAmount, smoothSpeed * Time.deltaTime);

        
        healthBar.fillAmount = displayHealth / maxHealth;
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);
    }

    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, maxHealth);
    }

    void ReloadLevel()
    {
        transform.position = dungeonGenerator.SpawnPosition;
        healthAmount = maxHealth;
        displayHealth = maxHealth; 
    }
}



