using UnityEngine;

/// <summary>
/// AI di chuyển cho enemy
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    [Header("🎯 Movement Type")]
    public MovementType movementType = MovementType.PatrolHorizontal;

    public enum MovementType
    {
        Static,              // Đứng yên
        PatrolHorizontal,    // Tuần tra trái phải
        PatrolVertical,      // Tuần tra lên xuống
        CircleAround,        // Bay vòng tròn
        ChasePlayer,         // Đuổi theo player
        Random              // Di chuyển ngẫu nhiên
    }

    [Header("⚙️ Movement Settings")]
    public float moveSpeed = 3f;
    public float patrolDistance = 5f; // Khoảng cách tuần tra

    [Header("🎯 Chase Settings (for ChasePlayer)")]
    public float chaseRange = 10f; // Khoảng cách phát hiện player
    public float stopDistance = 2f; // Dừng lại khi gần player

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Transform playerTransform;
    private float moveTimer = 0f;

    void Start()
    {
        startPosition = transform.position;

        // Tìm player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Setup target position cho patrol
        if (movementType == MovementType.PatrolHorizontal)
        {
            targetPosition = startPosition + Vector3.right * patrolDistance;
        }
        else if (movementType == MovementType.PatrolVertical)
        {
            targetPosition = startPosition + Vector3.up * patrolDistance;
        }
    }

    void Update()
    {
        switch (movementType)
        {
            case MovementType.Static:
                // Không di chuyển
                break;

            case MovementType.PatrolHorizontal:
                PatrolHorizontal();
                break;

            case MovementType.PatrolVertical:
                PatrolVertical();
                break;

            case MovementType.CircleAround:
                CircleMovement();
                break;

            case MovementType.ChasePlayer:
                ChasePlayer();
                break;

            case MovementType.Random:
                RandomMovement();
                break;
        }
    }

    void PatrolHorizontal()
    {
        // Di chuyển về target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Đến target thì đổi hướng
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (targetPosition.x > startPosition.x)
            {
                targetPosition = startPosition - Vector3.right * patrolDistance;
            }
            else
            {
                targetPosition = startPosition + Vector3.right * patrolDistance;
            }
        }
    }

    void PatrolVertical()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (targetPosition.y > startPosition.y)
            {
                targetPosition = startPosition - Vector3.up * patrolDistance;
            }
            else
            {
                targetPosition = startPosition + Vector3.up * patrolDistance;
            }
        }
    }

    void CircleMovement()
    {
        moveTimer += Time.deltaTime;
        float x = startPosition.x + Mathf.Cos(moveTimer * moveSpeed) * patrolDistance;
        float y = startPosition.y + Mathf.Sin(moveTimer * moveSpeed) * patrolDistance;
        transform.position = new Vector3(x, y, transform.position.z);
    }

    void ChasePlayer()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Trong phạm vi chase
        if (distanceToPlayer <= chaseRange && distanceToPlayer > stopDistance)
        {
            // Di chuyển về phía player
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    void RandomMovement()
    {
        moveTimer += Time.deltaTime;

        // Đổi hướng mỗi 2 giây
        if (moveTimer >= 2f)
        {
            moveTimer = 0f;
            targetPosition = startPosition + new Vector3(
                Random.Range(-patrolDistance, patrolDistance),
                Random.Range(-patrolDistance, patrolDistance),
                0
            );
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // Vẽ patrol range trong Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (movementType == MovementType.ChasePlayer)
        {
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }
        else if (movementType == MovementType.PatrolHorizontal)
        {
            Vector3 start = Application.isPlaying ? startPosition : transform.position;
            Gizmos.DrawLine(start - Vector3.right * patrolDistance, start + Vector3.right * patrolDistance);
        }
        else if (movementType == MovementType.PatrolVertical)
        {
            Vector3 start = Application.isPlaying ? startPosition : transform.position;
            Gizmos.DrawLine(start - Vector3.up * patrolDistance, start + Vector3.up * patrolDistance);
        }
    }
}