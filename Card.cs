namespace WPFBlackjack;

using System;
using System.Collections.Generic;
using System.Linq;

public class Card
{
    private string Suit { get; }
    public string FaceCardIdentity { get; private set; }
    public int Value { get; private set; }


    public Card(string suit, string value)
    {
        Suit = suit;
        FaceCardIdentity = value;
        Value = GetCardIntValue(value);
    }

    public int GetCardIntValue(string value)
    {
        if (value == "A")
        {
            return 11;
        }

        if (value is "J" or "Q" or "K")
        {
            return 10;
        }

        if (int.TryParse(value, out var result))
        {
            return result;
        }

        throw new ArgumentException($"Invalid card value. Value: {value}");
    }

    public string GetCardImg()
    {
        return $"/Images/{FaceCardIdentity}_of_{Suit}.png";
    }

    public void ChangeAceValue()
    {
        Value = 1;
    }


}