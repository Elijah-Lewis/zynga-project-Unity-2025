using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    public float healthAmount = 100f;
    public DungeonGenerator dungeonGenerator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dungeonGenerator = FindObjectOfType<DungeonGenerator>();
        
    }

    // Update is called once per frame
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

       if(Input.GetKeyDown(KeyCode.Space))
        {
            Heal(5);
        }
    }

    public void TakeDamage(float damage)
    {
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f;
    }

    public void Heal(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);

        healthBar.fillAmount = healthAmount / 100f;
    }

    void ReloadLevel()
    {
        transform.position = dungeonGenerator.SpawnPosition;

        healthAmount = 100f;
        healthBar.fillAmount = healthAmount / 100f;
    }
}
