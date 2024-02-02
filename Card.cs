namespace WPFBlackjack;

using System;
using System.Collections.Generic;
using System.Linq;

public class Card
{
    public string Suit { get; }
    public string Value { get; }

    public Card(string suit, string value)
    {
        Suit = suit;
        Value = value;
    }


    public string GetCardImg()
    {
        return $"/Images/{Value}_of_{Suit}.png";
    }
}