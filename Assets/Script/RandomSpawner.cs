using UnityEngine;

/// <summary>
/// Spawn random objects (stars, asteroids, enemies...) khi game start
/// </summary>
public class RandomSpawner : MonoBehaviour
{
    [Header("🎯 Spawn Settings")]
    [Tooltip("Loại object muốn spawn")]
    public SpawnType spawnType = SpawnType.Stars;
    
    public enum SpawnType
    {
        Stars,
        Asteroids,
        Enemies,
        Mixed // Spawn cả 3
    }
    
    [Header("📦 Prefabs")]
    [Tooltip("Prefab của star (kéo star prefab vào)")]
    public GameObject starPrefab;
    
    [Tooltip("Các prefabs của asteroids (có thể nhiều loại)")]
    public GameObject[] asteroidPrefabs;
    
    [Tooltip("Các prefabs của enemies")]
    public GameObject[] enemyPrefabs;
    
    [Header("🔢 Spawn Count")]
    [Tooltip("Số lượng stars spawn khi start")]
    public int starCount = 10;
    
    [Tooltip("Số lượng asteroids spawn khi start")]
    public int asteroidCount = 5;
    
    [Tooltip("Số lượng enemies spawn khi start")]
    public int enemyCount = 3;
    
    [Header("📍 Spawn Area")]
    [Tooltip("Tự động tính từ camera")]
    public bool useAutoArea = true;
    
    [Tooltip("Khoảng cách từ biên camera")]
    public float areaPadding = 2f;
    
    [Header("Manual Spawn Area (nếu useAutoArea = false)")]
    public float minX = -15f;
    public float maxX = 15f;
    public float minY = -10f;
    public float maxY = 10f;
    
    [Header("⚙️ Advanced Settings")]
    [Tooltip("Khoảng cách tối thiểu giữa các objects")]
    public float minDistanceBetween = 2f;
    
    [Tooltip("Khoảng cách tối thiểu từ player")]
    public float minDistanceFromPlayer = 5f;
    
    [Tooltip("Spawn random scale cho objects")]
    public bool randomScale = false;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    
    [Header("🔄 Respawn Settings")]
    [Tooltip("Có tự động respawn không khi hết")]
    public bool autoRespawn = false;
    
    [Tooltip("Thời gian delay giữa mỗi lần respawn")]
    public float respawnDelay = 3f;
    
    [Tooltip("Số lượng tối đa trong scene")]
    public int maxStars = 20;
    public int maxAsteroids = 10;
    public int maxEnemies = 5;

    private Transform playerTransform;

    void Start()
    {
        // Tìm player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        // Setup spawn area tự động
        if (useAutoArea)
        {
            SetupAutoArea();
        }
        
        // Spawn initial objects
        SpawnInitialObjects();
        
        // Bật auto respawn nếu cần
        if (autoRespawn)
        {
            InvokeRepeating("CheckAndRespawn", respawnDelay, respawnDelay);
        }
    }

    /// <summary>
    /// Spawn objects ban đầu
    /// </summary>
    void SpawnInitialObjects()
    {
        switch (spawnType)
        {
            case SpawnType.Stars:
                SpawnStars(starCount);
                break;
                
            case SpawnType.Asteroids:
                SpawnAsteroids(asteroidCount);
                break;
                
            case SpawnType.Enemies:
                SpawnEnemies(enemyCount);
                break;
                
            case SpawnType.Mixed:
                SpawnStars(starCount);
                SpawnAsteroids(asteroidCount);
                SpawnEnemies(enemyCount);
                break;
        }
        
        Debug.Log($"✅ RandomSpawner: Spawn hoàn tất! Stars: {starCount}, Asteroids: {asteroidCount}, Enemies: {enemyCount}");
    }

    #region Spawn Methods
    
    /// <summary>
    /// Spawn stars
    /// </summary>
    void SpawnStars(int count)
    {
        if (starPrefab == null)
        {
            Debug.LogWarning("⚠️ RandomSpawner: Star Prefab chưa được gán!");
            return;
        }
        
        for (int i = 0; i < count; i++)
        {
            Vector3 position = GetRandomValidPosition();
            GameObject star = Instantiate(starPrefab, position, Quaternion.identity);
            
            if (randomScale)
            {
                float scale = Random.Range(minScale, maxScale);
                star.transform.localScale = Vector3.one * scale;
            }
            
            star.name = $"Star_{i}";
        }
        
        Debug.Log($"⭐ Spawned {count} stars");
    }
    
    /// <summary>
    /// Spawn asteroids
    /// </summary>
    void SpawnAsteroids(int count)
    {
        if (asteroidPrefabs == null || asteroidPrefabs.Length == 0)
        {
            Debug.LogWarning("⚠️ RandomSpawner: Asteroid Prefabs chưa được gán!");
            return;
        }
        
        for (int i = 0; i < count; i++)
        {
            Vector3 position = GetRandomValidPosition();
            
            // Random prefab
            GameObject prefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
            GameObject asteroid = Instantiate(prefab, position, Quaternion.identity);
            
            if (randomScale)
            {
                float scale = Random.Range(minScale, maxScale);
                asteroid.transform.localScale = Vector3.one * scale;
            }
            
            asteroid.name = $"Asteroid_{i}";
        }
        
        Debug.Log($"🌑 Spawned {count} asteroids");
    }
    
