using UnityEngine;

/// <summary>
/// Camera theo dõi target (spaceship) một cách mượt mà
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [Tooltip("Object mà camera sẽ theo (kéo spaceship vào đây)")]
    public Transform target; // Spaceship

    [Header("Follow Speed")]
    [Tooltip("Tốc độ camera follow (càng cao càng nhanh). Giá trị từ 1-10")]
    [Range(1f, 10f)]
    public float smoothSpeed = 5f; // Độ mượt khi follow

    [Header("Offset")]
    [Tooltip("Khoảng cách offset từ target (X, Y, Z)")]
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Khoảng cách từ ship

    [Header("Boundaries (Optional)")]
    [Tooltip("Giới hạn camera không đi quá xa (bỏ trống nếu không cần)")]
    public bool useBoundaries = false; // Có dùng giới hạn không
    public float minX = -50f;
    public float maxX = 50f;
    public float minY = -50f;
    public float maxY = 50f;

    void LateUpdate()
    {
        // Kiểm tra có target không
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned!");
            return;
        }

        // Vị trí mục tiêu (target position + offset)
        Vector3 desiredPosition = target.position + offset;

        // Giới hạn camera trong boundaries (nếu bật)
        if (useBoundaries)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        // Smooth follow
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Cập nhật vị trí camera
        transform.position = smoothedPosition;
    }
}