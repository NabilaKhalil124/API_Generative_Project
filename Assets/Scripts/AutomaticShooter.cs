using UnityEngine;
using System.Collections;

public class AutomaticShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f; // Time between shots
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint; // Where bullets will spawn
    [SerializeField] private bool shootOnStart = true;
    [SerializeField] private Vector3 shootDirection = Vector3.right; // Default shooting direction
    
    [Header("Optional Settings")]
    [SerializeField] private bool useRandomDelay = false;
    [SerializeField] private float minRandomDelay = 0.3f;
    [SerializeField] private float maxRandomDelay = 1.0f;

    private void Start()
    {
        // If spawnPoint is not set, use this object's position
        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }

        if (shootOnStart)
        {
            StartShooting();
        }
    }

    public void StartShooting()
    {
        StartCoroutine(ShootRoutine());
    }

    public void StopShooting()
    {
        StopAllCoroutines();
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            Shoot();
            
            // Wait for next shot
            if (useRandomDelay)
            {
                yield return new WaitForSeconds(Random.Range(minRandomDelay, maxRandomDelay));
            }
            else
            {
                yield return new WaitForSeconds(fireRate);
            }
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab not assigned to AutomaticShooter!");
            return;
        }

        // Instantiate bullet at spawn point
        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity,transform);
        
        // Add velocity to the bullet
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = shootDirection.normalized * bulletSpeed;
        }
        else
        {
            // If no Rigidbody2D, try to move the bullet using transform
            StartCoroutine(MoveBullet(bullet));
        }

        // Destroy bullet after 5 seconds to prevent memory leaks
        Destroy(bullet, 5f);
    }

    private IEnumerator MoveBullet(GameObject bullet)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = bullet.transform.position;

        while (elapsedTime < 5f)
        {
            bullet.transform.position += shootDirection.normalized * bulletSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    // Public method to change shooting direction at runtime
    public void SetShootDirection(Vector3 newDirection)
    {
        shootDirection = newDirection.normalized;
    }

    // Public method to change fire rate at runtime
    public void SetFireRate(float newFireRate)
    {
        fireRate = Mathf.Max(0.1f, newFireRate); // Ensure fire rate isn't too small
        StopShooting();
        StartShooting();
    }
} 