using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public GameObject[] animalPrefabs;
    public Transform spawnPoint;
    public float minX = -3.5f;
    public float maxX = 3.5f;

    [Header("Fýrlatma Gecikmesi")]
    public float fireRate = 0.5f;
    private float nextSpawnTime = 0f;
    private bool isCooldownActive = false;

    [Header("MAKRO / SPAM ENGELLEME SÝSTEMÝ (YENÝ)")]
    [Tooltip("Oyuncu kaç defa hýzlýca basarsa cezalandýrýlsýn?")]
    public int maxSpamClickCount = 4;
    [Tooltip("Spam yaparsa kaç saniye boyunca kilitlensin ve atamasýn?")]
    public float penaltyDuration = 2.0f;
    [Tooltip("Týklamalarýn spam sayýlmasý için iki týk arasýndaki maksimum süre")]
    public float spamTimeWindow = 0.25f;

    private int currentSpamClicks = 0;    // Ard arda yapýlan hýzlý týklama sayýsý
    private float lastClickTime = 0f;      // Son týklama yapýlan zaman
    private bool isPenalized = false;      // Oyuncu þu an cezalý mý?
    private float penaltyEndTime = 0f;     // Ceza ne zaman bitecek?

    private GameObject currentPreviewAnimal;
    private int nextAnimalIndex;

    void Start()
    {
        PrepareNextAnimal();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        if (GameManager.Instance != null)
        {
            fireRate = GameManager.Instance.currentFireRate;
        }

        // --- CEZA SÜRESÝ KONTROLÜ ---
        if (isPenalized)
        {
            // Eðer ceza süresi bittiyse cezayý kaldýr
            if (Time.time >= penaltyEndTime)
            {
                isPenalized = false;
                currentSpamClicks = 0;
                Debug.Log("Ceza bitti! Artýk fýrlatabilirsin.");
                if (currentPreviewAnimal == null) PrepareNextAnimal();
            }
            else
            {
                // Oyuncu cezalýyken elindeki kediyi sakla veya deaktif et (görsel geri bildirim)
                if (currentPreviewAnimal != null)
                {
                    Destroy(currentPreviewAnimal);
                    currentPreviewAnimal = null;
                }
                return; // Cezalýyken Update'in geri kalanýný çalýþtýrma, týklamalarý tamamen kilitler
            }
        }

        // --- NORMAL COOLDOWN KONTROLÜ ---
        if (isCooldownActive && Time.time >= nextSpawnTime)
        {
            isCooldownActive = false;
            if (currentPreviewAnimal == null && !isPenalized)
            {
                PrepareNextAnimal();
            }
        }

        // 1. Fare konumunu takip et
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float clampedX = Mathf.Clamp(mousePosition.x, minX, maxX);
        Vector3 targetPos = new Vector3(clampedX, spawnPoint.position.y, 0);

        // 2. Önizleme kedisini hareket ettir
        if (currentPreviewAnimal != null)
        {
            currentPreviewAnimal.transform.position = targetPos;
        }

        // 3. TIKLAMA VE SPAM ANALÝZÝ
        if (Input.GetMouseButtonDown(0))
        {
            // Ýki týklama arasýndaki süreyi ölçüyoruz
            if (Time.time - lastClickTime <= spamTimeWindow)
            {
                currentSpamClicks++; // Çok hýzlý bastýysa spam sayacýný arttýr
                Debug.Log("Spam Algýlandý! Sayac: " + currentSpamClicks);
            }
            else
            {
                // Eðer oyuncu normal tempoda bekleyerek basýyorsa sayacý düþür/sýfýrla
                currentSpamClicks = Mathf.Max(0, currentSpamClicks - 1);
            }

            lastClickTime = Time.time; // Son týklama zamanýný güncelle

            // EÐER MAKRO/SPAM SINIRI AÞILDIYSA CEZA KES
            if (currentSpamClicks >= maxSpamClickCount)
            {
                isPenalized = true;
                penaltyEndTime = Time.time + penaltyDuration;
                Debug.LogWarning("MAKRO TESPÝT EDÝLDÝ! " + penaltyDuration + " saniye fýrlatma kilitlendi.");

                // Elindeki kediyi sahneden sil (Ceza aldýðýný anlasýn)
                if (currentPreviewAnimal != null)
                {
                    Destroy(currentPreviewAnimal);
                    currentPreviewAnimal = null;
                }
                return;
            }

            // Normal fýrlatma þartý (Cezalý deðilse ve normal bekleme süresi bittiyse)
            if (!isCooldownActive && currentPreviewAnimal != null && !isPenalized)
            {
                ThrowAnimal(targetPos);
            }
        }
    }

    void PrepareNextAnimal()
    {
        if (animalPrefabs == null || animalPrefabs.Length == 0) return;

        nextAnimalIndex = Random.Range(0, animalPrefabs.Length);
        currentPreviewAnimal = Instantiate(animalPrefabs[nextAnimalIndex], spawnPoint.position, Quaternion.identity);

        Rigidbody2D rb = currentPreviewAnimal.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;
    }

    void ThrowAnimal(Vector3 position)
    {
        isCooldownActive = true;
        nextSpawnTime = Time.time + fireRate;

        Rigidbody2D rb = currentPreviewAnimal.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = true;

        currentPreviewAnimal = null;
    }
}