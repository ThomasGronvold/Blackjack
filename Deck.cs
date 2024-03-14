using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace WPFBlackjack;

public static class Deck
{
    private static readonly List<Card> Cards = [];
    private static int _nextCardIndex;

    static Deck()
    {
        InitializeDeck();
    }

    private static void InitializeDeck()
    {
        string[] suits = ["Hearts", "Diamonds", "Clubs", "Spades"];
        string[] values =
        [
            "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A"
        ];

        var cardList =
            from suit in suits
            from value in values
            select new Card(suit, value);

        Cards.AddRange(cardList);

        /* The deck shuffle happens on GameManager initialization */
    }

    public static async Task ShuffleDeck()
    {
        var randomIntegers = await RandomInt.RandomOrderIntArrayApi();

        var cardsCount = Cards.Count;

        /* All aces that had their value changed to 1 wil change back to 11 */
        Cards.Where(c => c is { FaceCardIdentity: "A", Value: 1 })
            .ToList()
            .ForEach(c => c.ChangeAceValue());

        if (randomIntegers != null)
        {
            while (cardsCount > 1)
            {
                int randomIndex = randomIntegers[cardsCount - 1] - 1;

                (Cards[cardsCount - 1], Cards[randomIndex]) = (Cards[randomIndex], Cards[cardsCount - 1]);
                cardsCount--;
            }
        }
        else
        {
            Random rng = new();

            while (cardsCount > 1)
            {
                int randomInt = rng.Next(cardsCount--);
                (Cards[cardsCount], Cards[randomInt]) = (Cards[randomInt], Cards[cardsCount]);
            }
        }
    }

    public static async Task<Card> GetNextCardAsync()
    {
        if (_nextCardIndex >= Cards.Count)
        {
            await ShuffleDeck();
            _nextCardIndex = 0;
        }

        var cardToReturn = Cards[_nextCardIndex];
        _nextCardIndex++;

        return cardToReturn;
    }
}