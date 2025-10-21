using UnityEngine;

/// <summary>
/// Asteroid thông minh - Di chuyển trong boundaries, có health, có thể phá hủy
/// </summary>
public class Asteroid : MonoBehaviour
{
    [Header("🌑 Asteroid Stats")]
    [Tooltip("Máu của asteroid (0 = không thể phá hủy)")]
    public int health = 50;
    [Tooltip("Điểm cho player khi phá hủy")]
    public int scoreValue = 20;
    [Tooltip("Damage gây cho player khi va chạm")]
    public int collisionDamage = 10;

    [Header("🎯 Movement Type")]
    public MovementType movementType = MovementType.BounceInBoundary;

    public enum MovementType
    {
        Straight,           // Bay thẳng (ra khỏi màn hình thì destroy)
        BounceInBoundary,   // Dội lại khi chạm boundary ⭐ Recommended
        PatrolLoop,         // Bay theo vòng tròn
        ChasePlayer,        // Đuổi theo player (chậm)
        RandomWander        // Đi lang thang ngẫu nhiên
    }

    [Header("⚙️ Movement Settings")]
    [Tooltip("Tốc độ di chuyển")]
    public float moveSpeed = 2f;
    [Tooltip("Tốc độ xoay (degrees/second)")]
    public float rotationSpeed = 30f;

    [Header("📦 Boundaries")]
    [Tooltip("Giới hạn di chuyển (tự động từ camera hoặc thủ công)")]
    public bool useAutoBoundary = true;
    public float boundaryPadding = 2f; // Khoảng cách từ edge camera

    [Header("Manual Boundaries (nếu useAutoBoundary = false)")]
    public float minX = -20f;
    public float maxX = 20f;
    public float minY = -20f;
    public float maxY = 20f;

    [Header("🎯 Chase Settings (for ChasePlayer)")]
    public float chaseSpeed = 1.5f;
    public float chaseRange = 15f;

    [Header("💥 Collision Settings")]
    [Tooltip("Va chạm có làm game over không")]
    public bool causeGameOver = true;
    [Tooltip("Có phá hủy player không (nếu player có health)")]
    public bool damagePlayer = true;
    [Tooltip("Asteroid tự hủy khi chạm player")]
    public bool destroyOnPlayerCollision = true;

    [Header("💀 Death Effects")]
    public GameObject deathEffect;
    public AudioClip deathSound;

    [Header("🎁 Drops")]
    public GameObject dropItem;
    [Range(0, 100)]
    public float dropChance = 30f;

    // Private variables
    private Vector2 moveDirection;
    private Transform playerTransform;
    private float wanderTimer = 0f;
    private Vector2 wanderTarget;
    private int maxHealth;

    void Start()
    {
        maxHealth = health;

        // Setup boundaries tự động
        if (useAutoBoundary)
        {
            SetupAutoBoundaries();
        }

        // Setup movement direction
        moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

        // Tìm player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Random rotation ban đầu
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        Debug.Log($"Asteroid {gameObject.name}: Started with direction {moveDirection}, Type: {movementType}");
    }

