using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Arayüz")]
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Ayarlar")]
    [SerializeField] private string gameSceneName = "GameScene";

    void Start()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (highScoreText != null)
        {
            // Ekrana son rekoru yazdýrýyoruz
            highScoreText.text = "En Yüksek Skor: " + highScore.ToString();
        }
        else
        {
            Debug.LogWarning("MenuManager: UI Text atanmamýþ, Inspector'ý kontrol edebilir misin?");
        }
    }

    // --- UI Buton Fonksiyonlarý ---
    // Unity'deki butonlardan çaðrýlacaðý için public býrakýyoruz.

    public void ResetHighScore()
    {
        // Sadece HighScore anahtarýný siler
        PlayerPrefs.DeleteKey("HighScore");

        // Deðiþikliði anýnda diske kaydeder
        PlayerPrefs.Save();

        Debug.Log("En yüksek skor baþarýyla sýfýrlandý!");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
    //çýkýþ için 
    public void QuitGame()
    {
        Debug.Log("Çýkýþ iþlemi tetiklendi!");

        // Eðer oyunu Unity Editörü içinde test ediyorsak "Play" modunu durdurur
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            
        // Eðer oyun derlenmiþse (EXE, APK vb.) uygulamayý tamamen kapatýr
#else
        Application.Quit();
#endif
    }
}