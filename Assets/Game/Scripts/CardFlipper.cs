using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardFlipper : MonoBehaviour
{
    public Image cardImage;
    public Image coverImage;

    private Sprite hiddenImage;
    private MemoryGameManager gameManager;
    private bool isFlipped = false;
    private bool isAnimating = false;
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

        // Hide cursor on mobile platforms
#if UNITY_ANDROID || UNITY_IOS
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
#else
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
#endif
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
        isAnimating = false;

        if (animator != null)
        {
            animator.SetBool("IsFlipped", false);
            animator.SetBool("IsFlippingBack", false);
        }
    }

    void FlipCard()
    {
        if (isFlipped || isAnimating || gameManager == null || !gameManager.CanFlip(this))
            return;

        isAnimating = true;

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

        isFlipped = true;
        isAnimating = false;

        gameManager.CardFlipped(this);
    }

    public void HideCard()
    {
        if (isAnimating)
            return;

        isAnimating = true;

        if (coverImage != null)
            coverImage.enabled = true;

        if (animator != null)
        {
            animator.SetBool("IsFlippingBack", true);
        }

        Invoke(nameof(ResetFlipBack), flipDelay);
    }

    void ResetFlipBack()
    {
        if (animator != null)
        {
            animator.SetBool("IsFlippingBack", false);
            animator.SetBool("IsFlipped", false);
        }

        isFlipped = false;
        isAnimating = false;
    }

    public Sprite GetCardImage()
    {
        return hiddenImage;
    }

    public bool IsFlipped()
    {
        return isFlipped;
    }

    public void MoveToCenterAndDestroy(Vector3 worldTarget, float speed)
    {
        StartCoroutine(MoveAndFadeOut(worldTarget, speed));
    }

    IEnumerator MoveAndFadeOut(Vector3 targetPosition, float speed)
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector3 start = rt.position;
        Vector3 startScale = rt.localScale;

        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            rt.position = Vector3.Lerp(start, targetPosition, t);
            rt.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        Destroy(gameObject);
    }
}
