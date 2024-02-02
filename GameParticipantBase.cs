using System.Collections.Generic;

namespace WPFBlackjack;

public abstract class GameParticipantBase
{
    protected List<Card> _hand = new List<Card>();
    protected int CardValue;
    public int UpdateParticipantInfo(Card newCard)
    {
        var newValue = int.TryParse(newCard.Value, out var result);

        if (!newValue)
        {
            result = 10;
            CardValue += result;
        }
        else
        {
            CardValue += result;
        }

        _hand.Add(newCard);
        return CardValue;
    }
}