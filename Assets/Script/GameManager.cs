using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
        if (Instance == null)
        {
            Instance = this;
            // KHÔNG dùng DontDestroyOnLoad để score reset mỗi game
            // DontDestroyOnLoad(gameObject);
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

    public void AddScore(int points)
    {
        if (isGameOver) return;

        score += points;
        UpdateUI();
        Debug.Log($"⭐ +{points} điểm! Tổng: {score}");
    }

    /// <summary>
    /// Game Over - Lưu score và chuyển End Game scene
    /// </summary>
    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log($"💀 GAME OVER! Final Score: {score}");

        // Lưu score vào PlayerPrefs
        PlayerPrefs.SetInt("LastScore", score);

        // Kiểm tra high score
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            Debug.Log($"🎉 NEW HIGH SCORE: {score}!");
        }

        PlayerPrefs.Save();

        // Đợi 2 giây rồi chuyển scene
        Invoke("LoadEndGameScene", 2f);
    }

    /// <summary>
    /// Load End Game scene
    /// </summary>
    void LoadEndGameScene()
    {
        SceneManager.LoadScene("EndGame"); // Tên scene End Game
    }

    /// <summary>
    /// Restart game ngay (không qua End Game scene)
    /// </summary>
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}