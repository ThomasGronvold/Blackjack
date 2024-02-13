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
    private static GameParticipant Dealer = new();
    private static GameParticipant Player = new();

    public static void Initialize(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        StartGame();
    }

    /* Initialization Method */

    private static void StartGame()
    {
        // Player and Dealer is alternately dealt two cards each, starting with player

        // Player's First Card 
        HitParticipant();

        // Dealer's first card (isDealer, starts facing down) 
        HitParticipant(true, true);

        // Player's second card 
        HitParticipant();

        // Dealer's second card (isDealer)
        HitParticipant(true);
    }

    /* Public Methods */

    /* Make method work for all participants */
    public static void HitParticipant(bool isDealer = false, bool isFaceDown = false)
    {
        var participant = isDealer ? Dealer : Player;
        var opponent = participant == Player ? Dealer : Player;

        var newCard = Deck.GetNextCard();

        participant.AddCard(newCard, isDealer);

        var cardSum = participant.CardsSum;

        /* The new card get added and displayed to the Window */
        _mainWindow.DealCard(newCard, isDealer, isFaceDown, cardSum);

        if (participant.HasGameInitialized() && opponent.HasGameInitialized())
        {
            CheckGameOver(participant, opponent);
        }
    }

    public static string GetDealerHiddenCardURL()
    {
        return Dealer.DealerHiddenCardUrl();
    }

    /* Private Methods */

    private static void CheckGameOver(GameParticipant participant, GameParticipant opponent)
    {
        var currentParticipantString = participant == Player ? "Player" : "Dealer";
        var opponentParticipantString = participant == Player ? "Dealer" : "Player";

        if (participant.HasBlackjack())
        {
            // if both players has blackjack it's a 
            if (opponent.HasBlackjack())
            {
                GameResultPush();
            }
            else
            {
                GameResultBlackjack(currentParticipantString);
            }
        }

        // if a participants CardsSum after drawing a card is over 21 the participant busts
        else if (participant.CardsSum > 21)
        {
            GameResultBust(opponentParticipantString, currentParticipantString);
        }

        else if (
            participant.CardsSum >= 17 && opponent.CardsSum >= 17 &&
            participant.CardsSum == opponent.CardsSum)
        {
            GameResultPush();
        }

        else if (participant == Dealer && participant.CardsSum >= 17)
        {
            GameResult();
        }
    }

    public static void DealerTurn()
    {
        // The dealers first card should be flipped, and the dealer cardValue should be updated

        while (Dealer.CardsSum < 17)
        {
            HitParticipant(true);
        }
    }

    /* Game Result Methods */
    /* Planned: document every type of win / loss and who won / lost in a database */

    // Checks the cardSum of the player and dealer, the higher sum wins. 
    private static void GameResult()
    {
        if (Player.CardsSum > Dealer.CardsSum)
        {
            _mainWindow.GameResultScreen("Player", "The Player Won!");
        }
        else
        {
            _mainWindow.GameResultScreen("Dealer", "The Dealer Won!");
        }
    }

    private static void GameResultPush()
    {
        // Update database for tied games.
        _mainWindow.GameResultScreen("Tie");
    }

    private static void GameResultBlackjack(string whoWon)
    {
        // Update database for Blackjack games, and on which side.
        _mainWindow.GameResultScreen(whoWon, "Blackjack!");
    }

    private static void GameResultBust(string whoWon, string whoBusted)
    {
        // Update database with games where there is a bust, and on which side.
        _mainWindow.GameResultScreen(whoWon, $"{whoBusted} Busted.");
    }
}