using UnityEngine;

/// <summary>
/// Enemy bắn đạn về phía player - Đạn có thể gây damage/game over
/// </summary>
public class EnemyShooter : MonoBehaviour
{
    [Header("🔫 Shooting Settings")]
    [Tooltip("Prefab đạn của enemy (phải có Rigidbody2D)")]
    public GameObject bulletPrefab;

    [Tooltip("Vị trí bắn đạn (FirePoint child object)")]
    public Transform firePoint;

    [Tooltip("Thời gian giữa mỗi lần bắn (giây)")]
    public float fireRate = 2f;

    [Tooltip("Tốc độ đạn")]
    public float bulletSpeed = 8f;

    [Tooltip("Damage mỗi viên đạn gây cho player")]
    public int bulletDamage = 10;

    [Header("🎯 Aim Settings")]
    [Tooltip("Tự động ngắm player")]
    public bool aimAtPlayer = true;

    [Tooltip("Chỉ bắn khi player trong tầm này")]
    public float shootRange = 15f;

    [Tooltip("Độ chính xác (0 = perfect, 10 = rất sai)")]
    [Range(0f, 10f)]
    public float inaccuracy = 0f;

    [Header("🎨 Visual Effects")]
    [Tooltip("Hiệu ứng muzzle flash khi bắn")]
    public GameObject muzzleFlashEffect;

    [Tooltip("Âm thanh khi bắn")]
    public AudioClip shootSound;

    private Transform playerTransform;
    private float nextFireTime = 0f;
    private bool canShoot = true;

    void Start()
    {
        // Tìm player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning($"⚠️ {gameObject.name}: Không tìm thấy Player!");
            canShoot = false;
        }

        // Kiểm tra setup
        if (bulletPrefab == null)
        {
            Debug.LogError($"❌ {gameObject.name}: Bullet Prefab chưa gán!");
            canShoot = false;
        }

        if (firePoint == null)
        {
            Debug.LogWarning($"⚠️ {gameObject.name}: Fire Point chưa gán! Sẽ bắn từ vị trí enemy.");
            firePoint = transform;
        }
    }

    void Update()
    {
        if (!canShoot || playerTransform == null) return;

        // Kiểm tra cooldown
        if (Time.time >= nextFireTime)
        {
            // Kiểm tra player có trong tầm không
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= shootRange)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    /// <summary>
    /// Bắn đạn
    /// </summary>
    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Tạo đạn
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Setup đạn
        SetupBullet(bullet);

        // Hiệu ứng
        PlayShootEffects();

        Debug.Log($"🔫 {gameObject.name} bắn đạn!");
    }

    /// <summary>
    /// Setup bullet direction và damage
    /// </summary>
    void SetupBullet(GameObject bullet)
    {
        Vector3 shootDirection;

        if (aimAtPlayer && playerTransform != null)
        {
            // Tính hướng về player
            shootDirection = (playerTransform.position - firePoint.position).normalized;

            // Thêm độ không chính xác
            if (inaccuracy > 0)
            {
                float randomX = Random.Range(-inaccuracy, inaccuracy) * 0.1f;
                float randomY = Random.Range(-inaccuracy, inaccuracy) * 0.1f;
                shootDirection += new Vector3(randomX, randomY, 0);
                shootDirection.Normalize();
            }
        }
        else
        {
            // Bắn thẳng theo hướng FirePoint
            shootDirection = firePoint.up;
        }

        // Xoay bullet theo hướng bắn
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle - 90); // -90 vì sprite hướng lên

        // Set velocity bằng Rigidbody2D
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = shootDirection * bulletSpeed;
        }
        else
        {
            Debug.LogWarning($"⚠️ Enemy bullet không có Rigidbody2D! Thêm component này vào prefab.");
        }

        // Set damage cho bullet (nếu có EnemyBullet script)
        EnemyBullet enemyBulletScript = bullet.GetComponent<EnemyBullet>();
        if (enemyBulletScript != null)
        {
            enemyBulletScript.damage = bulletDamage;
            enemyBulletScript.speed = bulletSpeed;
            enemyBulletScript.direction = shootDirection;
        }
    }

    /// <summary>
    /// Hiệu ứng khi bắn
    /// </summary>
    void PlayShootEffects()
    {
        // Muzzle flash
        if (muzzleFlashEffect != null && firePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashEffect, firePoint.position, firePoint.rotation);
            Destroy(flash, 0.5f);
        }

        // Sound
        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound, transform.position);
        }
    }

    /// <summary>
    /// Vẽ shoot range trong Scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);

        // Vẽ line đến player (nếu đang chạy)
        if (Application.isPlaying && playerTransform != null && aimAtPlayer)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }
}