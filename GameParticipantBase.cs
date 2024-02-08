using System;
using System.Collections.Generic;
using System.Linq;

namespace WPFBlackjack;

public abstract class GameParticipantBase
{
    protected List<Card> _hand = new();
    public int CardsSum { get; private set; }

    public void AddCard(Card newCard)
    {
        /* newCard is passed through from GameManager and added to the participants _hand list */
        _hand.Add(newCard);
        /* After card has been added to the hand, the sum of all cards in the hand is calculated
           And the CardsSum property is updated with the correct value */
        CalculateCardsSum();
        /* If the sum of CardsSum is over 21, and the participant has an Ace with the value 11,
           the Ace will convert it's value into 1, to prevent the bust */
        AdjustAceValueIfBusted();
    }

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

    private void CalculateCardsSum()
    {
        CardsSum =
            _hand.Sum(x => x.Value);
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
}