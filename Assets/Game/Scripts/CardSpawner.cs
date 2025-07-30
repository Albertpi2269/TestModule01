using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform parent; // Grid layout parent (GridLayoutGroup)
    public List<Sprite> cardImages; // Add 8 unique images in Inspector
    public MemoryGameManager manager;

    private const string CardOrderKey = "CardOrder";

    private void Start()
    {
        if (PlayerPrefs.GetInt("ResumeFlag", 0) == 1 && PlayerPrefs.HasKey(CardOrderKey))
        {
            LoadCardState();
        }
        else
        {
            SpawnCards();
        }
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

        SaveCardOrder(allCardSprites);
        CreateCardsFromSprites(allCardSprites);
    }

    void SaveCardOrder(List<Sprite> sprites)
    {
        List<string> order = new List<string>();
        foreach (var sprite in sprites)
        {
            order.Add(sprite.name);
        }

        PlayerPrefs.SetString(CardOrderKey, string.Join(",", order));
        PlayerPrefs.Save();
    }

    void LoadCardState()
    {
        string savedData = PlayerPrefs.GetString(CardOrderKey);
        string[] names = savedData.Split(',');

        List<Sprite> orderedSprites = new List<Sprite>();
        foreach (string name in names)
        {
            Sprite sprite = cardImages.Find(s => s.name == name);
            if (sprite != null)
                orderedSprites.Add(sprite);
        }

        CreateCardsFromSprites(orderedSprites);
    }

    void CreateCardsFromSprites(List<Sprite> sprites)
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, parent);
            CardFlipper flipper = newCard.GetComponent<CardFlipper>();
            flipper.SetCard(sprites[i], manager);

            // Resize images to fit the cell
            Image[] images = newCard.GetComponentsInChildren<Image>();
            foreach (Image img in images)
            {
                RectTransform rt = img.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;

                img.rectTransform.localRotation = Quaternion.Euler(0, 0, 180); // Flip Z
            }

            newCard.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }
}
