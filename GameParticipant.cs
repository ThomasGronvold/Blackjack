using System.Collections.Generic;
using System.Linq;

namespace WPFBlackjack;

public class GameParticipant
{
    public readonly List<Card> _hand = new();
    public int CardsSum { get; private set; }

    /* Public Methods */

    public void AddCard(Card newCard)
    {
        _hand.Add(newCard);

        if (_hand.Count > 1)
        {
            /* After card has been added to the hand, the sum of all cards in the hand is calculated
               and the CardsSum property is updated with the correct value */
            CalculateCardsSum();

            /* If the sum of CardsSum is over 21, and the participant has an Ace with the value 11,
               the Ace will convert its value into 1, to prevent the bust */
            AdjustAceValueIfBusted();
        }
    }

    public bool HasBlackjack()
    {
        if (_hand.Count == 2 && CardsSum == 21)
        {
            return true;
        }

        return false;
    }

    public string DealerHiddenCard()
    {
        return _hand[0].GetCardImg();
    }

    public void ResetParticipant()
    {
        _hand.Clear();
        CardsSum = 0;
    }

    /* Private Methods */
    private void AdjustAceValueIfBusted()
    {
        var firstAceWithFullValue =
            _hand.FirstOrDefault(card => card.FaceCardIdentity == "A" && card.Value != 1)!;

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
}