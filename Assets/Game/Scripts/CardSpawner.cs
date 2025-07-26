using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform gridParent;
    public Sprite[] cardImages;
    public MemoryGameManager manager; // Drag your GameManager here

    void Start()
    {
        SpawnCards();
    }

    void SpawnCards()
    {
        List<Sprite> imagePool = new List<Sprite>();

        // Duplicate for pairs
        foreach (Sprite img in cardImages)
        {
            imagePool.Add(img);
            imagePool.Add(img);
        }

        // Shuffle
        for (int i = 0; i < imagePool.Count; i++)
        {
            Sprite temp = imagePool[i];
            int randIndex = Random.Range(i, imagePool.Count);
            imagePool[i] = imagePool[randIndex];
            imagePool[randIndex] = temp;
        }

        // Instantiate and assign
        for (int i = 0; i < 16; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, gridParent);
            CardFlipper flipper = newCard.GetComponent<CardFlipper>();
            flipper.SetCardImage(imagePool[i], manager);
        }
    }
}
