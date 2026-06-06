using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Skor Ayarlarý")]
    public int score { get; private set; } = 0;
    public int highScore { get; private set; } = 0;
    public int level { get; private set; } = 1;
    public int targetScore { get; private set; } = 50;
    public GameObject obstacle;
    [Header("Zaman Ayarlarý")]
    public float timeRemaining { get; private set; } = 20f;
    public bool isGameOver { get; private set; } = false;

    [Header("Zorluk Ayarlarý")]
    public float currentFireRate { get; private set; } = 0.5f;

    [Header("Ses Ayarlarý")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip explosionSFX;

    private AudioSource audioSource;
    private bool isGameActive = false; 

    void Awake()
    {
        // Singleton Pattern // Bellekte tek kopyayý garantiler.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // Ses sistemini dinamik olarak baþlatma
        audioSource = gameObject.AddComponent<AudioSource>();
        PlayBackgroundMusic();
    }

    private void OnDestroy()
    {
        //bellek sýzýntýsýný önlemek için 
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Sadece GameScene içindeysek oyun döngüsünü (Update) aktif et
        isGameActive = (scene.name == "GameScene");
    }

    void Update()
    {
        
        if (isGameOver || !isGameActive) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateTimerUI(timeRemaining);
            }
        }
        else
        {
            GameOver();
        }
    }

    // --- SES FONKSÝYONLARI ---

    private void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && audioSource != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.volume = 0.25f;
            audioSource.Play();
        }
    }

    public void PlayExplosionSound()
    {
        if (explosionSFX != null && audioSource != null)
        {
            // Arka plan müziðini kesmeden efekti anlýk çalar
            audioSource.PlayOneShot(explosionSFX, 0.6f);
        }
    }

    // --- OYUN MEKANÝKLERÝ ---

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

    private void LevelUp()
    {
        level++;
        targetScore += 100;
        timeRemaining = Mathf.Max(10f, 30f - (level * 2f)); 
        currentFireRate = Mathf.Max(0.2f, currentFireRate - 0.05f);

        if (UIManager.Instance != null) UIManager.Instance.ShowLevelUp(level);

        ActivateLevelObstacle();
    }

    private void ActivateLevelObstacle()
    {
        //engel arama 
        //GameObject obstacle = GameObject.Find("LevelObstacle");

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
            Debug.LogWarning("LevelObstacle isimli obje sahnede bulunamadý! Adýný kontrol edebilir misin?");
        }
    }

    // --- SAHNE VE DURUM KONTROLLERÝ ---

    public void GameOver()
    {
        isGameOver = true;
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameOver");
    }

    public void RestartGame()
    {
        ResetGameState();
        SceneManager.LoadScene("GameScene");
        PlayBackgroundMusic();
    }

    public void GoToMainMenu()
    {
        ResetGameState();
        SceneManager.LoadScene("MainMenu");
    }

    private void ResetGameState()
    {
        // Kod tekrarýný (DRY Prensibi) önlemek için sýfýrlama iþlemlerini tek yerde topladýk
        score = 0;
        level = 1;
        targetScore = 50;
        timeRemaining = 30f;
        currentFireRate = 0.5f;
        isGameOver = false;
    }
}