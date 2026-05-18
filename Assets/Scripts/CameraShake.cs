using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Diðer scriptlerden kolayca ulaþmak için "Instance" yapýyoruz
    public static CameraShake Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Bu fonksiyon dýþarýdan çaðrýlacak (Süre ve Þiddet alýr)
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Rastgele küçük sarsýntýlar oluþturur
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;

            // Bir sonraki frame'e kadar bekle
            yield return null;
        }

        // Sarsýntý bitince kamerayý eski yerine geri koy
        transform.localPosition = originalPos;
    }
}