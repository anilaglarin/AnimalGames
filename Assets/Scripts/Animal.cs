using UnityEngine;
using System.Collections.Generic;

public class Animal : MonoBehaviour
{
    public enum AnimalType { Cat1, Cat2, Cat3 }
    public AnimalType animalType;
    public float fallSpeed = 3f;

    [Header("Efekt Ayarlarý")]
    public GameObject explosionPrefab; // Inspector'dan partikül prefab'ýný buraya sürükle

    private bool inBasket = false;
    private Rigidbody2D rb;
    private bool isMatched = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameObject.tag = "Animal";
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isMatched) return;

        if (collision.gameObject.CompareTag("Animal"))
        {
            Animal other = collision.gameObject.GetComponent<Animal>();
            if (other != null && other.animalType == this.animalType && !other.isMatched)
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
            // 1. Ekraný Salla
            if (CameraShake.Instance != null)
            {
                StartCoroutine(CameraShake.Instance.Shake(0.15f, 0.2f));
            }

            // 2. Skoru Ekle
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(matchingAnimals.Count * 10);

                // --- SESÝ TETÝKLEYEN ADIM (YENÝ) ---
                // Kediler 3'lenip patladýðý an GameManager'a ses emri gidiyor.
                GameManager.Instance.PlayExplosionSound();
            }

            // 3. Her kedi için patlama efekti çýkar ve kediyi yok et
            foreach (GameObject obj in matchingAnimals)
            {
                if (obj != null)
                {
                    Animal animScript = obj.GetComponent<Animal>();
                    if (animScript != null)
                    {
                        animScript.isMatched = true;

                        // --- PARTÝKÜL EFEKTÝ BURADA OLUÞTURULUYOR ---
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

    void FindRecursiveMatches(GameObject current, List<GameObject> matches)
    {
        if (current == null || matches.Contains(current)) return;

        matches.Add(current);

        Collider2D[] nearby = Physics2D.OverlapCircleAll(current.transform.position, 1.2f);
        foreach (var col in nearby)
        {
            if (col == null || col.gameObject == null || col.gameObject == current) continue;

            Animal other = col.GetComponent<Animal>();
            if (other != null && other.animalType == this.animalType && !other.isMatched)
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
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
            }

            // Sepet mantýðý burada bitiyor
        }
    }

    public AnimalType GetAnimalType()
    {
        return animalType;
    }
}