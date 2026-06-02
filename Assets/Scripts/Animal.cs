using UnityEngine;
using System.Collections.Generic;

public class Animal : MonoBehaviour
{
    public enum AnimalType { Cat1, Cat2, Cat3 }

    [Header("Kimlik Ayarlarý")]
    [SerializeField] private AnimalType animalType;

    [Header("Efekt Ayarlarý")]
    [SerializeField] private GameObject explosionPrefab;

    //(Encapsulation): Veriyi dýþarýdan okunabilir, ama sadece içeriden deðiþtirilebilir yaptýk.
    public bool IsMatched { get; private set; }

    private bool inBasket = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Güvenlik: Unity Editöründe Tag unutulursa diye kodla garantiliyoruz.
        gameObject.tag = "Animal";
    }

    void Update()
    {
        // 1. Oyun bittiyse fiziksel iþlemleri ve hesaplamalarý durdur.
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        // 2. Çöp Toplayýcý (Garbage Collection): Ekrandan düþen objeleri bellekten sil.
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsMatched) return;

        if (collision.gameObject.CompareTag("Animal"))
        {
            Animal other = collision.gameObject.GetComponent<Animal>();

            // Çarptýðýmýz obje ayný türden bir kediyse ve henüz eþleþmemiþse (zincirleme reaksiyon kontrolü)
            if (other != null && other.animalType == this.animalType && !other.IsMatched)
            {
                CheckForExplosion();
            }
        }
    }

    void CheckForExplosion()
    {
        List<GameObject> matchingAnimals = new List<GameObject>();
        FindRecursiveMatches(gameObject, matchingAnimals);

        if (matchingAnimals.Count >= 3)
        {
            // 1. Ekraný Salla (Juice Effect)
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.TriggerShake(0.15f, 0.2f);
            }

            // 2. Skoru Ekle ve Sesi Çal
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(matchingAnimals.Count * 10);
                GameManager.Instance.PlayExplosionSound();
            }

            // 3. Eþleþen tüm kedileri döngüyle patlat ve bellekten (RAM) sil
            foreach (GameObject obj in matchingAnimals)
            {
                if (obj != null)
                {
                    Animal animScript = obj.GetComponent<Animal>();
                    if (animScript != null)
                    {
                        animScript.IsMatched = true; // Tekrar döngüye girmesini engeller (Infinite Loop Korumasý)

                        // Partikül Efektini Yarat
                        if (explosionPrefab != null)
                        {
                            Instantiate(explosionPrefab, obj.transform.position, Quaternion.identity);
                        }

                        Destroy(obj);
                    }
                }
            }
        }
    }
    //özyineleme ile sýnýrsýz derinlikte arama imkaný 
    void FindRecursiveMatches(GameObject current, List<GameObject> matches)
    {
        if (current == null || matches.Contains(current)) return;

        matches.Add(current);

        // Optimizasyon: Sadece belirli bir çaptaki objeleri tarar
        Collider2D[] nearby = Physics2D.OverlapCircleAll(current.transform.position, 1.2f);
        foreach (var col in nearby)
        {
            if (col == null || col.gameObject == current) continue;

            Animal other = col.GetComponent<Animal>();
            if (other != null && other.animalType == this.animalType && !other.IsMatched)
            {
                FindRecursiveMatches(col.gameObject, matches);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (inBasket) return;

        if (other.CompareTag("Basket"))
        {
            inBasket = true;

            
            // Çarptýðýmýz sepetin koduna ulaþýp, "beni listene ekle" diyoruz.
            Basket basketScript = other.GetComponent<Basket>();
            if (basketScript != null)
            {
                basketScript.AddAnimal(this); 
            }
        }
    }

    
    public AnimalType GetAnimalType()
    {
        return animalType;
    }
}