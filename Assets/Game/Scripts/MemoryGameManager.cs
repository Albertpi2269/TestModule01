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

    public GameObject scoreboardPanel; // Panel with matched/pending texts
    public GameObject homeButton;      // Home button (assign in Inspector)

    void Start()
    {
        gameOverPanel.SetActive(false);
        UpdateUI();
    }

    public void CardFlipped(CardFlipper card)
    {
        flippedCards.Add(card);
        totalFlips++;

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
            foreach (CardFlipper card in flippedCards)
            {
                Destroy(card.gameObject);
            }

            matchCount++;
            Debug.Log("Match Count: " + matchCount);

            if (matchCount == 8)
            {
                Debug.Log("? You Win!");
                gameOverPanel.SetActive(true);
            }
        }
        else
        {
            foreach (CardFlipper card in flippedCards)
            {
                card.HideCard(); // Flip back
            }
        }

        flippedCards.Clear();

        if (totalFlips >= maxFlipsAllowed && matchCount < 8)
        {
            Debug.Log("? Game Over! You used all attempts.");
            gameOverPanel.SetActive(true);
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        bool isGameOver = gameOverPanel.activeSelf;

        if (scoreboardPanel != null)
            scoreboardPanel.SetActive(!isGameOver);

        if (homeButton != null)
            homeButton.SetActive(!isGameOver);

        if (!isGameOver)
        {
            if (matchedText != null)
                matchedText.text = "Matched: " + matchCount + "/8";

            if (pendingText != null)
                pendingText.text = "Flips Left: " + ((maxFlipsAllowed - totalFlips) / 2);
        }
    }
}
