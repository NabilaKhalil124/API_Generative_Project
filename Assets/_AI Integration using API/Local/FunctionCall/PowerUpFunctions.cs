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

    // Skill identifier methods
    public static string DoubleTroubleSkill()
    {
        return "Double Trouble";
    }

    public static string PowerShotSkill()
    {
        return "Power Shot";
    }

    public static string NoSkillMentioned()
    {
        return "None";
    }

    // Execution methods
    public static void ExecuteDoubleTrouble(Transform target1, Transform target2)
    {
        if (!CheckInitialization()) return;

        if (target1 != null)
        {
            CreateTargetedBullet(target1.position);
        }

        if (target2 != null)
        {
            CreateTargetedBullet(target2.position);
        }

        Debug.Log("Double Trouble executed!");
    }

    public static void ExecutePowerShot()
    {
        if (!CheckInitialization()) return;

        GameObject powerBullet = Object.Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        powerBullet.transform.localScale = new Vector3(2f, 2f, 2f);
        
        SpriteRenderer spriteRenderer = powerBullet.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }
        
        Rigidbody2D rb = powerBullet.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = powerBullet.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }
        
        BulletBehavior bulletBehavior = powerBullet.GetComponent<BulletBehavior>();
        if (bulletBehavior != null)
        {
            bulletBehavior.Initialize(spawnPoint.up * powerBulletSpeed, true, 25f);
        }

        Debug.Log("Power Shot executed!");
    }

    // Helper method to create a targeted bullet
    private static void CreateTargetedBullet(Vector3 targetPosition)
    {
        Vector2 direction = ((Vector2)targetPosition - (Vector2)spawnPoint.position).normalized;
        GameObject bullet = Object.Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
        
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = bullet.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }
        
        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
        if (bulletBehavior != null)
        {
            bulletBehavior.Initialize(direction * normalBulletSpeed, false, 10f);
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

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

