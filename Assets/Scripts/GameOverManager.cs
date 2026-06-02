using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("Arayüz (UI) Baðlantýlarý")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    void Start()
    {
        // Singleton Güvenlik Duvarý
        if (finalScoreText == null || highScoreText == null)
        {
            Debug.LogWarning("GameOverManager: UI Text referanslarý eksik! Lütfen Inspector'dan atamalarý yap.");
            return;
        }

        
        if (GameManager.Instance != null)
        {
            
            finalScoreText.text = "Skorun: " + GameManager.Instance.score.ToString();
            highScoreText.text = "En Yüksek: " + GameManager.Instance.highScore.ToString();
        }
    }

  

    public void ClickRestart()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    public void ClickMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
    }
}