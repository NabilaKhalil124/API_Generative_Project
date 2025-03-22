using UnityEngine;
using System.Collections;

public class AutomaticTargetShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f;
    
    [Header("Target Settings")]
    [SerializeField] private Transform blueSquare;
    [SerializeField] private Transform redSquare;
    [SerializeField] private Transform spawnPoint;
    
    private Transform currentTarget;
    private Transform shooterTransform;

    private void Start()
    {
        shooterTransform = transform;
        
        if (spawnPoint == null)
        {
            GameObject spawnObj = new GameObject("BulletSpawnPoint");
            spawnPoint = spawnObj.transform;
            spawnPoint.SetParent(transform);
            spawnPoint.localPosition = new Vector3(0, 1, 0);
        }
        
        // Initialize PowerUpFunctions with the bullet prefab and spawn point
        PowerUpFunctions.Initialize(bulletPrefab, spawnPoint);
        
        SetTarget("blue");
        StartShooting();
    }

    public void SetTarget(string color)
    {
        switch (color.ToLower())
        {
            case "blue":
                currentTarget = blueSquare;
                break;
            case "red":
                currentTarget = redSquare;
                break;
            default:
                Debug.LogWarning($"Unknown target color: {color}");
                return;
        }
        
        if (currentTarget != null)
        {
            RotateSpawnPointTowardsTarget();
        }
    }

    private void Update()
    {
        if (currentTarget != null)
        {
            RotateSpawnPointTowardsTarget();
        }
    }

    private void RotateSpawnPointTowardsTarget()
    {
        if (spawnPoint == null || currentTarget == null) return;

        Vector2 spawnPos = spawnPoint.position;
        Vector2 targetPos = currentTarget.position;
        Vector2 direction = (targetPos - spawnPos).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        
        spawnPoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void StartShooting()
    {
        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(fireRate);
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || spawnPoint == null)
        {
            Debug.LogError("Bullet prefab or spawn point not assigned!");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = bullet.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }
        
        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
        if (bulletBehavior == null)
        {
            bulletBehavior = bullet.AddComponent<BulletBehavior>();
        }
        
        Vector2 shootDirection = spawnPoint.up;
        
        bulletBehavior.Initialize(shootDirection * bulletSpeed, false, 2f);

        Destroy(bullet, 5f);
    }

    // Example voice command processing
    public void ProcessPowerUpCommand(string command)
    {
        if (command.Contains("Double") || command.Contains("target all"))
        {
            PowerUpFunctions.DoubleTrouble(redSquare, blueSquare);
        }
        else if (command.Contains("Power") || command.Contains("powerful shot"))
        {
            PowerUpFunctions.PowerShot();
        }
    }
}