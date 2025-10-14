using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Vector2 moveDirection;

    void Start()
    {
        // Di chuyển ngẫu nhiên
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        moveDirection = new Vector2(randomX, randomY).normalized;
    }

    void Update()
    {
        // Di chuyển asteroid
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Va chạm với spaceship
        if (other.CompareTag("Player"))
        {
            // Trừ điểm (cần có ScoreManager)
            // ScoreManager.instance.AddScore(-10);

            // Hủy asteroid
            Destroy(gameObject);
        }
    }
}