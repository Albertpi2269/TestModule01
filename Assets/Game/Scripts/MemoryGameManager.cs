// Replace your existing script with this updated version
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
    private bool isGameOver = false;
    private bool resumedFromLastState = false;

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
    public AudioSource sfxSource;
    public AudioSource bgmSource;

    public bool IsResuming => resumedFromLastState;

    void Start()
    {
        resumedFromLastState = PlayerPrefs.GetInt("ResumeFlag", 0) == 1;

        if (resumedFromLastState)
        {
            LoadGameState();
        }

        StartBackgroundMusic();
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        SetUIActive(true);
        UpdateUI();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause && !isGameOver)
        {
            SaveGameState();
        }
    }

    void OnApplicationQuit()
    {
        if (!isGameOver)
        {
            SaveGameState();
        }
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
            flippedCards.Clear();

            if (matchCount == 8)
            {
                PlaySFX(winSound);
                GameOver(true);
                yield break;
            }
        }
        else
        {
            PlaySFX(notMatchSound);

            foreach (CardFlipper card in flippedCards)
            {
                card.HideCard();
            }

            flippedCards.Clear();
        }

        if (totalFlips >= maxFlipsAllowed && matchCount < 8)
        {
            PlaySFX(gameOverSound);
            GameOver(false);
        }

        UpdateUI();
    }

    void UpdateUI()
    {
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
        isGameOver = true;

        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Stop();

        gameOverPanel.SetActive(true);

        if (win && winPanel != null)
        {
            winPanel.SetActive(true);
        }

        SetUIActive(false);

        PlayerPrefs.DeleteAll();

        Debug.Log("Game Over. Save data cleared.");
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

    public void RestartGame()
    {
        matchCount = 0;
        totalFlips = 0;
        flippedCards.Clear();
        isGameOver = false;

        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);

        SetUIActive(true);
        UpdateUI();
        StartBackgroundMusic();

        PlayerPrefs.DeleteAll();

        Debug.Log("Game restarted and all save data cleared.");
    }

    public void SaveGameState()
    {
        PlayerPrefs.SetInt("SavedMatchCount", matchCount);
        PlayerPrefs.SetInt("SavedTotalFlips", totalFlips);
        PlayerPrefs.SetInt("ResumeFlag", 1);
        PlayerPrefs.Save();

        Debug.Log("Game Saved. MatchCount: " + matchCount + ", TotalFlips: " + totalFlips);
    }

    void LoadGameState()
    {
        matchCount = PlayerPrefs.GetInt("SavedMatchCount", 0);
        totalFlips = PlayerPrefs.GetInt("SavedTotalFlips", 0);
        Debug.Log("Loaded MatchCount: " + matchCount + ", TotalFlips: " + totalFlips);
    }
}
