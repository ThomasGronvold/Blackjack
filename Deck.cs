namespace WPFBlackjack;

using System.Collections.Generic;
using System;
using System.Linq;

public static class Deck
{
    private static List<Card> _cards;
    private static int _nextCardIndex;

    static Deck()
    {
        InitializeDeck();
    }

    private static void InitializeDeck()
    {
        _cards = new List<Card>();

        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        string[] values = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

        var cardList =
            from suit in suits
            from value in values
            select new Card(suit, value);

        _cards.AddRange(cardList);

        ShuffleDeck();
    }

    private static void ShuffleDeck()
    {
        int n = _cards.Count;
        Random rng = new Random();

        while (n > 1)
        {
            int k = rng.Next(n--);
            (_cards[n], _cards[k]) = (_cards[k], _cards[n]);
        }
    }

    public static Card GetNextCard()
    {
        Card cardToReturn = _cards[_nextCardIndex];
        _nextCardIndex++;
        return cardToReturn;
    }
}