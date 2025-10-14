using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    // ===== MOVEMENT SETTINGS =====
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // Tốc độ di chuyển
    
    // ===== SHOOTING SETTINGS =====
    [Header("Shooting Settings")]
    [SerializeField] private GameObject laserPrefab; // Prefab của laser
    [SerializeField] private Transform firePoint; // Vị trí bắn laser
    [SerializeField] private float fireRate = 0.5f; // Thời gian giữa các lần bắn (giây)
    [SerializeField] private bool autoShoot = true; // Bắn tự động
    
    // ===== PRIVATE VARIABLES =====
    private Vector2 movement;
    private float nextFireTime = 0f; // Thời gian được phép bắn tiếp theo
    
    // ===== BOUNDARY SETTINGS (Optional) =====
    [Header("Boundary Settings (Optional)")]
    [SerializeField] private bool useBoundary = true;
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float minY = -4f;
    [SerializeField] private float maxY = 4f;

    // ===== INITIALIZATION =====
    void Start()
    {
        Debug.Log("ShipController: Khởi tạo thành công!");
        
        if (autoShoot)
        {
            Debug.Log("ShipController: Chế độ bắn tự động đã BẬT");
        }
    }

    // ===== UPDATE - XỬ LÝ INPUT & BẮN =====
    void Update()
    {
        // Đọc input từ phím mũi tên
        movement = Vector2.zero;
        
        if (Keyboard.current != null)
        {
            if (Keyboard.current.upArrowKey.isPressed)
                movement.y += 1f;
            if (Keyboard.current.downArrowKey.isPressed)
                movement.y -= 1f;
            if (Keyboard.current.leftArrowKey.isPressed)
                movement.x -= 1f;
            if (Keyboard.current.rightArrowKey.isPressed)
                movement.x += 1f;
        }
        
        // Di chuyển spaceship
        MovePlayer();
        
        // Xử lý bắn laser tự động
        HandleAutoShooting();
    }

    // ===== MOVEMENT FUNCTION =====
    void MovePlayer()
    {
        if (movement.magnitude > 0)
        {
            // Tính toán vị trí mới
            Vector3 newPosition = transform.position + new Vector3(movement.x, movement.y, 0f).normalized * moveSpeed * Time.deltaTime;
            
            // Giới hạn vị trí trong boundary
            if (useBoundary)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
            }
            
            // Áp dụng vị trí mới
            transform.position = newPosition;
        }
    }

    // ===== AUTO SHOOTING FUNCTION =====
    void HandleAutoShooting()
    {
        // Bắn tự động theo khoảng thời gian fireRate
        if (autoShoot && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    // ===== SHOOT LASER =====
   void Shoot()
{
    if (laserPrefab != null)
    {
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        
        // Tạo laser
        GameObject bullet = Instantiate(laserPrefab, spawnPosition, Quaternion.identity);
        
        // GÁN firing_ship cho Projectile script
        Projectile projectile = bullet.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.firing_ship = gameObject; // Gán ship hiện tại
        }
    }
    else
    {
        Debug.LogWarning("ShipController: Laser Prefab chưa được gán!");
    }
}
    // ===== VISUALIZATION =====
    void OnDrawGizmosSelected()
    {
        if (useBoundary)
        {
            Gizmos.color = Color.yellow;
            Vector3 topLeft = new Vector3(minX, maxY, 0);
            Vector3 topRight = new Vector3(maxX, maxY, 0);
            Vector3 bottomLeft = new Vector3(minX, minY, 0);
            Vector3 bottomRight = new Vector3(maxX, minY, 0);
            
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
    
    // ===== PUBLIC METHODS =====
    
    public void SetAutoShoot(bool value)
    {
        autoShoot = value;
        Debug.Log("ShipController: Bắn tự động = " + value);
    }
    
    public void SetFireRate(float newFireRate)
    {
        fireRate = newFireRate;
        Debug.Log("ShipController: Fire rate mới = " + fireRate);
    }
    
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
        Debug.Log("ShipController: Move speed mới = " + moveSpeed);
    }
}