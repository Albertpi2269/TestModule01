using UnityEngine;
using System.Collections.Generic;

public class MemoryGameManager : MonoBehaviour
{
    private List<CardFlipper> flippedCards = new List<CardFlipper>();

    public void CardFlipped(CardFlipper card)
    {
        flippedCards.Add(card);

        if (flippedCards.Count == 2)
        {
            CheckMatch();
        }
    }

    public bool CanFlip(CardFlipper card)
    {
        return flippedCards.Count < 2 && !flippedCards.Contains(card);
    }

    void CheckMatch()
    {
        if (flippedCards[0].GetCardImage() == flippedCards[1].GetCardImage())
        {
            // Match found
            flippedCards.Clear();
        }
        else
        {
            // No match — hide cards after delay
            StartCoroutine(HideCardsAfterDelay());
        }
    }

    private IEnumerator<WaitForSeconds> HideCardsAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        foreach (CardFlipper card in flippedCards)
        {
            card.HideCard();
        }

        flippedCards.Clear();
    }
}
