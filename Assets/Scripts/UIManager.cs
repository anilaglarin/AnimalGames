using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Arayüz Metinleri")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private TextMeshProUGUI levelText;

    void Awake()
    {
        // Sahnede birden fazla UI Manager olmasýný engelliyoruz
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void UpdateScoreUI()
    {
        if (GameManager.Instance == null) return;

        if (scoreText != null) scoreText.text = "Skor: " + GameManager.Instance.score.ToString();
        if (highScoreText != null) highScoreText.text = "Max: " + GameManager.Instance.highScore.ToString();
        if (targetText != null) targetText.text = "Hedef: " + GameManager.Instance.targetScore.ToString();
        if (levelText != null) levelText.text = "Level: " + GameManager.Instance.level.ToString();
    }

    public void UpdateTimerUI(float time)
    {
        if (timerText != null)
        {
            timerText.text = "Süre: " + Mathf.CeilToInt(time).ToString() + "s";

            // Süre azaldýðýnda oyuncuyu uyaralým
            timerText.color = time <= 5f ? Color.red : Color.white;
        }
    }

    public void ShowLevelUp(int level)
    {
        Debug.Log("Yeni levele geçildi: " + level);
        // Ýleride buraya UI animasyonlarý veya pop-up eklenebilir
        UpdateScoreUI();
    }
}