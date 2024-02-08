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
        // Player is dealt one card
        HitPlayer();

        //Dealer is dealt one card face down
        HitDealer(true);

        // Player is dealt card number 2
        HitPlayer();

        // dealer is dealt card number 2
        HitDealer();
    }

    private static void PlayerLogic()
    {
        if (Player.HasBlackjack())
        {
            /* Call a method from the window to visually show the blackjack win */
            /* Later on update Database with player win and blackjack statistics */
        }

        if (GameManager.Player.CardsSum > 21)
        {
            /* Call a method from the window to visually show the player bust */
            /* Later on update Database with player Loss */
            Console.WriteLine("yo");
        }
        else
        {
            //Console.WriteLine("HAHAH");
        }
    }

    public static void HitPlayer()
    {
        Card newCard = Deck.GetNextCard();
        Player.AddCard(newCard);
        _mainWindow.DealCard(newCard);
        PlayerLogic();
    }

    private static void HitDealer(bool isFaceDown = false)
    {
        Card newCard = Deck.GetNextCard();
        Dealer.AddCard(newCard);
        _mainWindow.DealCard(newCard, true, isFaceDown);
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