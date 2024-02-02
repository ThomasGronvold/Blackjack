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

namespace WPFBlackjack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GameManager.Initialize(this);
        }

        public void DealCard(Card card, bool dealer = false)
        {
            // Create a new BitmapImage
            string imgUrl = card.GetCardImg();
            var newImage = new BitmapImage(new Uri($"{imgUrl}", UriKind.Relative));

            // Create a new Image element
            Random random = new Random();
            int tilt = random.Next(-5, 5);
            var dynamicImage = new System.Windows.Controls.Image
            {
                Source = newImage,
                Height = 250,
                RenderTransform = new RotateTransform(tilt),
                Margin = new Thickness(tilt + 50)
            };

            dynamicImage.SetValue(Grid.ColumnProperty, 1);

            if (dealer)
            {
                MoveExistingCards(DealerGrid);
                UpdateCardCount(card, true);
                DealerGrid.Children.Add(dynamicImage);
            }
            else
            {
                MoveExistingCards(PlayerCards);
                UpdateCardCount(card);
                PlayerCards.Children.Add(dynamicImage);
            }
        }

        private void MoveExistingCards(Grid cardsToMove)
        {
            var cardImageElements = LogicalTreeHelper
                .GetChildren(cardsToMove)
                .OfType<System.Windows.Controls.Image>();

            foreach (var cardImage in cardImageElements)
            {
                Thickness existingMargin = cardImage.Margin;
                double newRight = existingMargin.Right + 55.0;
                Thickness newMargin = new Thickness(existingMargin.Left, existingMargin.Top, newRight, existingMargin.Bottom);
                cardImage.Margin = newMargin;
            }
        }

        private void UpdateCardCount(Card card, bool dealer = false)
        {
            if (!dealer)
                PlayerCardCount.Text = GameManager.Player.UpdateParticipantInfo(card).ToString();
            else
                DealerCardCount.Text = GameManager.Dealer.UpdateParticipantInfo(card).ToString();
        }

        // Button clicks methods
        private void HitBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Card newCard = Deck.GetNextCard();
            DealCard(newCard);
        }
    }
}