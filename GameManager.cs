using System;
using System.Threading.Tasks;

namespace WPFBlackjack;

public static class GameManager
{
    private static MainWindow? _mainWindow;
    private static readonly GameParticipant Dealer = new();
    private static readonly GameParticipant Player = new();

    public static async Task Initialize(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        await ResetGame();
        StartGame();
    }
    
    /* Initialization Method */

    private static async Task ResetGame()
    {
        Player.ResetParticipant();
        Dealer.ResetParticipant();
        await Deck.ShuffleDeck();
    }

    private static async void StartGame()
    {
        // Player and Dealer is alternately dealt two cards each, starting with player

        // Player's First Card 
        await HitParticipantAsync(shouldCheckWin: false);

        // Dealer's first card (isDealer, starts facing down) 
        await HitParticipantAsync(true, true, shouldCheckWin: false);

        // Player's second card 
        await HitParticipantAsync(shouldCheckWin: false);

        // Dealer's second card (isDealer)
        await HitParticipantAsync(true, shouldCheckWin: false);

        CheckBlackjack();
    }

    /* Public Methods */

    public static async Task HitParticipantAsync(bool isDealer = false, bool isFaceDown = false, bool shouldCheckWin = true, bool isDouble = false)
    {
        var participant = isDealer ? Dealer : Player;
        var opponent = participant == Player ? Dealer : Player;

        var newCard = await Deck.GetNextCardAsync();

        participant.AddCard(newCard);

        var cardSum = isDealer && !shouldCheckWin ? newCard.Value : participant.CardsSum;

        /* The new card get added and displayed to the Window */
        _mainWindow!.DealCard(newCard, isDealer, isFaceDown, cardSum);

        if (shouldCheckWin && !CheckGameOver(participant, opponent))
        {
            if (!isDealer && cardSum == 21 || isDouble)
            {
                foreach (var card in Player._hand)
                {
                    Console.WriteLine($"{card.FaceCardIdentity} {card.Value}");
                }
         
                _mainWindow.EndPlayerTurn();
            }
        }
    }

    public static string GetDealerHiddenCard()
    {
        _mainWindow!.DealerCardCount.Text = $"{Dealer.CardsSum} Dealer";
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
        if (
            participant == Dealer &&
            participant.CardsSum >= 17 && opponent.CardsSum >= 17 &&
            participant.CardsSum == opponent.CardsSum)
        {
            GameResultPush();
            return true;
        }
        if (participant == Player || participant.CardsSum < 17) return false;
        GameResult();
        return true;
    }

    private static void CheckBlackjack()
    {
        // if both players has blackjack it's a push.
        if (Player.HasBlackjack() && Dealer.HasBlackjack())
        {
            GameResultPush();
        }
        // Check for Blackjack in Player's and Dealer's hand.
        else if (Player.HasBlackjack())
        {
            GameResultBlackjack("Player");
        }
        else if (Dealer.HasBlackjack())
        {
            GameResultBlackjack("Dealer");
        }
    }

    public static async void DealerTurn()
    {
        // Dealer draws cards until the sum of the hand is 17 or above.
        if (Dealer.CardsSum < 17)
        {
            while (Dealer.CardsSum < 17)
            {
                await HitParticipantAsync(true);
            }
        }
        else
        {
            GameResult();
        }
    }

    /* Private Methods */

    // Checks the cardSum of the player and dealer, the higher sum wins. 
    private static void GameResult()
    {
        if (Player.CardsSum == Dealer.CardsSum)
        {
            GameResultPush();
        }
        else if (Player.CardsSum > Dealer.CardsSum)
        {
            _mainWindow!.GameResultScreen("Player", "The Player Won!");
        }
        else
        {
            _mainWindow!.GameResultScreen("Dealer", "The Dealer Won!");
        }
    }

    private static void GameResultPush()
    {
        _mainWindow!.GameResultScreen("Tie");
    }

    private static void GameResultBlackjack(string whoWon)
    {
        _mainWindow!.GameResultScreen(whoWon, "Blackjack!");
    }

    private static void GameResultBust(string whoWon, string whoBusted)
    {
        _mainWindow!.GameResultScreen(whoWon, $"{whoBusted} Busted.");
    }
}