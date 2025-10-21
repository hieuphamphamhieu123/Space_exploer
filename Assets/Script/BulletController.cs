using UnityEngine;

/// <summary>
/// Script cho đạn - gây damage cho enemy
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("⚔️ Bullet Settings")]
    public float speed = 10f;
    public int damage = 25;
    public float lifetime = 3f; // Tự hủy sau 3 giây

    void Start()
    {
        // Tự destroy sau lifetime giây
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Di chuyển đạn
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Bắn trúng enemy
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            Debug.Log($"💥 Đạn bắn trúng {other.gameObject.name}!");
            enemy.TakeDamage(damage);
            Destroy(gameObject); // Destroy đạn
            return;
        }

        // Bắn trúng asteroid
        Asteroid asteroid = other.GetComponent<Asteroid>();
        if (asteroid != null)
        {
            // Có thể thêm logic phá hủy asteroid ở đây
            Destroy(gameObject);
            return;
        }

        // Đạn chạm object khác (không phải player)
        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}