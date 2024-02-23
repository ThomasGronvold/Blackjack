using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Documents;


namespace WPFBlackjack;

public static class GameManager
{
    private static MainWindow _mainWindow;
    private static readonly GameParticipant Dealer = new();
    private static readonly GameParticipant Player = new();

    public static void Initialize(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        ResetGame();
        StartGame();
    }

    /* Initialization Method */

    private static void ResetGame()
    {
        Player.ResetParticipant();
        Dealer.ResetParticipant();
        Deck.ShuffleDeck();
    }

    private static void StartGame()
    {
        // Player and Dealer is alternately dealt two cards each, starting with player

        // Player's First Card 
        HitParticipant(shouldCheckWin: false);

        // Dealer's first card (isDealer, starts facing down) 
        HitParticipant(true, true, shouldCheckWin: false);

        // Player's second card 
        HitParticipant(shouldCheckWin: false);

        // Dealer's second card (isDealer)
        HitParticipant(true, shouldCheckWin: false);

        CheckBlackjack();
    }

    /* Public Methods */

    public static void HitParticipant(bool isDealer = false, bool isFaceDown = false, bool shouldCheckWin = true, bool isDouble = false)
    {
        var participant = isDealer ? Dealer : Player;
        var opponent = participant == Player ? Dealer : Player;

        var newCard = Deck.GetNextCard();

        participant.AddCard(newCard);

        var cardSum = isDealer && !shouldCheckWin ? newCard.Value : participant.CardsSum;

        /* The new card get added and displayed to the Window */
        _mainWindow.DealCard(newCard, isDealer, isFaceDown, cardSum);

        if (shouldCheckWin && !CheckGameOver(participant, opponent))
        {
            if (!isDealer && cardSum == 21 || isDouble)
            {
                _mainWindow.EndPlayerTurn();
            }
        }
    }

    public static string GetDealerHiddenCard()
    {
        _mainWindow.DealerCardCount.Text = $"{Dealer.CardsSum} Dealer";
        return Dealer.DealerHiddenCard();
    }

    /* Private Methods */

    private static bool CheckGameOver(GameParticipant participant, GameParticipant opponent)
    {
        var currentParticipantString = participant == Player ? "Player" : "Dealer";
        var opponentParticipantString = participant == Player ? "Dealer" : "Player";

        // if a participants CardsSum after drawing a card is over 21 the participant busts.
        if (participant.CardsSum > 21)
        {
            GameResultBust(opponentParticipantString, currentParticipantString);
            return true;
        }
        else if (
            participant.CardsSum >= 17 && opponent.CardsSum >= 17 &&
            participant.CardsSum == opponent.CardsSum)
        {
            GameResultPush();
            return true;
        }
        else if (participant == Dealer && participant.CardsSum >= 17)
        {
            GameResult();
            return true;
        }
        return false;
    }

    private static void CheckBlackjack()
    {
        // if both players has blackjack it's a push.
        if (Player.HasBlackjack() && Dealer.HasBlackjack())
        {
            GameResultPush();
        }
        // Check for Blackjack in both Player and Dealer hands.
        else if (Player.HasBlackjack())
        {
            GameResultBlackjack("Player");
        }
        else if (Dealer.HasBlackjack())
        {
            GameResultBlackjack("Dealer");
        }
    }

    public static void DealerTurn()
    {
        // The dealers first card should be flipped, and the dealer cardValue should be updated
        if (Dealer.CardsSum < 17)
        {
            while (Dealer.CardsSum < 17)
            {
                HitParticipant(true);
            }
        }
        else
        {
            GameResult();
        }
    }

    /* Game Result Methods */
    /* Planned: document every type of win / loss and who won / lost in a database */

    // Checks the cardSum of the player and dealer, the higher sum wins. 
    private static void GameResult()
    {
        if (Player.CardsSum == Dealer.CardsSum)
        {
            GameResultPush();
        }
        else if (Player.CardsSum > Dealer.CardsSum)
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