using UnityEngine;
using UnityEngine.UI; // For health bar UI if you want to add it later

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    private bool isDamaged = false; // Flag to prevent multiple hits from same bullet
    private float damageResetTime = 0.1f; // Time before enemy can take damage again
    
    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleBulletCollision(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        HandleBulletCollision(other);
    }

    private void HandleBulletCollision(Collider2D other)
    {
        if (!isDamaged && other.CompareTag("Bullet"))
        {
            BulletBehavior bullet = other.GetComponent<BulletBehavior>();
            if (bullet != null)
            {
                // Take damage based on bullet's damage value
                TakeDamage(bullet.GetDamage());
                
                // Destroy the bullet
                Destroy(other.gameObject);
                
                // Set damage flag and start reset timer
                isDamaged = true;
                Invoke("ResetDamageFlag", damageResetTime);
            }
        }
    }

    private void ResetDamageFlag()
    {
        isDamaged = false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy {gameObject.name} took {damage} damage. Health: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Add any death effects here (particles, sound, etc.)
        Debug.Log($"Enemy {gameObject.name} destroyed!");
        
        // Destroy the enemy
        Destroy(gameObject);
    }

    // Public getter for current health (useful for other systems)
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
} 