using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Asteroid Settings")]
    public bool destroyOnCollision = true;
    public bool causeGameOver = true;

    [Header("Movement")]
    public float moveSpeed = 2f;
    private Vector2 moveDirection;

    void Start()
    {
        moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        Debug.Log("Asteroid: Started with direction: " + moveDirection);
    }

    void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    // Thử phương pháp 1: Collision (không trigger)
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Asteroid: OnCollisionEnter2D! Hit: " + collision.gameObject.name);
        Debug.Log("Asteroid: Object tag: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Asteroid: PLAYER HIT! Calling HitPlayer()...");
            HitPlayer();
        }
    }

    // Thử phương pháp 2: Trigger (nếu collision không work)
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Asteroid: OnTriggerEnter2D! Hit: " + other.gameObject.name);
        Debug.Log("Asteroid: Object tag: " + other.tag);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Asteroid: PLAYER HIT (Trigger)! Calling HitPlayer()...");
            HitPlayer();
        }
    }

    void HitPlayer()
    {
        Debug.Log("Asteroid: HitPlayer() called!");

        if (causeGameOver)
        {
            if (GameManager.Instance != null)
            {
                Debug.Log("Asteroid: Calling GameOver...");
                GameManager.Instance.GameOver();
            }
            else
            {
                Debug.LogError("Asteroid: GameManager.Instance is NULL!");
            }
        }

        if (destroyOnCollision)
        {
            Destroy(gameObject);
        }
    }
}