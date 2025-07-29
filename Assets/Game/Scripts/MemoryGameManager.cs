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

    public GameObject scoreboardPanel;
    public GameObject homeButton;
    public GameObject[] uiButtons;

    [Header("Match Animation Settings")]
    public float matchMoveSpeed = 2f;

    void Start()
    {
        gameOverPanel.SetActive(false);
        SetUIActive(true);
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
            // Convert screen center to world position in canvas space
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
                GameOver(true);
            }
        }
        else
        {
            foreach (CardFlipper card in flippedCards)
            {
                card.HideCard();
            }
        }

        flippedCards.Clear();

        if (totalFlips >= maxFlipsAllowed && matchCount < 8)
        {
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
        gameOverPanel.SetActive(true);
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
}
