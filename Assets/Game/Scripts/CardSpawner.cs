using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform parent; // Grid layout parent (GridLayoutGroup)
    public List<Sprite> cardImages; // Add 8 unique images in Inspector
    public MemoryGameManager manager; // Drag your GameManager GameObject here

    private void Start()
    {
        SpawnCards();
    }

    void SpawnCards()
    {
        if (cardImages.Count < 8)
        {
            Debug.LogError("You need at least 8 unique images.");
            return;
        }

        List<Sprite> allCardSprites = new List<Sprite>();
        foreach (Sprite img in cardImages)
        {
            allCardSprites.Add(img);
            allCardSprites.Add(img); // make pairs
        }

        // Shuffle
        for (int i = 0; i < allCardSprites.Count; i++)
        {
            Sprite temp = allCardSprites[i];
            int randomIndex = Random.Range(i, allCardSprites.Count);
            allCardSprites[i] = allCardSprites[randomIndex];
            allCardSprites[randomIndex] = temp;
        }

        // Instantiate cards
        for (int i = 0; i < 16; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, parent);
            CardFlipper flipper = newCard.GetComponent<CardFlipper>();

            flipper.SetCard(allCardSprites[i], manager);

            // Resize images
            Image[] images = newCard.GetComponentsInChildren<Image>();
            foreach (Image img in images)
            {
                RectTransform rt = img.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;

                // ?? Rotate 180 degrees on Y axis (flip face down)
                img.rectTransform.localRotation = Quaternion.Euler(0, 0, 180);
            }

            newCard.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

}
