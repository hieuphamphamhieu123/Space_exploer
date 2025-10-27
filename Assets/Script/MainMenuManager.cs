using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý Main Menu - Play, Instructions, Quit
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("🎮 UI Panels")]
    [Tooltip("Panel instructions (để ẩn/hiện)")]
    public GameObject instructionsPanel; // ⭐ KHAI BÁO BIẾN

    [Header("📺 Scene Settings")]
    [Tooltip("Tên scene gameplay")]
    public string gameplaySceneName = "GamePlayScene";

    [Header("🔊 Audio")]
    public AudioClip buttonClickSound;

    // ⭐ KHAI BÁO BIẾN PRIVATE
    private bool isInstructionsShowing = false;

    void Awake()
    {
        // Ẩn panel ngay từ đầu
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
            isInstructionsShowing = false;
            Debug.Log("📖 Instructions panel hidden in Awake");
        }
    }

    void Start()
    {
        Debug.Log("🏠 Main Menu loaded!");

        // Đảm bảo panel ẩn
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
            isInstructionsShowing = false;
        }
        else
        {
            Debug.LogWarning("⚠️ Instructions Panel chưa được gán trong Inspector!");
        }
    }

    /// <summary>
    /// Chơi game - Load gameplay scene
    /// </summary>
    public void PlayGame()
    {
        Debug.Log($"▶️ Loading {gameplaySceneName}...");
        PlayButtonSound();

        SceneManager.LoadScene(gameplaySceneName);
    }

    /// <summary>
    /// Toggle Instructions Panel (ẩn/hiện)
    /// </summary>
    public void ToggleInstructions()
    {
        if (instructionsPanel == null)
        {
            Debug.LogError("❌ Instructions Panel chưa được gán trong Inspector!");
            return;
        }

        // Đảo trạng thái
        isInstructionsShowing = !isInstructionsShowing;
        instructionsPanel.SetActive(isInstructionsShowing);

        PlayButtonSound();

        if (isInstructionsShowing)
        {
            Debug.Log("📖 Showing instructions...");
        }
        else
        {
            Debug.Log("📖 Hiding instructions...");
        }
    }

    /// <summary>
    /// Hiện Instructions
    /// </summary>
    public void ShowInstructions()
    {
        if (instructionsPanel == null)
        {
            Debug.LogError("❌ Instructions Panel chưa được gán!");
            return;
        }

        instructionsPanel.SetActive(true);
        isInstructionsShowing = true;
        PlayButtonSound();
        Debug.Log("📖 Instructions shown");
    }

    /// <summary>
    /// Ẩn Instructions
    /// </summary>
    public void HideInstructions()
    {
        if (instructionsPanel == null)
        {
            Debug.LogError("❌ Instructions Panel chưa được gán!");
            return;
        }

        instructionsPanel.SetActive(false);
        isInstructionsShowing = false;
        PlayButtonSound();
        Debug.Log("📖 Instructions hidden");
    }

    /// <summary>
    /// Quit game
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("👋 Quitting game...");
        PlayButtonSound();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Play button sound
    /// </summary>
    void PlayButtonSound()
    {
        if (buttonClickSound != null)
        {
            AudioSource.PlayClipAtPoint(buttonClickSound, Camera.main.transform.position);
        }
    }
}