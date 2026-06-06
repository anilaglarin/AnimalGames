using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private GameObject[] animalPrefabs;
    [SerializeField] private Transform spawnPoint;

    [Header("Sýnýr Ayarlarý")]
    [SerializeField] private float minX = -3.5f;
    [SerializeField] private float maxX = 3.5f;
    
    [Header("Makro / Spam Engelleme Sistemi")]
    [Tooltip("Oyuncu kaç defa hýzlýca basarsa cezalandýrýlsýn?")]
    [SerializeField] private int maxSpamClickCount = 4;
    [Tooltip("Spam yaparsa kaç saniye boyunca kilitlensin ve atamasýn?")]
    [SerializeField] private float penaltyDuration = 2.0f;
    [Tooltip("Týklamalarýn spam sayýlmasý için iki týk arasýndaki maksimum süre")]
    [SerializeField] private float spamTimeWindow = 0.25f;

    // --- (Encapsulation) ---
    private float fireRate = 0.5f;
    private float nextSpawnTime = 0f;
    private bool isCooldownActive = false;

    private int currentSpamClicks = 0;
    private float lastClickTime = 0f;
    private bool isPenalized = false;
    private float penaltyEndTime = 0f;

    private GameObject currentPreviewAnimal;
    private Camera mainCamera; // Performans optimizasyonu için kamerayý önbelleðe alýyoruz

    void Start()
    {
        // Update içinde her karede kamerayý aramak yerine, oyun baþýnda bir kez bulup kaydediyoruz.
        mainCamera = Camera.main;
        PrepareNextAnimal();
    }

    void Update()
    {
        // GameManager kontrollerini birleþtirerek iþlemci yükünü hafiflettik
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.isGameOver) return;
            fireRate = GameManager.Instance.currentFireRate; // Merkezi senkronizasyon
        }

        
        if (isPenalized)
        {
            HandlePenaltyState();
            return; // Cezalýyken Update fonksiyonunun geri kalanýný çalýþtýrmaya gerek yok
        }

        // --- NORMAL COOLDOWN KONTROLÜ ---
        if (isCooldownActive && Time.time >= nextSpawnTime)
        {
            isCooldownActive = false;
            if (currentPreviewAnimal == null)
            {
                PrepareNextAnimal();
            }
        }

        // 1. Fare konumunu takip et (Önbelleðe alýnmýþ kamera ile------>optimizasyon için)
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float clampedX = Mathf.Clamp(mousePosition.x, minX, maxX);
        Vector3 targetPos = new Vector3(clampedX, spawnPoint.position.y, 0);

        // 2. Önizleme kedisini hareket ettir
        if (currentPreviewAnimal != null)
        {
            currentPreviewAnimal.transform.position = targetPos;
        }

        // 3. Týklama Algýlama (Kod karmaþasýný önlemek için alt fonksiyona taþýndý)
        if (Input.GetMouseButtonDown(0))
        {
            ProcessClickInput(targetPos);
        }
    }

    

    private void HandlePenaltyState()
    {
        if (Time.time >= penaltyEndTime)
        {
            isPenalized = false;
            currentSpamClicks = 0;
            Debug.Log("Ceza bitti! Artýk fýrlatabilirsin.");

            if (currentPreviewAnimal == null)
            {
                PrepareNextAnimal();
            }
        }
        else if (currentPreviewAnimal != null)
        {
            Destroy(currentPreviewAnimal);
            currentPreviewAnimal = null;
        }
    }

    private void ProcessClickInput(Vector3 targetPos)
    {
        // Ýki týklama arasýndaki süreyi ölçüyoruz
        if (Time.time - lastClickTime <= spamTimeWindow)
        {
            currentSpamClicks++;
            Debug.Log("Spam Algýlandý! Sayac: " + currentSpamClicks);
        }
        else
        {
            currentSpamClicks = Mathf.Max(0, currentSpamClicks - 1);
        }

        lastClickTime = Time.time;

        // Sýnýr aþýldýysa ceza 
        if (currentSpamClicks >= maxSpamClickCount)
        {
            isPenalized = true;
            penaltyEndTime = Time.time + penaltyDuration;
            Debug.LogWarning("MAKRO TESPÝT EDÝLDÝ! " + penaltyDuration + " saniye fýrlatma kilitlendi.");

            if (currentPreviewAnimal != null)
            {
                Destroy(currentPreviewAnimal);
                currentPreviewAnimal = null;
            }
            return;
        }

        // Normal fýrlatma þartý
        if (!isCooldownActive && currentPreviewAnimal != null)
        {
            ThrowAnimal(targetPos);
        }
    }

    private void PrepareNextAnimal()
    {
        if (animalPrefabs == null || animalPrefabs.Length == 0) return;

        int nextAnimalIndex = Random.Range(0, animalPrefabs.Length);
        currentPreviewAnimal = Instantiate(animalPrefabs[nextAnimalIndex], spawnPoint.position, Quaternion.identity);

        Rigidbody2D rb = currentPreviewAnimal.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;
    }

    private void ThrowAnimal(Vector3 position)
    {
        isCooldownActive = true;
        nextSpawnTime = Time.time + fireRate;

        Rigidbody2D rb = currentPreviewAnimal.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = true;

        currentPreviewAnimal = null;
    }
}