using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;

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
        string[] values =
        {
            /*"2", "3", "4", "5", "6", "7", "8", "9",*/ "10", "J", "Q", "K", "A"
        };

        var cardList =
            from suit in suits
            from value in values
            select new Card(suit, value);

        _cards.AddRange(cardList);

        ShuffleDeck();
    }

    public static void ShuffleDeck()
    {
        _cards.Where(c => c is { FaceCardIdentity: "A", Value: 1 })
            .ToList()
            .ForEach(c => c.ChangeAceValue());

        var n = _cards.Count;
        Random rng = new();

        while (n > 1)
        {
            int k = rng.Next(n--);
            (_cards[n], _cards[k]) = (_cards[k], _cards[n]);
        }

        _nextCardIndex = 0;
    }

    public static Card GetNextCard()
    {
        Card cardToReturn = _cards[_nextCardIndex];
        _nextCardIndex++;

        if (_nextCardIndex >= _cards.Count)
        {
            ShuffleDeck();
        }

        return cardToReturn;
    }
}