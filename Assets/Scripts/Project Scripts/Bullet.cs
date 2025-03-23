using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private Vector2 velocity;
    private bool isPowered;
    private float damage = 10f;
    private Rigidbody2D rb;

    private void Awake()
    {
        // Get or add required components
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }

        gameObject.tag = "Bullet";
    }

    public void Initialize(Vector2 velocity, bool isPowered, float damage)
    {
        this.velocity = velocity;
        this.isPowered = isPowered;
        this.damage = damage;

        // Apply initial velocity to Rigidbody2D
        rb.linearVelocity = velocity;
    }

    public float GetDamage()
    {
        return damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit an enemy
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log($"Hit enemy: {other.gameObject.name}");
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        // Add any cleanup code here (particles, sound effects, etc.)
    }
}