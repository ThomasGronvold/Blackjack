using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace WPFBlackjack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private int _tempBankAmount = 1000;
        private int _tempPotAmount;

        public MainWindow()
        {
            InitializeComponent();
            //GameManager.Initialize(this);
        }

        /* Public Methods */

        public void DealCard(Card card, bool isDealer, bool isFaceDown, int cardsSum)
        {
            /* Creates and returns an Image element with the correct properties. */
            var dynamicImage = CreateDynamicImage(card, isFaceDown);

            /* Dependent on turn, Already existing cards will be moved to make space
               for the new card, The value count of the cards will be updated,
               And the new Card will be added. */
            if (isDealer)
            {
                MoveExistingCards(DealerCardGrid);
                DealerCardCount.Text = $"{cardsSum} Dealer";
                DealerCardGrid.Children.Add(dynamicImage);
            }
            else
            {
                MoveExistingCards(PlayerCardGrid);
                PlayerCardCount.Text = $"{cardsSum} Player";
                PlayerCardGrid.Children.Add(dynamicImage);
            }
        }

        public void GameResultScreen(string whoWon, string result = "")
        {
            DealerShowCard();

            ActionGrid.Visibility = Visibility.Hidden;

            var isTie = whoWon == "Tie";
            GameOverMainText.Text = isTie ? "Tie!" : $"{whoWon} Won!";
            GameOverSubText.Text = isTie ? "It's a push." : $"{result}";

            // Tie
            if (whoWon == "Tie")
            {
                _tempBankAmount += _tempPotAmount;
            }
            // Blackjack win
            else if (whoWon == "Player" && result == "Blackjack!")
            {
                _tempBankAmount += (int)(_tempPotAmount * 2.5);
            }
            // Normal win
            else if (whoWon == "Player")
            {
                _tempBankAmount += _tempPotAmount * 2;
            }

            _tempPotAmount = 0;

            UpdateBankAndBetTextBlocks();

            NewHandBtn.Visibility = Visibility.Visible;
        }

        public void EndPlayerTurn()
        {
            DealerShowCard();
            ActionGrid.Visibility = Visibility.Hidden;
            GameManager.DealerTurn();
        }

        /* Private Methods */

        private static Image CreateDynamicImage(Card card, bool isFaceDown = false)
        {
            // Create a new BitmapImage
            var imgUrl = card.GetCardImg();
            var newImage = isFaceDown
                ? new BitmapImage(new Uri("/Images/Cardback.png", UriKind.Relative))
                : new BitmapImage(new Uri($"{imgUrl}", UriKind.Relative));

            // Create a new Image element
            var random = new System.Random();
            var tilt = random.Next(-5, 5);
            var dynamicImage = new Image
            {
                Source = newImage,
                Height = 250,
                RenderTransform = new RotateTransform(tilt),
            };

            dynamicImage.SetValue(Grid.ColumnProperty, 1);

            return dynamicImage;
        }

        /* Moves existing cards to the left so that a new card can have the correct Bet
           without blocking the view of the other cards. */
        private static void MoveExistingCards(Grid cardsToMove)
        {
            var cardImageElements = LogicalTreeHelper
                .GetChildren(cardsToMove)
                .OfType<Image>();

            foreach (var cardImage in cardImageElements)
            {
                var existingMargin = cardImage.Margin;
                var newRight = existingMargin.Right + 55.0;
                var newMargin = new Thickness(existingMargin.Left, existingMargin.Top, newRight,
                    existingMargin.Bottom);
                cardImage.Margin = newMargin;
            }
        }

        private void DealerShowCard()
        {
            var dealerHiddenCardImage = DealerCardGrid.Children[0] as Image;
            var dealerHiddenCard = GameManager.GetDealerHiddenCard();
            dealerHiddenCardImage!.Source = new BitmapImage(new Uri(dealerHiddenCard, UriKind.RelativeOrAbsolute));
        }

        private void ChangeActiveGrid()
        {
            if (GameGrid.Visibility == Visibility.Visible)
            {
                GameGrid.Visibility = Visibility.Hidden;
                BettingGrid.Visibility = Visibility.Visible;
            }
            else
            {
                GameGrid.Visibility = Visibility.Visible;
                BettingGrid.Visibility = Visibility.Hidden;
            }
        }

        private void UpdateBankAndBetTextBlocks()
        {
            var bankAmount = $"Bank: ${_tempBankAmount}";
            var betAmount = $"${_tempPotAmount}";

            BankAmount.Text = bankAmount;
            BankAmountBettingGrid.Text = bankAmount;

            BetAmount.Text = betAmount;
            BetAmountBettingGrid.Text = betAmount;
        }

        // Button clicks methods
        private async void HitButton_OnClick(object sender, RoutedEventArgs e)
        {
            /* HitParticipant method adds a card and validates the state of the game,
             then calls the DealCard method here in window to show the card and update the (visual) value*/
            await GameManager.HitParticipantAsync();
            DoubleButton.IsEnabled = false;
        }

        private async void DoubleButton_OnClick(object sender, RoutedEventArgs e)
        {
            /* Gives a user a card with the isDouble parameter which makes the GameManager know the player's turn is over */
            if (_tempBankAmount < _tempPotAmount) return;

            // The bank amount is lowered by the pot amount, and the pot amount is doubled.
            _tempBankAmount -= _tempPotAmount;
            _tempPotAmount *= 2;

            UpdateBankAndBetTextBlocks();

            await GameManager.HitParticipantAsync(isDouble: true);
        }

        private void StandButton_OnClick(object sender, RoutedEventArgs e)
        {
            EndPlayerTurn();
        }

        private void NewHandButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Clear the player's and dealer's cards in the window.
            PlayerCardGrid.Children.Clear();
            DealerCardGrid.Children.Clear();

            // Clear the GameResult text and hide the new game button.
            GameOverMainText.Text = "";
            GameOverSubText.Text = "";
            NewHandBtn.Visibility = Visibility.Hidden;

            // Make the ActionGrid visible again.
            ActionGrid.Visibility = Visibility.Visible;

            // Enable the double button
            DoubleButton.IsEnabled = true;

            // Reinitialize GameManager
            ChangeActiveGrid();
        }

        private async void BettingButton_OnClick(object sender, RoutedEventArgs e)
        {
            Button chipClicked = (Button)sender;
            int chipValue = Convert.ToInt32(chipClicked.Tag);

            // If chipValue is 0 it means the player cleared his bet amount. Bank and Bet will be the same as before the bet.
            // If chipValue is 1 it means the player is ready to play, and the game will start.
            if (chipValue == 0)
            {
                _tempBankAmount += _tempPotAmount;
                _tempPotAmount = 0;
            }
            else if (chipValue == 1)
            {
                // Game start, betting grid gets hidden, and game grids becomes visible.
                ChangeActiveGrid();

                await GameManager.Initialize(this!);
            }
            else if (_tempBankAmount - chipValue >= 0)
            {
                _tempBankAmount -= chipValue;
                _tempPotAmount += chipValue;
            }

            UpdateBankAndBetTextBlocks();
        }
    }
}