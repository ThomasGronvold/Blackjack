using System;
using System.Collections.Generic;
using System.Linq;

namespace WPFBlackjack;

public class GameParticipant
{
    private List<Card> _hand = new();
    public int CardsSum { get; private set; }

    /* Public Methods */

    public void AddCard(Card newCard, bool isDealer)
    {
        _hand.Add(newCard);

        if (_hand.Count > 1)
        {
            /* After card has been added to the hand, the sum of all cards in the hand is calculated
          And the CardsSum property is updated with the correct value */
            CalculateCardsSum(isDealer);

            /* If the sum of CardsSum is over 21, and the participant has an Ace with the value 11,
               the Ace will convert it's value into 1, to prevent the bust */
            AdjustAceValueIfBusted();
        }
    }

    public bool HasBlackjack()
    {
        if (_hand.Count == 2)
        {
            var anyAces = _hand.Any(c => c.FaceCardIdentity == "A");
            var anyFaceCards = _hand.Any(c => c.FaceCardIdentity is "J" or "Q" or "K");

            if (anyAces && anyFaceCards)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasGameInitialized()
    {
        if (_hand.Count >= 2)
        {
            return true;
        }

        return false;
    }

    public string DealerHiddenCardUrl()
    {
        return _hand[0].GetCardImg();
    }

    /* Private Methods */
    private void AdjustAceValueIfBusted()
    {
        Card firstAceWithFullValue =
            _hand.FirstOrDefault(card => card.FaceCardIdentity == "A" && card.Value != 1);

        if (CardsSum > 21 && firstAceWithFullValue != null)
        {
            firstAceWithFullValue.ChangeAceValue();
            CalculateCardsSum();
            AdjustAceValueIfBusted();
        }
    }

    private void CalculateCardsSum(bool isDealer = false)
    {
        /* If there are 2 cards in dealers hand it means the Players round is not over
           and the first card the dealer has gotten should be hidden, and so should the value. */
        if (!(isDealer && _hand.Count == 2))
        {
            CardsSum =
                _hand.Sum(x => x.Value);
        }
        else
        {
            CardsSum = _hand[1].Value;
        }
    }
}