    /// <summary>
    /// Spawn enemies
    /// </summary>
    void SpawnEnemies(int count)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("⚠️ RandomSpawner: Enemy Prefabs chưa được gán!");
            return;
        }
        
        for (int i = 0; i < count; i++)
        {
            Vector3 position = GetRandomValidPosition();
            
            // Random prefab
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemy = Instantiate(prefab, position, Quaternion.identity);
            
            enemy.name = $"Enemy_{i}";
        }
        
        Debug.Log($"👾 Spawned {count} enemies");
    }
    
    #endregion

    #region Position Helpers
    
    /// <summary>
    /// Lấy vị trí random hợp lệ
    /// </summary>
    Vector3 GetRandomValidPosition()
    {
        Vector3 position;
        int attempts = 0;
        int maxAttempts = 30;
        
        do
        {
            // Random position trong area
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            position = new Vector3(x, y, 0);
            
            attempts++;
            
            // Nếu thử quá nhiều lần → bỏ qua validation
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("⚠️ Không tìm được vị trí hợp lệ sau 30 lần thử");
                break;
            }
        }
        while (!IsPositionValid(position));
        
        return position;
    }
    
    /// <summary>
    /// Kiểm tra vị trí có hợp lệ không
    /// </summary>
    bool IsPositionValid(Vector3 position)
    {
        // Kiểm tra khoảng cách từ player
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(position, playerTransform.position);
            if (distanceToPlayer < minDistanceFromPlayer)
            {
                return false; // Quá gần player
            }
        }
        
        // Kiểm tra khoảng cách với objects khác (optional - có thể bỏ để tăng hiệu suất)
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(position, minDistanceBetween);
        if (nearbyObjects.Length > 0)
        {
            return false; // Quá gần objects khác
        }
        
        return true;
    }
    
    /// <summary>
    /// Tự động setup spawn area từ camera
    /// </summary>
    void SetupAutoArea()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;
            
            minX = cam.transform.position.x - camWidth + areaPadding;
            maxX = cam.transform.position.x + camWidth - areaPadding;
            minY = cam.transform.position.y - camHeight + areaPadding;
            maxY = cam.transform.position.y + camHeight - areaPadding;
            
            Debug.Log($"📐 Auto spawn area: X[{minX:F1}, {maxX:F1}], Y[{minY:F1}, {maxY:F1}]");
        }
    }

    #endregion

    #region Auto Respawn

    /// <summary>
    /// Kiểm tra và respawn nếu cần
    /// </summary>
    /// <summary>
    /// Kiểm tra và respawn nếu cần - KHÔNG DÙNG TAGS
    /// </summary>
    void CheckAndRespawn()
    {
        // Đếm số lượng hiện tại bằng cách tìm components
        int currentStars = FindObjectsOfType<Star>()?.Length ?? 0;

        // Đếm asteroids
        int currentAsteroids = FindObjectsOfType<Asteroid>()?.Length ?? 0;

        // Đếm enemies
        int currentEnemies = FindObjectsOfType<Enemy>()?.Length ?? 0;

        Debug.Log($"📊 Current count - Stars: {currentStars}, Asteroids: {currentAsteroids}, Enemies: {currentEnemies}");

        // Respawn stars
        if (spawnType == SpawnType.Stars || spawnType == SpawnType.Mixed)
        {
            if (currentStars < maxStars && starPrefab != null)
            {
                int toSpawn = Mathf.Min(starCount / 2, maxStars - currentStars);
                if (toSpawn > 0)
                {
                    SpawnStars(toSpawn);
                    Debug.Log($"⭐ Respawned {toSpawn} stars");
                }
            }
        }

        // Respawn asteroids
        if (spawnType == SpawnType.Asteroids || spawnType == SpawnType.Mixed)
        {
            if (currentAsteroids < maxAsteroids && asteroidPrefabs != null && asteroidPrefabs.Length > 0)
            {
                int toSpawn = Mathf.Min(asteroidCount / 2, maxAsteroids - currentAsteroids);
                if (toSpawn > 0)
                {
                    SpawnAsteroids(toSpawn);
                    Debug.Log($"🌑 Respawned {toSpawn} asteroids");
                }
            }
        }

        // Respawn enemies
        if (spawnType == SpawnType.Enemies || spawnType == SpawnType.Mixed)
        {
            if (currentEnemies < maxEnemies && enemyPrefabs != null && enemyPrefabs.Length > 0)
            {
                int toSpawn = Mathf.Min(enemyCount / 2, maxEnemies - currentEnemies);
                if (toSpawn > 0)
                {
                    SpawnEnemies(toSpawn);
                    Debug.Log($"👾 Respawned {toSpawn} enemies");
                }
            }
        }
    }

    #endregion

    #region Debug Visualization

    void OnDrawGizmos()
    {
        // Vẽ spawn area
        Gizmos.color = Color.green;
        
        Vector3 center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0);
        
        Gizmos.DrawWireCube(center, size);
        
        // Vẽ min distance từ player
        if (playerTransform != null && Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, minDistanceFromPlayer);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Vẽ các vị trí spawn preview
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        
        // Vẽ một số điểm random
        for (int i = 0; i < 20; i++)
        {
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            Gizmos.DrawSphere(new Vector3(x, y, 0), 0.3f);
        }
    }
    
    #endregion
}
