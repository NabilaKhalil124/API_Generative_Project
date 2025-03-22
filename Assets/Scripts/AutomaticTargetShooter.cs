using UnityEngine;
using System.Collections;

public class AutomaticTargetShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 300f;
    [SerializeField] private float fireRate = 0.5f;
    
    [Header("Target Settings")]
    [SerializeField] private RectTransform blueSquare;
    [SerializeField] private RectTransform redSquare;
    [SerializeField] private Transform spawnPoint;
    
    private RectTransform currentTarget;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
        if (spawnPoint == null)
        {
            GameObject spawnObj = new GameObject("BulletSpawnPoint");
            spawnPoint = spawnObj.transform;
            spawnPoint.SetParent(transform);
            spawnPoint.localPosition = new Vector3(0, 50, 0); // Offset upward
        }
        
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
        
        // Immediately rotate spawn point to face target
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

        // Calculate direction from spawn point to target
        Vector2 spawnPos = spawnPoint.position;
        Vector2 targetPos = currentTarget.position;
        Vector2 direction = (targetPos - spawnPos).normalized;

        // Calculate angle to target (-90 because UI elements face right by default)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        
        // Apply rotation to the spawn point only
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

        // Instantiate bullet as child of the canvas
        GameObject bullet = Instantiate(bulletPrefab, transform.parent);
        RectTransform bulletRect = bullet.GetComponent<RectTransform>();
        
        // Set initial position
        bulletRect.position = spawnPoint.position;
        
        // Initialize bullet behavior
        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
        if (bulletBehavior == null)
        {
            bulletBehavior = bullet.AddComponent<BulletBehavior>();
        }
        
        // Use spawn point's up direction for bullet velocity
        Vector2 shootDirection = spawnPoint.up;
        
        // Initialize bullet with velocity in the spawn point's up direction
        bulletBehavior.Initialize(shootDirection * bulletSpeed, false, 10f);
        
        // Match bullet rotation to spawn point
        bulletRect.rotation = spawnPoint.rotation;

        // Destroy after delay
        Destroy(bullet, 5f);
    }
} 