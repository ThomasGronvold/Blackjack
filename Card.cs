using System;

namespace WPFBlackjack;

public class Card(string suit, string value)
{
    private string Suit { get; } = suit;
    public string FaceCardIdentity { get; } = value;
    public int Value { get; private set; } = GetCardIntValue(value);

    private static int GetCardIntValue(string value)
    {
        if (value == "A") return 11;

        if (value is "J" or "Q" or "K") return 10;

        if (int.TryParse(value, out var result)) return result;

        throw new ArgumentException($"Invalid card value. Value: {value}");
    }

    public string GetCardImg()
    {
        return $"/Images/{FaceCardIdentity}_of_{Suit}.png";
    }

    public void ChangeAceValue()
    {
        Value = Value == 11 ? 1 : 11;
    }
}