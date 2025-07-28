using UnityEngine;
using UnityEngine.UI;

public class CardFlipper : MonoBehaviour
{
    public Image cardImage;
    public Image coverImage;

    private Sprite hiddenImage;
    private MemoryGameManager gameManager;
    private bool isFlipped = false;
    private Button button;

    [Header("Animation Settings")]
    public Animator animator;
    public float flipDelay = 0.5f;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(FlipCard);

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void SetCard(Sprite image, MemoryGameManager manager)
    {
        hiddenImage = image;
        gameManager = manager;

        if (cardImage != null)
            cardImage.sprite = hiddenImage;

        if (coverImage != null)
            coverImage.enabled = true;

        isFlipped = false;

        // Reset animator states
        if (animator != null)
        {
            animator.SetBool("IsFlipped", false);
            animator.SetBool("IsFlippingBack", false);
        }
    }

    void FlipCard()
    {
        if (isFlipped || gameManager == null || !gameManager.CanFlip(this))
            return;

        isFlipped = true;

        if (animator != null)
        {
            animator.SetBool("IsFlipped", true);
        }

        Invoke(nameof(ShowCard), flipDelay);
    }

    void ShowCard()
    {
        if (coverImage != null)
            coverImage.enabled = false;

        gameManager.CardFlipped(this);
    }

    public void HideCard()
    {
        isFlipped = false;

        if (coverImage != null)
            coverImage.enabled = true;

        if (animator != null)
        {
            animator.SetBool("IsFlippingBack", true);
            Invoke(nameof(ResetFlipBack), flipDelay);
        }
    }

    void ResetFlipBack()
    {
        if (animator != null)
        {
            animator.SetBool("IsFlippingBack", false);
        }
    }

    public Sprite GetCardImage()
    {
        return hiddenImage;
    }
}
