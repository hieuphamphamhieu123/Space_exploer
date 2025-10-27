using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Quản lý End Game Scene - Hiển thị score, restart, quit
/// </summary>
public class EndGameManager : MonoBehaviour
{
    [Header("📊 UI References")]
    [Tooltip("Text hiển thị điểm cuối cùng")]
    public TextMeshProUGUI finalScoreText;

    [Tooltip("Text hiển thị high score")]
    public TextMeshProUGUI highScoreText;

    [Tooltip("Text thông báo (Game Over, Victory...)")]
    public TextMeshProUGUI messageText;

    [Header("🎨 Visual Effects")]
    [Tooltip("Panel chính (để fade in)")]
    public CanvasGroup mainPanel;

    [Tooltip("Tốc độ fade in")]
    public float fadeSpeed = 1f;

    [Header("🔊 Audio")]
    [Tooltip("Âm thanh game over")]
    public AudioClip gameOverSound;

    [Tooltip("Âm thanh khi new high score")]
    public AudioClip newHighScoreSound;

    private int finalScore = 0;
    private int highScore = 0;
    private bool isNewHighScore = false;

    void Start()
    {
        // Lấy score từ GameManager hoặc PlayerPrefs
        LoadScore();

        // Hiển thị UI
        DisplayScore();

        // Play sound
        PlayGameOverSound();

        // Fade in effect
        if (mainPanel != null)
        {
            StartCoroutine(FadeIn());
        }
    }

    /// <summary>
    /// Load score từ PlayerPrefs hoặc GameManager
    /// </summary>
    void LoadScore()
    {
        // Lấy score từ PlayerPrefs (đã được lưu từ GameManager)
        finalScore = PlayerPrefs.GetInt("LastScore", 0);
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // Kiểm tra new high score
        if (finalScore > highScore)
        {
            highScore = finalScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            isNewHighScore = true;

            Debug.Log($"🎉 NEW HIGH SCORE: {highScore}!");
        }

        Debug.Log($"📊 Final Score: {finalScore}, High Score: {highScore}");
    }

    /// <summary>
    /// Hiển thị score lên UI
    /// </summary>
    void DisplayScore()
    {
        // Final score
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Score: {finalScore}";

            // Animate số (optional)
            StartCoroutine(AnimateScore(finalScoreText, 0, finalScore, 1f));
        }

        // High score
        if (highScoreText != null)
        {
            if (isNewHighScore)
            {
                highScoreText.text = $"🎉 NEW HIGH SCORE! 🎉\n{highScore}";
                highScoreText.color = Color.yellow;

                // Animate scale
                StartCoroutine(PulseText(highScoreText));
            }
            else
            {
                highScoreText.text = $"High Score: {highScore}";
            }
        }

        // Message
        if (messageText != null)
        {
            if (isNewHighScore)
            {
                messageText.text = "AMAZING!";
            }
            else if (finalScore > highScore / 2)
            {
                messageText.text = "GOOD JOB!";
            }
            else
            {
                messageText.text = "GAME OVER";
            }
        }
    }

    /// <summary>
    /// Play game over sound
    /// </summary>
    void PlayGameOverSound()
    {
        if (isNewHighScore && newHighScoreSound != null)
        {
            AudioSource.PlayClipAtPoint(newHighScoreSound, Camera.main.transform.position);
        }
        else if (gameOverSound != null)
        {
            AudioSource.PlayClipAtPoint(gameOverSound, Camera.main.transform.position);
        }
    }

    #region Button Functions

    /// <summary>
    /// Restart game - Chơi lại
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("🔄 Restarting game...");
        SceneManager.LoadScene("GamePlayScene"); // ✅ ĐÚNG
    }

    /// <summary>
    /// Quay về Main Menu
    /// </summary>
    public void ReturnToMainMenu()
    {
        Debug.Log("🏠 Returning to Main Menu...");

        // Load main menu scene
        SceneManager.LoadScene("MainMenu"); // Hoặc tên scene main menu của bạn
    }

    /// <summary>
    /// Quit game
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("👋 Quitting game...");

#if UNITY_EDITOR
        // Trong Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Trong build
        Application.Quit();
#endif
    }

    #endregion

    #region Visual Effects

    /// <summary>
    /// Fade in panel
    /// </summary>
    System.Collections.IEnumerator FadeIn()
    {
        mainPanel.alpha = 0f;

        while (mainPanel.alpha < 1f)
        {
            mainPanel.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        mainPanel.alpha = 1f;
    }

    /// <summary>
    /// Animate score từ 0 đến target
    /// </summary>
    System.Collections.IEnumerator AnimateScore(TextMeshProUGUI text, int start, int target, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            int current = (int)Mathf.Lerp(start, target, t);
            text.text = $"Score: {current}";
            yield return null;
        }

        text.text = $"Score: {target}";
    }

    /// <summary>
    /// Pulse text effect (phóng to thu nhỏ)
    /// </summary>
    System.Collections.IEnumerator PulseText(TextMeshProUGUI text)
    {
        Vector3 originalScale = text.transform.localScale;
        float pulseSpeed = 2f;

        while (true)
        {
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * 0.1f;
            text.transform.localScale = originalScale * scale;
            yield return null;
        }
    }

    #endregion
}