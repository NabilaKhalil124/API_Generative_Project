using UnityEngine;
using System.Collections;
using System.Reflection;
using LLMUnity;

public class AutomaticTargetShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f;
    
    [Header("Target Settings")]
    [SerializeField] private Transform chicken;
    [SerializeField] private Transform frog;
    [SerializeField] private Transform spawnPoint;
    
    [Header("AI Settings")]
    [SerializeField] private LLMCharacter llmCharacter;
    
    private Transform currentTarget;

    private void Start()
    {
        if (spawnPoint == null)
        {
            GameObject spawnObj = new GameObject("BulletSpawnPoint");
            spawnPoint = spawnObj.transform;
            spawnPoint.SetParent(transform);
            spawnPoint.localPosition = new Vector3(0, 1, 0);
        }
        
        PowerUpFunctions.Initialize(bulletPrefab, spawnPoint);
        
        if (llmCharacter == null)
        {
            llmCharacter = FindObjectOfType<LLMCharacter>();
            if (llmCharacter == null)
            {
                Debug.LogError("LLMCharacter not found in scene!");
                return;
            }
        }
        
        SetTarget("chicken");
        StartShooting();
    }

    string[] GetFunctionNames<T>()
    {
        var functionNames = new System.Collections.Generic.List<string>();
        foreach (var function in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            if (function.Name.EndsWith("Skill"))
            {
                functionNames.Add(function.Name);
            }
        }
        return functionNames.ToArray();
    }

    private string ConstructPowerUpPrompt(string message)
    {
        string prompt = "Command: " + message + "\n\n";
        prompt += "Reply with EXACTLY one line containing ONLY the function name:\n";
        prompt += "Available functions:\n";
        prompt += "- DoubleTroubleSkill (for 'double trouble' or similar commands)\n";
        prompt += "- PowerShotSkill (for 'power shot' or similar commands)\n";
        prompt += "- NoSkillMentioned (if no power-up is mentioned)\n\n";
        prompt += "Example responses:\n";
        prompt += "DoubleTroubleSkill\n";
        prompt += "PowerShotSkill\n";
        prompt += "NoSkillMentioned";

        return prompt;
    }

    public async void ProcessVoiceCommand(string message)
    {
        try
        {
            Debug.Log($"Processing voice command: {message}");

            // First, check if it's a targeting command
            string messageLower = message.ToLower();
            if (messageLower.Contains("target") || messageLower.Contains("aim at") || messageLower.Contains("shoot at"))
            {
                SetTarget(message);
                return;
            }

            // If not a targeting command, try to process as power-up
            string powerUpResponse = await llmCharacter.Chat(ConstructPowerUpPrompt(message));
            string powerUpFunction = powerUpResponse.Split('\n')[0].Trim();
            Debug.Log($"Power-up function received: {powerUpFunction}");

            // Direct mapping of function names to actions
            switch (powerUpFunction)
            {
                case "DoubleTroubleSkill":
                    Debug.Log("Executing Double Trouble");
                    PowerUpFunctions.ExecuteDoubleTrouble(frog, chicken);
                    break;
                case "PowerShotSkill":
                    Debug.Log("Executing Power Shot");
                    PowerUpFunctions.ExecutePowerShot();
                    break;
                case "NoSkillMentioned":
                    Debug.Log("No power-up skill recognized");
                    break;
                default:
                    Debug.LogWarning($"Unhandled function: {powerUpFunction}");
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error processing voice command: {e.Message}");
            Debug.LogException(e);
        }
    }

    public void SetTarget(string target)
    {
        string targetLower = target.ToLower();
        if (targetLower.Contains("frog"))
        {
            currentTarget = frog;
            Debug.Log("Targeting frog");
        }
        else if (targetLower.Contains("chicken"))
        {
            currentTarget = chicken;
            Debug.Log("Targeting chicken");
        }
        else
        {
            Debug.LogWarning($"Unknown target: {target}");
            return;
        }
        
        if (currentTarget != null)
        {
            RotateSpawnPointTowardsTarget();
        }
        else
        {
            Debug.LogError($"Target reference is null for: {target}");
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
        if (bulletBehavior != null)
        {
            bulletBehavior.Initialize(spawnPoint.up * bulletSpeed, false, 2f);
        }

        Destroy(bullet, 5f);
    }
}
