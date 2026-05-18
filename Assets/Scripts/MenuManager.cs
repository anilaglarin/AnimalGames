using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;

    void Start()
    {
        // Kayýtlý rekoru göster
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
        {
            highScoreText.text = "En Yüksek Skor: " + highScore;
        }
    }

    public void PlayGame()
    {
        // "GameScene" senin oyun sahnenin adý olmalý
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan çýkýldý!"); // Editörde çalýþmaz, mobilde/exe'de çalýþýr
        Application.Quit();
    }
}