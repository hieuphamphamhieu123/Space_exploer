using UnityEngine;

/// <summary>
/// Gắn vào các ship có thể "chiếm đoạt" trong map
/// Player chạm vào + đủ điểm → Chuyển control sang ship này
/// </summary>
public class ShipTakeover : MonoBehaviour
{
    [Header("🚀 Takeover Settings")]
    [Tooltip("Số điểm cần để chiếm ship này")]
    public int requiredScore = 100;

    [Tooltip("Tên ship")]
    public string shipName = "Station 1";

    [Tooltip("Tốc độ di chuyển của ship này")]
    public float moveSpeed = 5f;

    [Header("✨ Visual Effects")]
    public bool enableFloating = true;
    public float floatSpeed = 1f;
    public float floatHeight = 0.3f;

    [Header("🎨 UI Display (Optional)")]
    public GameObject priceUI; // UI hiển thị giá (nếu có)

    private Vector3 startPosition;
    private bool isTaken = false;
    private ShipController shipController;

    void Start()
    {
        startPosition = transform.position;

        // Đảm bảo có Collider2D trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogWarning($"⚠️ ShipTakeover '{shipName}': Thiếu Collider2D!");
        }

        // Disable ShipController ban đầu (ship này chưa được điều khiển)
        shipController = GetComponent<ShipController>();
        if (shipController != null)
        {
            shipController.enabled = false; // Tắt control
        }
        else
        {
            Debug.LogWarning($"⚠️ ShipTakeover '{shipName}': Thiếu ShipController! Thêm component này.");
        }

        // Đảm bảo ship này KHÔNG có tag Player (chưa là player)
        if (gameObject.tag == "Player")
        {
            gameObject.tag = "Untagged";
        }
    }

    void Update()
    {
        // Hiệu ứng floating nếu chưa bị chiếm
        if (enableFloating && !isTaken)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Debug log
        Debug.Log($"🔵 ShipTakeover '{shipName}': Touched by {other.gameObject.name} (Tag: {other.tag})");

        if (other.CompareTag("Player") && !isTaken)
        {
            TryTakeover(other.gameObject);
        }
    }

    void TryTakeover(GameObject currentPlayer)
    {
        // Kiểm tra GameManager
        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager không tìm thấy!");
            return;
        }

        int currentScore = GameManager.Instance.score;

        // Kiểm tra đủ điểm chưa
        if (currentScore < requiredScore)
        {
            int needed = requiredScore - currentScore;
            Debug.Log($"❌ Chưa đủ điểm cho '{shipName}'! Cần thêm {needed} điểm. (Hiện tại: {currentScore}/{requiredScore})");
            FlashRed();
            return;
        }

        // ✅ ĐỦ ĐIỂM - Chiếm ship!
        Debug.Log($"✅ Đã chiếm '{shipName}'! Chuyển control...");
        isTaken = true;

        PerformTakeover(currentPlayer);
    }

    void PerformTakeover(GameObject oldPlayer)
    {
        // 1. Tắt control của player cũ
        ShipController oldController = oldPlayer.GetComponent<ShipController>();
        if (oldController != null)
        {
            oldController.enabled = false;
            Debug.Log("🔴 Đã tắt control của ship cũ");
        }

        // 2. Bật control của ship mới (this)
        if (shipController != null)
        {
            shipController.enabled = true;
            
            Debug.Log($"🟢 Đã bật control của {shipName}");
        }

        // 3. Đổi tag: Ship cũ không còn là Player, ship mới là Player
        oldPlayer.tag = "Untagged";
        gameObject.tag = "Player";

        // 4. Update camera để follow ship mới
        CameraFollow cam = Camera.main?.GetComponent<CameraFollow>();
        if (cam != null)
        {
            cam.target = transform;
            Debug.Log($"📷 Camera follow {shipName}");
        }

        // 5. Tắt floating effect
        enableFloating = false;

        // 6. Ẩn UI giá (nếu có)
        if (priceUI != null)
        {
            priceUI.SetActive(false);
        }

        // 7. Có thể xóa ship cũ hoặc để nguyên
        // Destroy(oldPlayer); // Uncomment nếu muốn xóa ship cũ

        Debug.Log($"🎉 Giờ bạn điều khiển {shipName}!");
    }

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
        yield return new WaitForSeconds(0.2f);
        sr.color = original;
    }

    // Visual trong Scene view
    void OnDrawGizmos()
    {
        if (isTaken) return;

        // Vẽ vòng tròn trigger
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1f);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 3f,
            $"{shipName}\n💰 {requiredScore} điểm");
#endif
    }
}