using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using GameCook.DirtSweeper.State;
using GameCook.DirtSweeper.Utils;
using Microsoft.Phone.Controls;
using MineSweeper;

namespace GameCook.DirtSweeper.Pages
{
    public partial class NewGamePage : PhoneApplicationPage
    {
        private readonly GameState gameState = new GameState();
        private Image[] imageViewInstances;
       
        public NewGamePage()
        {
            InitializeComponent();

            imageViewInstances = new Image[]{ dino1, dino2, dino3, dino4, dino5, dino6, dino7, dino8, dino9, dino10, dino11, dino12 };

        }

        private void RefreshStatDisplays()
        {
            //TODO need to calculate if a new level has been reached

            // Calculate discoveries and next discovery
            var level = gameState.GetLevel();
            var nextLevel = level+1;
            
            // Setup game stats
            totalDigs.Text = String.Format("{0:#,##0;($#,##0.00);0}", gameState.GetTotalGames());
            totalBones.Text = String.Format("{0:#,##0;($#,##0.00);0}", gameState.GetTotalBones());


            discoveries.Text = level + "/12";


            nextDiscovery.Text = level < imageViewInstances.Length ? gameState.GetRewardTotal(nextLevel) + " Bones" : " ";

            
        }

        private void SwapOutFoundDinos()
        {
            
            var level = gameState.GetLevel();

            if (level <= 0) return;
            
            //TODO Double Check this.
            int i;
            var total = level > imageViewInstances.Length ? imageViewInstances.Length : level;

            for (i = 0; i < total; i++)
            {
                imageViewInstances[i].Source = new BitmapImage(new Uri("../../Assets/dinos/dino_" + (i + 1) + "_found.png", UriKind.Relative));
                
            }
            
            //Just make sure we don't attach the listener to a image view that doesn't exist
            if (i >= imageViewInstances.Length)
                i = imageViewInstances.Length - 1;
            
            imageViewInstances[i].ImageOpened += new EventHandler<RoutedEventArgs>(NewGamePage_ImageOpened);
        }

        private void NewGamePage_ImageOpened(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
            scrollViewer.UpdateLayout();
            GeneralTransform transformToVisual = img.TransformToVisual(dinoStackPanel);
            Point point = transformToVisual.Transform(new Point(0, 0));
            scrollViewer.ScrollToHorizontalOffset(point.X - 180);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            var level = gameState.GetLevel();
            var nextLevel = level + 1;
            var totalBonesFound = gameState.GetTotalBones();
            var rewardsTotal = gameState.GetRewardTotal(level + 1);

            

            //TODO harded number of dinos, need to fix that
            if (totalBonesFound >= rewardsTotal && level < 12)
            {
                gameState.IncreaseLevel();
                if ((nextLevel) <= 12)
                    level = nextLevel;

                NewDiscovery.Visibility = Visibility.Visible;
                UnlockDino(level);
            }
            else
            {
                ShowMuseum();
            }

            SwapOutFoundDinos();

            base.OnNavigatedTo(e);

            
        }

        private void ShowMuseum()
        {

            RefreshStatDisplays();
            
            TweenUtil.AnimateInStageLeft(statsBoard, .5);
            TweenUtil.AnimateInStageRight(difficultyButtons, .8);
            difficultyButtons.Visibility = Visibility.Visible;
            SoundUtil.PlayBackgroundLoop("Sounds/DirtSweeperCinematic2.wav");

            if (gameState.GetActiveGame())
            {
                continueGameButton.Visibility = Visibility.Visible;
                TweenUtil.AnimateInStageRight(continueGameButton, 1.8);
            }
            else
            {
                continueGameButton.Visibility = Visibility.Collapsed;
            }

        }

        private void UnlockDino(int level)
        {
            difficultyButtons.Visibility = Visibility.Collapsed;

            var trans = new TranslateTransform { Y = 70 };
            scrollViewer.RenderTransform = trans;
            scrollViewer.IsEnabled = false;
            
            // Page Animations
            TweenUtil.AnimateInStageTop(title, .3);
            TweenUtil.AnimateInStageBottom(continueButton, 1.2);
            TweenUtil.AutoScale(continueButton, .9, true);

            SoundUtil.PlayBackgroundLoop("Sounds/DirtSweeperUnlockAward.wav", 0.8f, false);
            
        }

        private void HideUnlockDino()
        {
            RefreshStatDisplays();

            TweenUtil.AnimateInStageBottom(scrollViewer, .3, true);
            scrollViewer.IsEnabled = true;

            TweenUtil.AnimateOutStageLeft(NewDiscovery);


            double from = 70;

            Storyboard sb;

            var trans = new TranslateTransform { Y = from };
            scrollViewer.RenderTransform = trans;

            sb = new Storyboard();
            sb.BeginTime = TimeSpan.FromSeconds(0);

            var db = new DoubleAnimation();
            db.To = 0;
            db.From = from;
            db.EasingFunction = new SineEase();
            sb.Duration = db.Duration = TimeSpan.FromSeconds(.5);
            sb.Children.Add(db);
            Storyboard.SetTarget(db, trans);
            Storyboard.SetTargetProperty(db, new PropertyPath("Y"));
            sb.Begin();


            difficultyButtons.Visibility = Visibility.Visible;

            TweenUtil.AnimateInStageRight(difficultyButtons, .8);

            SoundUtil.PlayBackgroundLoop("Sounds/DirtSweeperCinematic2.wav");
        }

        private void exit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SoundUtil.LoadSound("Sounds/DirtSweeperButtonPress.wav");
            NavigationService.Navigate(new Uri("/DirtSweeper/Pages/StartPage.xaml", UriKind.Relative));
        }

        private void easy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartGame("easy");
        }

        private void medium_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartGame("medium");
        }

        private void hard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartGame("hard");
        }

        private void StartGame(string difficulty)
        {
            SoundUtil.LoadSound("Sounds/DirtSweeperButtonPress.wav");
            string queryData = string.Format("?difficulty=" + difficulty);
            gameState.ClearActiveGame();

            gameState.Save("activeGame", true);
            gameState.UpdateTotalGames();

            NavigationService.Navigate(new Uri("/DirtSweeper/Pages/GamePage.xaml" + queryData, UriKind.Relative));
        }

        private void clearBones_ButtonClick(object sender, RoutedEventArgs e)
        {
            gameState.SetTotalBones(0);
            gameState.SetLevel(0);

            Uri assetUri;

            var total = imageViewInstances.Length;
            for (int i = 0; i < total; i++)
            {
                assetUri = new Uri("../../Assets/dinos/dino_" + (i + 1) + "_hidden.png", UriKind.Relative);
                imageViewInstances[i].Source = new BitmapImage(assetUri);
            }

            RefreshStatDisplays();
            
        }

        private void incresaseBones_ButtonClick(object sender, RoutedEventArgs e)
        {
            gameState.SetTotalBones(gameState.GetTotalBones()+5);

            RefreshStatDisplays();
        }

        private void decresaseBones_ButtonClick(object sender, RoutedEventArgs e)
        {
            var value = gameState.GetTotalBones() - 5;
            if (value < 0) value = 0;
            gameState.SetTotalBones(value);
            RefreshStatDisplays();
        }

        private void continue_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HideUnlockDino();
        }

        private void continueGame_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SoundUtil.LoadSound("Sounds/DirtSweeperButtonPress.wav");
            NavigationService.Navigate(new Uri("/DirtSweeper/Pages/GamePage.xaml", UriKind.Relative));
        }
    }
}