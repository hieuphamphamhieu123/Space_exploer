using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Va chạm với Star
        if (other.CompareTag("Star"))
        {
            Debug.Log("Thu thập star!");
            Destroy(other.gameObject);
        }

        // Va chạm với Asteroid
        if (other.CompareTag("Finish"))
        {
            Debug.Log("Chạm asteroid!");
            Destroy(other.gameObject);
        }
    }
}