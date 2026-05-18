using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI timerText; // Yeni: Zamaný gösteren yazý
    public TextMeshProUGUI targetText; // Yeni: Hedef skoru gösteren yazý

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
        if (scoreText != null) scoreText.text = "Skor: " + GameManager.Instance.score;
        if (highScoreText != null) highScoreText.text = "Max: " + GameManager.Instance.highScore;
        if (targetText != null) targetText.text = "Hedef: " + GameManager.Instance.targetScore;
    }

    public void UpdateTimerUI(float time)
    {
        if (timerText != null)
        {
            // Zamaný 00:00 formatýnda yazdýrýr
            timerText.text = "Süre: " + Mathf.CeilToInt(time).ToString() + "s";
            
            // Süre 5 saniyenin altýna düþerse rengi kýrmýzý yap (Dikkat çeksin!)
            if (time <= 5f) timerText.color = Color.red;
            else timerText.color = Color.white;
        }
    }

    public void ShowLevelUp(int level)
    {
        Debug.Log("YENÝ LEVEL: " + level);
        // Buraya ekranda "Level Up!" yazdýran bir animasyon gelebilir
        UpdateScoreUI(); 
    }
}