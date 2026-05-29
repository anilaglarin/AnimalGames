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

    [Header("Zorluk Ayarlarý")]
    public float currentFireRate = 0.5f; 

    [Header("Ses Ayarlarý (Yeni)")]
    public AudioClip backgroundMusic; // Müfettiþ paneline (Inspector) atayacaðýn müzik
    public AudioClip explosionSFX;    // Müfettiþ paneline atayacaðýn pop sesi
    private AudioSource audioSource;   // Sesleri fiziksel olarak çalacak bileþen

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }

        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // --- SES SÝSTEMÝNÝ BAÞLATMA ---
        // GameManager objesine otomatik olarak bir AudioSource bileþeni takýyoruz
        audioSource = gameObject.AddComponent<AudioSource>();
        PlayBackgroundMusic();
    }

    void Update()
    {
        if (isGameOver) return;

        if (SceneManager.GetActiveScene().name != "GameScene") return;

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

    // --- YENÝ SES FONKSÝYONLARI ---
    void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && audioSource != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;      // Müzik bittikten sonra sürekli baþa sarar
            audioSource.volume = 0.25f;   // Müziðin ses seviyesini %25 yapýyoruz
            audioSource.Play();
        }
    }

    public void PlayExplosionSound()
    {
        if (explosionSFX != null && audioSource != null)
        {
            // PlayOneShot, arka plan müziðini kesmeden üstüne bu ses efektini anlýk çalar
            audioSource.PlayOneShot(explosionSFX, 0.6f); 
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

        currentFireRate = Mathf.Max(0.2f, currentFireRate - 0.05f);

        if (UIManager.Instance != null) UIManager.Instance.ShowLevelUp(level);

        // --- ENGELÝ AKTÝFLEÞTÝREN KOD ---
        GameObject obstacle = GameObject.Find("LevelObstacle");
        
        if (obstacle == null)
        {
            GameObject basket = GameObject.Find("Basket");
            if (basket != null)
            {
                Transform obstacleTransform = basket.transform.Find("LevelObstacle");
                if (obstacleTransform != null)
                {
                    obstacle = obstacleTransform.gameObject;
                }
            }
        }

        if (obstacle != null)
        {
            obstacle.SetActive(true);
            Debug.Log("Engel baþarýyla devreye sokuldu!");
        }
        else
        {
            Debug.LogWarning("LevelObstacle isimli obje sahnede bulunamadý! Adýný kontrol et.");
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameOver");
    }

    public void RestartGame()
    {
        score = 0;
        level = 1;
        targetScore = 50;
        timeRemaining = 30f;
        currentFireRate = 0.5f;
        isGameOver = false;
        SceneManager.LoadScene("GameScene");
        
        // Oyun yeniden baþlayýnca müziði de en baþtan tetikliyoruz
        PlayBackgroundMusic(); 
    }

    public void GoToMainMenu()
    {
        score = 0;
        level = 1;
        targetScore = 50;
        timeRemaining = 30f;
        currentFireRate = 0.5f;
        isGameOver = false;
        SceneManager.LoadScene("MainMenu");
    }
}