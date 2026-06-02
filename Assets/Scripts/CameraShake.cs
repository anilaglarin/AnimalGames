using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Kapsülleme: Instance dýþarýdan okunabilir ama sadece içeriden yazýlabilir.
    public static CameraShake Instance { get; private set; }

    private Vector3 originalPos;
    private Coroutine currentShakeCoroutine;

    void Awake()
    {
        // Singleton Güvenlik Duvarý: Sahnede birden fazla CameraShake varsa klonlarý yok et.
        if (Instance == null)
        {
            Instance = this;
            // Orijinal pozisyonu oyun baþlarken bir kere alýp güvene alýyoruz.
            originalPos = transform.localPosition;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Dýþarýdan diðer scriptlerin çaðýracaðý GÜVENLÝ tetikleyici metodumuz
    public void TriggerShake(float duration, float magnitude)
    {
        // Eðer halihazýrda devam eden bir sarsýntý varsa, kameranýn sapýtmamasý için onu durduruyoruz.
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
            transform.localPosition = originalPos; // Kamerayý hemen merkeze çek
        }

        // Sarsýntýyý artýk CameraShake'in kendisi baþlatýyor (Böylece kedi silinse de sarsýntý devam eder)
        currentShakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    // Arka planda çalýþan asýl sarsýntý motoru (Dýþarýya kapalý)
    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = originalPos.x + Random.Range(-1f, 1f) * magnitude;
            float y = originalPos.y + Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }

        // Sarsýntý bitince kamerayý orijinal yerine mükemmel bir þekilde geri oturt
        transform.localPosition = originalPos;
        currentShakeCoroutine = null; // Ýþlem bitti, temizlik yapýldý
    }
}