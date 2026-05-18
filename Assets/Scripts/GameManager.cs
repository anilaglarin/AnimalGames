using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Skor Ayarlarý")]
    public int score = 0;
    public int highScore = 0;
    public int level = 1;
    public int targetScore = 50;

    [Header("Zaman Ayarlarý")]
    public float timeRemaining = 30f;
    public bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }

        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void Update()
    {
        if (isGameOver) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (UIManager.Instance != null) UIManager.Instance.UpdateTimerUI(timeRemaining);
        }
        else
        {
            GameOver();
        }
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        if (UIManager.Instance != null) UIManager.Instance.UpdateScoreUI();

        if (score >= targetScore)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        targetScore += 100;
        timeRemaining = 30f - (level * 2f);
        if (timeRemaining < 10f) timeRemaining = 10f;

        if (UIManager.Instance != null) UIManager.Instance.ShowLevelUp(level);
    }

    public void GameOver()
    {
        isGameOver = true;
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameOver");
    }

    // --- EKSÝK OLAN VE HATAYA SEBEP OLAN KISIMLAR BURASIYDI, EKLEDÝM: ---

    public void RestartGame()
    {
        score = 0;
        level = 1;
        targetScore = 50;
        timeRemaining = 30f;
        isGameOver = false;
        SceneManager.LoadScene("GameScene"); // Sahne adýnýn doðruluðundan emin ol
    }

    public void GoToMainMenu()
    {
        score = 0;
        level = 1;
        isGameOver = false;
        SceneManager.LoadScene("MainMenu"); // Sahne adýnýn doðruluðundan emin ol
    }
}