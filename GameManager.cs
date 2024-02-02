using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Documents;

namespace WPFBlackjack;

public static class GameManager
{
    private static MainWindow _mainWindow;
    public static Dealer Dealer = new();
    public static Player Player = new();

    public static void Initialize(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;



        StartGame();
    }

    private static void StartGame()
    {
        Console.WriteLine("Test");
        // Player is dealt one card

        var player1Card = Deck.GetNextCard();
        _mainWindow.DealCard(player1Card);

        //Dealer is dealt one card face down
        var dealer1Card = Deck.GetNextCard();
        _mainWindow.DealCard(dealer1Card, true);

        // Player is dealth card number 2
        var player2Card = Deck.GetNextCard();
        _mainWindow.DealCard(player2Card);

        // dealer is dealt card number 2
        var dealer2Card = Deck.GetNextCard();
        _mainWindow.DealCard(dealer2Card, true);
    }

    private static void HitPlayer()
    {
        // Logic to give an additional card to the player
    }

    private static void StandPlayer()
    {
        // Logic to end the player's turn
    }

    private static void DealerTurn()
    {
        // Logic to handle the dealer's turn
    }
}