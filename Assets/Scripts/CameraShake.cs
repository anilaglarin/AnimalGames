using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    
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

   
    public void TriggerShake(float duration, float magnitude)
    {
        // Eðer halihazýrda devam eden bir sarsýntý varsa, kameranýn sapýtmamasý için onu durduruyoruz.
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
            transform.localPosition = originalPos; 
        }

        
        currentShakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    
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

        //sarsýntý bitince kamera eski yerine oturmasý için 
        transform.localPosition = originalPos;
        currentShakeCoroutine = null; 
    }
}