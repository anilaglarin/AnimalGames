using UnityEngine;
using System.Collections.Generic;

public class Basket : MonoBehaviour
{
    [Header("Sepet Ayarlarý")]
    [Tooltip("Sepet maksimum kaç hayvan alabilir?")]
    [SerializeField] private int basketCapacity = 10;

    private List<Animal> animalsInBasket = new List<Animal>();

    
    // Hayvan sepete girdiðinde Animal scripti tarafýndan çaðrýlýr
    public void AddAnimal(Animal newAnimal)
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        // --- ÝÞTE HAYAT KURTARAN TEMÝZLÝK SATIRI ---
        // Sahneden silinmiþ ama listede yer iþgal eden "hayalet (null)" kedileri listeden atar.
        animalsInBasket.RemoveAll(animal => animal == null);

        if (newAnimal == null || animalsInBasket.Contains(newAnimal)) return;

        animalsInBasket.Add(newAnimal);
        CheckMatches();

        // Sepet doldu mu kontrolü
        if (animalsInBasket.Count >= basketCapacity)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private void CheckMatches()
    {
        int count = animalsInBasket.Count;
        if (count < 3) return;

        // Okunabilirliði artýrmak için son 3 hayvaný geçici deðiþkenlere alýyoruz
        Animal last1 = animalsInBasket[count - 1];
        Animal last2 = animalsInBasket[count - 2];
        Animal last3 = animalsInBasket[count - 3];

        
        if (last1 != null && last2 != null && last3 != null)
        {
            if (last1.GetAnimalType() == last2.GetAnimalType() &&
                last2.GetAnimalType() == last3.GetAnimalType())
            {
                // 3'lü eþleþme bulundu!
                RemoveMatchedAnimals(last1, last2, last3);
            }
        }
    }

    private void RemoveMatchedAnimals(Animal a1, Animal a2, Animal a3)
    {
        // 1. Listeden Toplu ve Güvenli Silme (Performans Optimizasyonu)
        animalsInBasket.RemoveRange(animalsInBasket.Count - 3, 3);

        
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.TriggerShake(0.15f, 0.2f);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(5);
            GameManager.Instance.PlayExplosionSound(); // Hayvanlar patladýðý için sesi burada da çaðýrdým
        }

        //bellek temizliði 
        Destroy(a1.gameObject);
        Destroy(a2.gameObject);
        Destroy(a3.gameObject);

        Debug.Log("Sepette 3'lü Eþleþme Tamam! +5 Puan");
    }
}