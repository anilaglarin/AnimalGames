using UnityEngine;
using System.Collections.Generic;

public partial class Basket : MonoBehaviour
{
    public int basketCapacity = 10; // Sepet kaç hayvan alabilir?
    private List<Animal> animalsInBasket = new List<Animal>();

    // Hayvan sepete girdiðinde Animal scripti tarafýndan çaðrýlýr
    public void AddAnimal(Animal newAnimal)
    {
        if (GameManager.Instance.isGameOver) return;

        animalsInBasket.Add(newAnimal); //aaa
        CheckMatches();

        // Sepet doldu mu kontrolü
        if (animalsInBasket.Count >= basketCapacity)
        {
            GameManager.Instance.GameOver();
        }
    }

    private void CheckMatches()
    {
        if (animalsInBasket.Count < 3) return;

        // Basit Match-3 Mantýðý: Son eklenen 3 hayvanýn tipi ayný mý?
        // (Bu kýsým geliþtirilebilir, ilk etapta son 3'e bakýyoruz)
        int count = animalsInBasket.Count;
        if (animalsInBasket[count - 1].GetAnimalType() == animalsInBasket[count - 2].GetAnimalType() &&
            animalsInBasket[count - 2].GetAnimalType() == animalsInBasket[count - 3].GetAnimalType())
        {
            // 3'lü eþleþme bulundu!
            RemoveMatchedAnimals(count - 1, count - 2, count - 3);
        }
    }

    private void RemoveMatchedAnimals(int i1, int i2, int i3)
    {
        // Önce sahneden (görsel olarak) yok et
        Destroy(animalsInBasket[i1].gameObject);
        Destroy(animalsInBasket[i2].gameObject);
        Destroy(animalsInBasket[i3].gameObject);

        // Listeden çýkar (Sýralama bozulmasýn diye sondan baþa doðru)
        animalsInBasket.RemoveAt(i1);
        animalsInBasket.RemoveAt(i2);
        animalsInBasket.RemoveAt(i3);

        // Skoru artýr
        GameManager.Instance.AddScore(5);
        Debug.Log("Eþleþme Tamam! +5 Puan");
    }
}