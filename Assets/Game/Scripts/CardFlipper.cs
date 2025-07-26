using UnityEngine;
using UnityEngine.UI;

public class CardFlipper : MonoBehaviour
{
    public Image cardImage; // Drag the Image component that shows the sprite
    private Sprite hiddenImage;
    private MemoryGameManager gameManager;
    private bool isFlipped = false;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(FlipCard);
    }

    public void SetCardImage(Sprite image, MemoryGameManager manager)
    {
        hiddenImage = image;
        gameManager = manager;
        cardImage.enabled = false; // hide image initially
    }

    void FlipCard()
    {
        if (isFlipped || !gameManager.CanFlip(this)) return;

        isFlipped = true;
        cardImage.sprite = hiddenImage;
        cardImage.enabled = true;

        gameManager.CardFlipped(this);
    }

    public void HideCard()
    {
        isFlipped = false;
        cardImage.enabled = false;
    }

    public Sprite GetCardImage()
    {
        return hiddenImage;
    }
}
