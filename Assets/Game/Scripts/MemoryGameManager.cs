using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MemoryGameManager : MonoBehaviour
{
    private List<CardFlipper> flippedCards = new List<CardFlipper>();
    private int matchCount = 0;
    private int totalFlips = 0;
    private const int maxFlipsAllowed = 32;

    [Header("UI")]
    public TextMeshProUGUI matchedText;
    public TextMeshProUGUI pendingText;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject scoreboardPanel;
    public GameObject homeButton;
    public GameObject[] uiButtons;

    [Header("Match Animation Settings")]
    public float matchMoveSpeed = 2f;

    [Header("Audio Clips")]
    public AudioClip cardFlipSound;
    public AudioClip matchSound;
    public AudioClip notMatchSound;
    public AudioClip gameOverSound;
    public AudioClip winSound;
    public AudioClip backgroundMusic;

    [Header("Audio Sources")]
    public AudioSource sfxSource; // For SFX
    public AudioSource bgmSource; // For BGM

    void Start()
    {
        StartBackgroundMusic();
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        SetUIActive(true);
        UpdateUI();
    }

    void StartBackgroundMusic()
    {
        if (bgmSource != null && backgroundMusic != null)
        {
            bgmSource.clip = backgroundMusic;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void CardFlipped(CardFlipper card)
    {
        flippedCards.Add(card);
        totalFlips++;

        PlaySFX(cardFlipSound);
        UpdateUI();

        if (flippedCards.Count == 2)
        {
            StartCoroutine(CheckMatch());
        }
    }

    public bool CanFlip(CardFlipper card)
    {
        return flippedCards.Count < 2 &&
               !flippedCards.Contains(card) &&
               totalFlips < maxFlipsAllowed;
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.5f);

        if (flippedCards[0].GetCardImage() == flippedCards[1].GetCardImage())
        {
            PlaySFX(matchSound);

            Vector3 centerScreen = Vector3.zero;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                flippedCards[0].GetComponent<RectTransform>(),
                new Vector2(Screen.width / 2f, Screen.height / 2f),
                Camera.main,
                out centerScreen
            );

            foreach (CardFlipper card in flippedCards)
            {
                card.MoveToCenterAndDestroy(centerScreen, matchMoveSpeed);
            }

            matchCount++;

            if (matchCount == 8)
            {
                PlaySFX(winSound);
                GameOver(true);
            }
        }
        else
        {
            PlaySFX(notMatchSound);

            foreach (CardFlipper card in flippedCards)
            {
                card.HideCard();
            }
        }

        flippedCards.Clear();

        if (totalFlips >= maxFlipsAllowed && matchCount < 8)
        {
            PlaySFX(gameOverSound);
            GameOver(false);
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        bool isGameOver = gameOverPanel.activeSelf;

        if (!isGameOver)
        {
            if (matchedText != null)
                matchedText.text = "Matched: " + matchCount + "/8";

            if (pendingText != null)
                pendingText.text = "Flips Left: " + ((maxFlipsAllowed - totalFlips) / 2);
        }
    }

    void GameOver(bool win)
    {
        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Stop();

        gameOverPanel.SetActive(true);

        if (win && winPanel != null)
        {
            winPanel.SetActive(true);
        }

        SetUIActive(false);
    }

    void SetUIActive(bool isActive)
    {
        if (scoreboardPanel != null)
            scoreboardPanel.SetActive(isActive);

        if (homeButton != null)
            homeButton.SetActive(isActive);

        foreach (GameObject btn in uiButtons)
        {
            if (btn != null)
                btn.SetActive(isActive);
        }
    }

    // Call this when restarting the game
    public void RestartGame()
    {
        matchCount = 0;
        totalFlips = 0;
        flippedCards.Clear();

        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);

        SetUIActive(true);
        UpdateUI();
        StartBackgroundMusic();
    }
}
