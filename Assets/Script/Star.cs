using UnityEngine;

/// <summary>
/// Script cho star - thu thập để cộng điểm
/// </summary>
public class Star : MonoBehaviour
{
    public int pointValue = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Cộng điểm
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(pointValue);
            }

            // Destroy star
            Destroy(gameObject);
        }
    }
}