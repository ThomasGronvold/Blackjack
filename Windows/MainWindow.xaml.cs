using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;

namespace WPFBlackjack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            GameManager.Initialize(this);
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

                if (cardsSum == 21)
                {
                    EndPlayerTurn();
                }
            }
        }

        public void GameResultScreen(string whoWon, string result = "")
        {
            DealerShowCard();

            ActionGrid.Visibility = Visibility.Hidden;

            var isTie = whoWon == "Tie";
            GameOverMainText.Text = isTie ? "Tie!" : $"{whoWon} Won!";
            GameOverSubText.Text = isTie ? "It's a push." : $"{result}";

            NewHandBtn.Visibility = Visibility.Visible;
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
            var random = new Random();
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

        /* Moves existing cards to the left so that a new card can have the correct spot
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

        private void EndPlayerTurn()
        {
            DealerShowCard();
            ActionGrid.Visibility = Visibility.Hidden;
            GameManager.DealerTurn();
        }

        // Button clicks methods
        private void HitBtn_OnClick(object sender, RoutedEventArgs e)
        {
            /* HitParticipant method adds a card and validates the state of the game,
             then calls the DealCard method here in window to show the card and update the (visual) value*/
            GameManager.HitParticipant();
        }

        private void DoubleButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ActionGrid.Visibility = Visibility.Hidden;
            GameOverMainText.Text = "Player Won!";
            GameOverSubText.Text = "The Dealer Busted!";
        }

        private void StandButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            EndPlayerTurn();
        }

        private void NewHandButtonBase_OnClick(object sender, RoutedEventArgs e)
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

            // Reinitialize GameManager
            GameManager.Initialize(this);
        }
    }
}