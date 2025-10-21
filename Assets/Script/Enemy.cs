using UnityEngine;

/// <summary>
/// Script quản lý kẻ địch: health, damage, death
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("⚔️ Enemy Stats")]
    [Tooltip("Máu của enemy")]
    public int maxHealth = 100;
    private int currentHealth;

    [Tooltip("Điểm cho player khi tiêu diệt enemy này")]
    public int scoreValue = 50;

    [Tooltip("Damage gây ra khi va chạm với player")]
    public int collisionDamage = 20;

    [Header("💥 Death Effects")]
    [Tooltip("Prefab hiệu ứng nổ khi chết")]
    public GameObject deathEffect;

    [Tooltip("Âm thanh khi chết")]
    public AudioClip deathSound;

    [Header("🎁 Drops (Optional)")]
    [Tooltip("Item rơi khi chết (star, powerup...)")]
    public GameObject dropItem;
    [Tooltip("% rơi item (0-100)")]
    [Range(0, 100)]
    public float dropChance = 50f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Nhận damage từ bullet hoặc player
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} nhận {damage} damage! Health: {currentHealth}/{maxHealth}");

        // Hiệu ứng flash đỏ
        FlashRed();

        // Kiểm tra chết chưa
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Enemy chết
    /// </summary>
    void Die()
    {
        Debug.Log($"💀 {gameObject.name} đã bị tiêu diệt!");

        // Cộng điểm cho player
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        // Spawn hiệu ứng nổ
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Phát âm thanh
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        // Rơi item
        TryDropItem();

        // Destroy enemy
        Destroy(gameObject);
    }

    /// <summary>
    /// Thử rơi item
    /// </summary>
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

    /// <summary>
    /// Va chạm với player
    /// </summary>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"💥 Enemy đâm vào Player!");

            // CÁCH 1: Game Over ngay lập tức
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }

            // Enemy cũng chết khi đâm vào player
            Die();
        }
    }
    /// <summary>
    /// Hiệu ứng flash đỏ khi nhận damage
    /// </summary>
    void FlashRed()
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
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = original;
    }

    /// <summary>
    /// Hiển thị health bar trong Scene view (Debug)
    /// </summary>
    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            float healthPercent = (float)currentHealth / maxHealth;
            UnityEditor.Handles.Label(transform.position + Vector3.up,
                $"HP: {currentHealth}/{maxHealth}");
        }
#endif
    }
}