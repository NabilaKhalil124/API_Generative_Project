using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    private Vector2 velocity;
    private bool isPowered;
    private float damage = 10f; // Default damage
    private RectTransform rectTransform;

    // Layer masks for checking
    private int chickenLayer;
    private int frogLayer;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // Ensure bullet has the correct tag
        gameObject.tag = "Bullet";

        // Get layer numbers by name
        chickenLayer = LayerMask.NameToLayer("Chicken");
        frogLayer = LayerMask.NameToLayer("Frog");
    }

    public void Initialize(Vector2 velocity, bool isPowered, float damage)
    {
        this.velocity = velocity;
        this.isPowered = isPowered;
        this.damage = damage;
    }

    public float GetDamage()
    {
        return damage;
    }

    private void Update()
    {
        // Move the bullet using anchoredPosition for UI space
        rectTransform.anchoredPosition += velocity * Time.deltaTime;
        CheckLayerOverlap();
    }

    private void CheckLayerOverlap()
    {
        // Get all colliders overlapping with the bullet
        Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);

        foreach (Collider2D collider in overlappingColliders)
        {
            // Check if collider is on Chicken layer
            if (collider.gameObject.layer == chickenLayer)
            {
                Debug.Log($"Bullet overlapping with Chicken: {collider.gameObject.name}");
                HandleEnemyCollision(collider);
            }
            // Check if collider is on Frog layer
            else if (collider.gameObject.layer == frogLayer)
            {
                Debug.Log($"Bullet overlapping with Frog: {collider.gameObject.name}");
                HandleEnemyCollision(collider);
            }
        }
    }

    private void HandleEnemyCollision(Collider2D collider)
    {
        Enemy enemy = collider.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleEnemyCollision(other);
    }

    // Optional: Visualize the overlap check radius in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}