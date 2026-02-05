using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score")]
    public int score = 0;

    [Header("Counts")]
    public int goodCount = 0;
    public int badCount = 0;

    [Header("Win / Lose")]
    public int winAt = 3;
    public int loseAt = 3;
    public bool gameEnded = false;

    [Header("Player (optional)")]
    public PlayerMovement2D playerMovement; // le scritp  du mouvement

    [Header("UI (optional)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI goodText;   
    public TextMeshProUGUI badText;    
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Audio (optional)")]
    public AudioSource audioSource;
    public AudioClip goodSfx;
    public AudioClip badSfx;
    public AudioClip winSfx;
    public AudioClip loseSfx;

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (playerMovement == null) playerMovement = FindFirstObjectByType<PlayerMovement2D>();

        // Panels off au départ
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        UpdateUI();
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.R)) RestartScene();
        if (Input.GetKeyDown(KeyCode.Escape)) QuitGame();
    }

    public void CollectGood(int points)
    {
        if (gameEnded) return;

        score += points;
        goodCount++;

        PlaySfx(goodSfx);
        Debug.Log($"⚽ Bon ballon ! +{points} | Score={score} | Good={goodCount}/{winAt}");

        UpdateUI();

        if (goodCount >= winAt)
            Win();
    }

    public void CollectBad(int points)
    {
        if (gameEnded) return;

        score += points; // points peut être négatif
        badCount++;

        PlaySfx(badSfx);
        Debug.Log($"❌ Mauvais ballon ! {points} | Score={score} | Bad={badCount}/{loseAt}");

        UpdateUI();

        if (badCount >= loseAt)
            Lose();
    }

    private void Win()
    {
        if (gameEnded) return;
        gameEnded = true;

        if (playerMovement != null)
            playerMovement.canMove = false; 

        if (winPanel != null) winPanel.SetActive(true);
        PlaySfx(winSfx);

        Debug.Log("✅ VICTOIRE !");
    }

    private void Lose()
    {
        if (gameEnded) return;
        gameEnded = true;

        if (playerMovement != null)
            playerMovement.canMove = false;

        if (losePanel != null) losePanel.SetActive(true);
        PlaySfx(loseSfx);

        Debug.Log("💀 DÉFAITE !");
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";

        if (goodText != null)
            goodText.text = $"Good: {goodCount}/{winAt}";

        if (badText != null)
            badText.text = $"Bad: {badCount}/{loseAt}";
    }

    private void PlaySfx(AudioClip clip)
    {
        if (audioSource == null) return;
        if (clip == null) return;

        audioSource.PlayOneShot(clip);
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
