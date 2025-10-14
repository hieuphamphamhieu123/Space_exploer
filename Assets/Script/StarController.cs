using UnityEngine;

public class StarController : MonoBehaviour
{
    [SerializeField] private int pointValue = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Star va chạm với: " + other.name); // THÊM DÒNG NÀY

        if (other.CompareTag("Player"))
        {
            Debug.Log("Star bị thu thập!"); // THÊM DÒNG NÀY
            Destroy(gameObject);
        }
    }
}