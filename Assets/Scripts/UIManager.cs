using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI timerText; // Zamaný gösteren yazý
    public TextMeshProUGUI targetText; // Hedef skoru gösteren yazý
    public TextMeshProUGUI levelText; // Yeni: Kenarda level yazacak olan metin

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void UpdateScoreUI()
    {
        if (GameManager.Instance == null) return;

        if (scoreText != null) scoreText.text = "Skor: " + GameManager.Instance.score;
        if (highScoreText != null) highScoreText.text = "Max: " + GameManager.Instance.highScore;
        if (targetText != null) targetText.text = "Hedef: " + GameManager.Instance.targetScore;

        // --- LEVEL YAZISINI BURADA GÜNCELLÝYORUZ ---
        if (levelText != null) levelText.text = "Level: " + GameManager.Instance.level;
    }

    public void UpdateTimerUI(float time)
    {
        if (timerText != null)
        {
            // Zamaný saniye cinsinden yazdýrýr
            timerText.text = "Süre: " + Mathf.CeilToInt(time).ToString() + "s";

            // Süre 5 saniyenin altýna düþerse rengi kýrmýzý yap (Dikkat çeksin!)
            if (time <= 5f) timerText.color = Color.red;
            else timerText.color = Color.white;
        }
    }

    public void ShowLevelUp(int level)
    {
        Debug.Log("YENÝ LEVEL: " + level);
        // Buraya ileride ekranda "Level Up!" animasyonunu tetikleyecek kod gelebilir
        UpdateScoreUI();
    }
}