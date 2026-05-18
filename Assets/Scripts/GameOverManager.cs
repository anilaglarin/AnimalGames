using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    // Bu satýrlar Inspector'da kutucuklarý oluþturur
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            finalScoreText.text = "Skorun: " + GameManager.Instance.score;
            highScoreText.text = "En Yüksek: " + GameManager.Instance.highScore;
        }
    }

    public void ClickRestart()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }

    public void ClickMainMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.GoToMainMenu();
    }
}