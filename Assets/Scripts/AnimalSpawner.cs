using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public GameObject[] animalPrefabs;
    public Transform spawnPoint;
    public float minX = -3.5f; 
    public float maxX = 3.5f;

    [Header("Fýrlatma Gecikmesi")]
    public float fireRate = 0.5f; // Ýki fýrlatma arasýnda beklenecek süre (Saniye)
    private float nextSpawnTime = 0f; // Yeni kedinin ne zaman geleceðini tutan zamanlayýcý
    private bool isCooldownActive = false; // Bekleme süresinde miyiz kontrolü

    private GameObject currentPreviewAnimal; // Elimizde tuttuðumuz görsel kedi
    private int nextAnimalIndex; // Sýradaki kedinin listedeki numarasý

    void Start()
    {
        PrepareNextAnimal();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        // --- ZAMANLAYICI KONTROLÜ ---
        // Eðer fýrlatma yapýldýysa ve bekleme süresindeysek, sürenin dolmasýný bekliyoruz
        if (isCooldownActive && Time.time >= nextSpawnTime)
        {
            isCooldownActive = false;
            PrepareNextAnimal(); // Süre dolunca yeni kediyi elimize veriyoruz
        }

        // 1. Fare konumunu takip et
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float clampedX = Mathf.Clamp(mousePosition.x, minX, maxX);
        Vector3 targetPos = new Vector3(clampedX, spawnPoint.position.y, 0);

        // 2. Elimizdeki önizleme kedisini fareyle birlikte hareket ettir
        if (currentPreviewAnimal != null)
        {
            currentPreviewAnimal.transform.position = targetPos;
        }

        // 3. Týklayýnca fýrlat (Sadece elimizde kedi VARKEN ve COOLDOWN YOKKEN çalýþýr)
        if (Input.GetMouseButtonDown(0) && currentPreviewAnimal != null && !isCooldownActive)
        {
            ThrowAnimal(targetPos);
        }
    }

    void PrepareNextAnimal()
    {
        if (animalPrefabs == null || animalPrefabs.Length == 0) return;

        // Rastgele sýradaki kediyi seç
        nextAnimalIndex = Random.Range(0, animalPrefabs.Length);
        
        // Önizleme için kediyi oluþtur
        currentPreviewAnimal = Instantiate(animalPrefabs[nextAnimalIndex], spawnPoint.position, Quaternion.identity);
        
        // Önizleme kedisinin fiziðini kapat (yere düþmemesi için)
        Rigidbody2D rb = currentPreviewAnimal.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;
    }

    void ThrowAnimal(Vector3 position)
    {
        // Elimdeki önizleme kedisinin fiziðini aç ve fýrlat
        Rigidbody2D rb = currentPreviewAnimal.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = true;

        // Artýk bu kedi baðýmsýz, listemizden çýkarýyoruz
        currentPreviewAnimal = null;

        // --- COOLDOWN BAÞLATMA ---
        isCooldownActive = true;
        nextSpawnTime = Time.time + fireRate; // Bir sonraki kedinin geliþ zamanýný kilitliyoruz
    }
}