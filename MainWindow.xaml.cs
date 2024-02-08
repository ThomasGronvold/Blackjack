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

        public void DealCard(Card card, bool dealer = false, bool isFaceDown = false)
        {
            /* Creates and returns an Image element with the correct properties. */
            var dynamicImage = CreateDynamicImage(card, isFaceDown);

            /* Dependent on turn, Already existing cards will be moved to make space
               for the new card, The value count of the cards will be updated,
               And the new Card will be added. */
            if (dealer)
            {
                MoveExistingCards(DealerGrid);
                DealerCardCount.Text = $"{GameManager.Dealer.CardsSum.ToString()} Dealer";
                DealerGrid.Children.Add(dynamicImage);
            }
            else
            {
                MoveExistingCards(PlayerCards);
                PlayerCardCount.Text = $"{GameManager.Player.CardsSum.ToString()} Player";
                PlayerCards.Children.Add(dynamicImage);
            }
        }

        private static Image CreateDynamicImage(Card card, bool isFaceDown = false)
        {
            // Create a new BitmapImage
            string imgUrl = card.GetCardImg();
            var newImage = isFaceDown
                ? new BitmapImage(new Uri("/Images/Cardback.png", UriKind.Relative))
                : new BitmapImage(new Uri($"{imgUrl}", UriKind.Relative));

            // Create a new Image element
            Random random = new Random();
            int tilt = random.Next(-5, 5);
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
        private void MoveExistingCards(Grid cardsToMove)
        {
            var cardImageElements = LogicalTreeHelper
                .GetChildren(cardsToMove)
                .OfType<Image>();

            foreach (var cardImage in cardImageElements)
            {
                Thickness existingMargin = cardImage.Margin;
                double newRight = existingMargin.Right + 55.0;
                Thickness newMargin = new Thickness(existingMargin.Left, existingMargin.Top, newRight,
                    existingMargin.Bottom);
                cardImage.Margin = newMargin;
            }
        }

        // Button clicks methods
        private void HitBtn_OnClick(object sender, RoutedEventArgs e)
        {
            /* HitPlayer method adds a card and validates the state of the game,
             then calls the DealCard method here in window to show the card and update the (visual) value*/
            GameManager.HitPlayer();
        }
    }
}