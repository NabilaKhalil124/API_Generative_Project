using UnityEngine;

// Static class for power-up functions
public class PowerUpFunctions
{
    // Static references needed for bullet creation
    private static GameObject bulletPrefab;
    private static Transform spawnPoint;
    private static float normalBulletSpeed = 10f;
    private static float powerBulletSpeed = 15f;

    // Initialize static references
    public static void Initialize(GameObject bulletPref, Transform spawn)
    {
        bulletPrefab = bulletPref;
        spawnPoint = spawn;
        Debug.Log($"PowerUpFunctions initialized with bullet prefab: {bulletPref != null}, spawn point: {spawn != null}");
    }

    // Double Trouble targeting specific enemies
    public static void DoubleTrouble(Transform redSquare, Transform blueSquare)
    {
        if (!CheckInitialization()) return;

        // Create bullet for red square
        if (redSquare != null)
        {
            CreateTargetedBullet(redSquare.position);
        }

        // Create bullet for blue square
        if (blueSquare != null)
        {
            CreateTargetedBullet(blueSquare.position);
        }

        Debug.Log("Double Trouble activated - targeting both squares!");
    }

    // Helper method to create a targeted bullet
    private static void CreateTargetedBullet(Vector3 targetPosition)
    {
        // Calculate direction to target
        Vector2 direction = ((Vector2)targetPosition - (Vector2)spawnPoint.position).normalized;
        
        // Create bullet
        GameObject bullet = Object.Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
        
        // Get or add required components
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = bullet.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }
        
        // Initialize bullet
        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
        if (bulletBehavior != null)
        {
            bulletBehavior.Initialize(direction * normalBulletSpeed, false, 10f);
        }

        // Rotate bullet to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Power Shot with enhanced properties
    public static void PowerShot()
    {
        if (!CheckInitialization()) return;

        // Create powered-up bullet
        GameObject powerBullet = Object.Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // Scale up the bullet
        powerBullet.transform.localScale = new Vector3(2f, 2f, 2f);
        
        // Change color to red
        SpriteRenderer spriteRenderer = powerBullet.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }
        
        // Get or add required components
        Rigidbody2D rb = powerBullet.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = powerBullet.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }
        
        // Initialize with enhanced properties
        BulletBehavior bulletBehavior = powerBullet.GetComponent<BulletBehavior>();
        if (bulletBehavior != null)
        {
            bulletBehavior.Initialize(spawnPoint.up * powerBulletSpeed, true, 25f);
        }

        Debug.Log("Power Shot activated - enhanced bullet fired!");
    }

    // Helper method to check initialization
    private static bool CheckInitialization()
    {
        if (bulletPrefab == null || spawnPoint == null)
        {
            Debug.LogError("PowerUpFunctions not properly initialized! Call Initialize first.");
            return false;
        }
        return true;
    }

    // Get available function names
    public static string[] GetPowerUpFunctionNames()
    {
        return new string[] { "Double Trouble.", "Power Shot." };
    }
} 