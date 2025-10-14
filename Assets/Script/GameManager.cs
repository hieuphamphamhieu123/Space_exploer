using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Quản lý toàn bộ game: điểm, game over
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("📊 UI")]
    public TextMeshProUGUI scoreText;

    [Header("⚙️ Game Settings")]
    public int score = 0;
    public int pointsPerStar = 10;

    private bool isGameOver = false;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    /// <summary>
    /// Cộng điểm khi thu thập star
    /// </summary>
    public void AddScore(int points)
    {
        if (isGameOver) return;

        score += points;
        UpdateUI();
        Debug.Log($"⭐ +{points} điểm! Tổng: {score}");
    }

    /// <summary>
    /// Game Over
    /// </summary>
    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log($"💀 GAME OVER! Điểm cuối: {score}");

        Invoke("RestartGame", 2f);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}