    void Update()
    {
        // Di chuyển theo type
        switch (movementType)
        {
            case MovementType.Straight:
                MoveStraight();
                break;
            case MovementType.BounceInBoundary:
                MoveBounce();
                break;
            case MovementType.PatrolLoop:
                MoveOrbit();
                break;
            case MovementType.ChasePlayer:
                ChasePlayer();
                break;
            case MovementType.RandomWander:
                RandomWander();
                break;
        }

        // Xoay asteroid (visual effect)
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    #region Movement Methods

    /// <summary>
    /// Bay thẳng - Tự hủy khi ra khỏi boundaries
    /// </summary>
    void MoveStraight()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        // Kiểm tra ra khỏi boundaries → Destroy
        if (transform.position.x < minX - 5 || transform.position.x > maxX + 5 ||
            transform.position.y < minY - 5 || transform.position.y > maxY + 5)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Bounce off walls - Dội lại khi chạm biên
    /// </summary>
    void MoveBounce()
    {
        // Di chuyển
        Vector2 newPos = (Vector2)transform.position + moveDirection * moveSpeed * Time.deltaTime;

        // Kiểm tra boundaries và bounce
        bool bounced = false;

        // Bounce X
        if (newPos.x <= minX || newPos.x >= maxX)
        {
            moveDirection.x = -moveDirection.x; // Đảo hướng X
            bounced = true;
        }

        // Bounce Y
        if (newPos.y <= minY || newPos.y >= maxY)
        {
            moveDirection.y = -moveDirection.y; // Đảo hướng Y
            bounced = true;
        }

        // Clamp position trong boundaries
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        transform.position = newPos;

        if (bounced)
        {
            Debug.Log($"{gameObject.name} bounced! New direction: {moveDirection}");
        }
    }

    /// <summary>
    /// Bay theo vòng tròn quanh điểm ban đầu
    /// </summary>
    void MoveOrbit()
    {
        float radius = 5f;
        float angle = Time.time * moveSpeed;

        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        transform.position = Vector3.Lerp(transform.position,
            Vector3.zero + offset, Time.deltaTime * moveSpeed);
    }

    /// <summary>
    /// Đuổi theo player (chậm)
    /// </summary>
    void ChasePlayer()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= chaseRange)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position,
                chaseSpeed * Time.deltaTime);
        }
        else
        {
            // Ngoài tầm → Đi lang thang
            MoveBounce();
        }
    }

    /// <summary>
    /// Đi lang thang ngẫu nhiên
    /// </summary>
    void RandomWander()
    {
        wanderTimer += Time.deltaTime;

        // Đổi hướng mỗi 2-4 giây
        if (wanderTimer >= Random.Range(2f, 4f))
        {
            wanderTimer = 0f;

            // Random target trong boundaries
            wanderTarget = new Vector2(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY)
            );
        }

        // Di chuyển về target
        transform.position = Vector2.MoveTowards(transform.position, wanderTarget,
            moveSpeed * Time.deltaTime);
    }

    #endregion

    #region Damage & Death

    /// <summary>
    /// Nhận damage từ bullet
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (health <= 0) return; // Invincible nếu health = 0

        health -= damage;
        Debug.Log($"{gameObject.name} nhận {damage} damage! Health: {health}/{maxHealth}");

        // Flash effect
        FlashWhite();

        // Kiểm tra chết
        if (health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Asteroid bị phá hủy
    /// </summary>
    void Die()
    {
        Debug.Log($"💥 {gameObject.name} bị phá hủy!");

        // Cộng điểm
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        // Spawn death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Play sound
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        // Drop item
        TryDropItem();

        // Destroy
        Destroy(gameObject);
    }

    void TryDropItem()
    {
        if (dropItem != null)
        {
            float roll = Random.Range(0f, 100f);
            if (roll <= dropChance)
            {
                Instantiate(dropItem, transform.position, Quaternion.identity);
                Debug.Log($"🎁 {gameObject.name} rơi item!");
            }
        }
    }

    #endregion

    #region Collision

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Asteroid collision: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");

        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Asteroid trigger: {other.gameObject.name} (Tag: {other.tag})");

        if (other.CompareTag("Player"))
        {
            HandlePlayerCollision(other.gameObject);
        }
    }

    void HandlePlayerCollision(GameObject player)
    {
        Debug.Log($"💥 Asteroid hit player!");

       

        // Game over
        if (causeGameOver && GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }

        // Destroy asteroid
        if (destroyOnPlayerCollision)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Helpers

    void SetupAutoBoundaries()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            float camHeight = cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            minX = cam.transform.position.x - camWidth - boundaryPadding;
            maxX = cam.transform.position.x + camWidth + boundaryPadding;
            minY = cam.transform.position.y - camHeight - boundaryPadding;
            maxY = cam.transform.position.y + camHeight + boundaryPadding;

            Debug.Log($"Auto boundaries: X[{minX:F1}, {maxX:F1}], Y[{minY:F1}, {maxY:F1}]");
        }
    }

    void FlashWhite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            StartCoroutine(FlashCoroutine(sr));
        }
    }

    System.Collections.IEnumerator FlashCoroutine(SpriteRenderer sr)
    {
        Color original = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sr.color = original;
    }

    #endregion

    #region Debug Visualization

    void OnDrawGizmos()
    {
        // Vẽ boundaries
        Gizmos.color = Color.yellow;

        float drawMinX = useAutoBoundary && Application.isPlaying ? minX : minX;
        float drawMaxX = useAutoBoundary && Application.isPlaying ? maxX : maxX;
        float drawMinY = useAutoBoundary && Application.isPlaying ? minY : minY;
        float drawMaxY = useAutoBoundary && Application.isPlaying ? maxY : maxY;

        // Vẽ khung boundaries
        Vector3 topLeft = new Vector3(drawMinX, drawMaxY, 0);
        Vector3 topRight = new Vector3(drawMaxX, drawMaxY, 0);
        Vector3 bottomLeft = new Vector3(drawMinX, drawMinY, 0);
        Vector3 bottomRight = new Vector3(drawMaxX, drawMinY, 0);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        // Vẽ chase range nếu dùng chase mode
        if (movementType == MovementType.ChasePlayer)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ move direction
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, moveDirection * 2f);
        }

        // Hiển thị health
#if UNITY_EDITOR
        if (Application.isPlaying && health > 0)
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f,
                $"HP: {health}/{maxHealth}");
        }
#endif
    }

    #endregion
}