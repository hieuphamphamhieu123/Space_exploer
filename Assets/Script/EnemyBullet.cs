using UnityEngine;

/// <summary>
/// Đạn của enemy - GÂY DAMAGE/GAME OVER cho player
/// </summary>
public class EnemyBullet : MonoBehaviour
{
    [Header("💥 Damage Settings")]
    [Tooltip("Damage gây cho player")]
    public int damage = 10;

    [Tooltip("Có làm game over ngay không (nếu false thì chỉ trừ máu)")]
    public bool instantKill = false;

    [Header("⚙️ Bullet Settings")]
    [HideInInspector]
    public Vector3 direction = Vector3.down;

    [HideInInspector]
    public float speed = 5f;

    [Tooltip("Thời gian tồn tại (giây)")]
    public float lifetime = 5f;

    [Header("💥 Effects")]
    [Tooltip("Hiệu ứng khi va chạm")]
    public GameObject hitEffect;

    [Tooltip("Âm thanh khi hit")]
    public AudioClip hitSound;

    void Start()
    {
        // Tự hủy sau lifetime giây
        Destroy(gameObject, lifetime);

        // Kiểm tra setup
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogError("❌ EnemyBullet: Thiếu Collider2D! Thêm Circle Collider 2D với Is Trigger = true");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning("⚠️ EnemyBullet: Collider2D chưa tick Is Trigger!");
        }
    }

    void Update()
    {
        // Di chuyển theo direction (backup nếu không dùng Rigidbody2D)
        if (GetComponent<Rigidbody2D>() == null)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Debug log
        Debug.Log($"🔵 EnemyBullet chạm: {other.gameObject.name} (Tag: {other.tag})");

        // Chạm PLAYER → GÂY DAMAGE!
        if (other.CompareTag("Player"))
        {
            Debug.Log($"💥💥💥 ENEMY BULLET HIT PLAYER! Damage: {damage}");
            HitPlayer(other.gameObject);
            return;
        }

        // Bỏ qua enemy (không bắn enemy của chính mình)
        if (other.GetComponent<Enemy>() != null)
        {
            Debug.Log("⚪ EnemyBullet chạm Enemy - bỏ qua");
            return;
        }

        // Chạm vật khác → Destroy bullet
        Debug.Log("⚫ EnemyBullet chạm object khác - destroy");
        DestroyBullet(other.transform.position);
    }

    /// <summary>
    /// Xử lý khi hit player
    /// </summary>
    void HitPlayer(GameObject player)
    {
        // Hiệu ứng
        PlayHitEffects(player.transform.position);

        
        // Option 2: Instant kill hoặc không có PlayerHealth
        if (instantKill )
        {
            Debug.Log("💀 Player bị tiêu diệt!");

            // Game Over
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                Debug.LogError("❌ GameManager.Instance is NULL!");
            }
        }

        // Destroy bullet
        Destroy(gameObject);
    }

    /// <summary>
    /// Hiệu ứng khi hit
    /// </summary>
    void PlayHitEffects(Vector3 position)
    {
        // Spawn hit effect
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, position, Quaternion.identity);
            Destroy(effect, 1f);
        }

        // Play sound
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, position);
        }
    }

    /// <summary>
    /// Destroy bullet với effects
    /// </summary>
    void DestroyBullet(Vector3 position)
    {
        PlayHitEffects(position);
        Destroy(gameObject);
    }
}