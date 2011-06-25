using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using GameCook.DirtSweeper.Core;
using GameCook.DirtSweeper.State;
using GameCook.DirtSweeper.Utils;
using Microsoft.Advertising.Mobile.UI;
using Microsoft.Phone.Controls;
using System.Device.Location;
using MineSweeper.DirtSweeper.Serialization;

namespace GameCook.DirtSweeper.Pages
{
    public partial class MainPage : PhoneApplicationPage
    {
        
        public enum OverlayStates
        {
            PAUSE,
            LOSE,
            WIN,
        }

        private Storyboard continueStoryboard;
        private GameState gameState;
        private Random oRand = new Random();
        private Storyboard restartStoryboard;
        private bool clickLock;
        private List<Image> covers;
        private Game game;
        private int secs;
        private GeoCoordinateWatcher gcw = null;
        private AdControl adControl;
        // Constructor

        private int level;

        public MainPage()
        {
            InitializeComponent();

            adControl = AdControlUtil.CreateNewAd();
            adControl.HorizontalAlignment = HorizontalAlignment.Right;
            adControl.VerticalAlignment = VerticalAlignment.Top;
            adControl.Margin = new Thickness(0,0,0,0);
            LayoutRoot.Children.Add(adControl);

            this.gcw = new GeoCoordinateWatcher();
            this.gcw.Start();
            this.gcw.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(gcw_PositionChanged);
        }

        void gcw_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            this.gcw.Stop();
            adControl.Location = new Location(e.Position.Location.Latitude, e.Position.Location.Longitude);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var data = NavigationContext.QueryString;

            if (data.ContainsKey("difficulty"))
            {
                if (data["difficulty"] == "medium")
                {
                    level = 1;
                }
                if (data["difficulty"] == "hard")
                {
                    level = 2;
                }
            }

            // Setup button storyboards
            restartStoryboard = TweenUtil.AutoScale(retyButton, .9, false);
            continueStoryboard = TweenUtil.AutoScale(continueButton, .9, false);

            gameState = new GameState();

            double height = LayoutRoot.ActualHeight;
            double width = LayoutRoot.ActualWidth;

            InitByLevel(level);

            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 400); // 100 Milliseconds
            timer.Tick += (sender, args) =>
                    {
                        if (game != null && game.Status == GameStatus.InProgess && game.Begin.HasValue)
                        {
                            secs = (int) Math.Round((DateTime.Now - game.Begin.Value).TotalSeconds);
                            if (secs > 999)
                                secs = 999;

                            
                        }
                    };
            timer.Start();

            base.OnNavigatedTo(e);

            SaveGameState();

            gameState.SetActivePage("GamePage.xaml");

            TweenUtil.AnimateInStageLeft(gameGuy, .3);

            SoundUtil.PlayBackgroundLoop("Sounds/HappyDigGuyTheme.wav");


        }

        private void InitByLevel(int lv)
        {
            switch (lv)
            {
                case 0:
                    InitGame(6, 4, 3, 18, 64);
                    break;
                case 1:
                    InitGame(6, 6, 8, 12, 64);
                    break;
                case 2:
                    InitGame(6, 8, 10, 6, 64);
                    break;
            }
        }

        private void InitGame(int height, int width, int mines, int digs, int tileSize)
        {
            if (gameState.GetGameState() != "")
            {
                var memStream = new MemoryStream(Encoding.UTF8.GetBytes(gameState.GetGameState()));
                var responseSerializer = new DataContractJsonSerializer(typeof (Game));
                game = (Game) responseSerializer.ReadObject(memStream);
                level = game.DifficultyLevel;
                height = game.Height;
                width = game.Width;
                mines = game.Bones;
                //digs = game.Digs;
            }
            else
            {
                game = Game.RandomGame(width, height, mines, level);
            }

            minePanel.Children.Clear();
            minePanel.Height = tileSize*height;
            minePanel.Width = tileSize*width;
            covers = new List<Image>();
            string[] grassTiles = {
                                      "sprite_grass_1.png", "sprite_grass_1.png", "sprite_grass_1.png",
                                      "sprite_grass_1.png", "sprite_grass_1.png", "sprite_grass_1.png",
                                      "sprite_grass_2.png", "sprite_grass_3.png", "sprite_grass_4.png",
                                      "sprite_grass_5.png", "sprite_grass_6.png"
                                  };
            string[] holeTiles = {
                                     "sprite_hole_2.png", "sprite_hole_3.png", "sprite_hole_4.png", "sprite_hole_5.png",
                                     "sprite_hole_5.png"
                                 };
            Uri assetUri;

            double topLeftValue = 0.0;
            double topValue = 0.0;

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    Tile tile = game.GetTile(i, j);
                    Image tileSprite = null;
                    if (tile.IsMine)
                    {
                        assetUri = new Uri("../../Assets/sprites/sprite_bone_1.png", UriKind.Relative);
                        tileSprite = createTileImage(assetUri);
                    }
                    else if (tile.SurroundMines > 0)
                    {
                        assetUri = new Uri("../../Assets/sprites/" + holeTiles[tile.SurroundMines - 1], UriKind.Relative);
                        tileSprite = createTileImage(assetUri);
                    }
                    else
                    {
                        assetUri = new Uri("../../Assets/sprites/sprite_hole_1.png", UriKind.Relative);
                        tileSprite = createTileImage(assetUri);
                    }


                    topLeftValue = j*tileSize;
                    topValue = i*tileSize;

                    //TODO make sure we add in empty holes
                    if (tileSprite != null)
                    {
                        Canvas.SetLeft(tileSprite, topLeftValue);
                        Canvas.SetTop(tileSprite, topValue);
                        minePanel.Children.Add(tileSprite);
                    }

                    int spriteID = oRand.Next(0, grassTiles.Length - 1);
                    Image cover = createTileImage(new Uri("../../Assets/sprites/" + grassTiles[spriteID], UriKind.Relative));
                    cover.Tag = i*width + j;
                    //cover.Height = cover.Width = tileSize;
                    cover.MouseLeftButtonDown += cover_MouseLeftButtonDown;
                    Canvas.SetLeft(cover, topLeftValue);
                    Canvas.SetTop(cover, i*tileSize);
                    minePanel.Children.Add(cover);

                    //For debug
                    cover.Opacity = 1;

                    covers.Add(cover);

                    if (tile.Visited)
                        cover.Visibility = Visibility.Collapsed;
                }
        }

        private Image createTileImage(Uri uri)
        {
            var cover = new Image();
            cover.Source = new BitmapImage(uri);
            return cover;
        }

        private void cover_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (clickLock)
                return;

            var tile = sender as Image;
            var index = (int)tile.Tag;
            

            if (game.GetTile(index).IsMine)
                SoundUtil.LoadSound("Sounds/DirtSweeperFoundNothin.wav");
            else
                SoundUtil.LoadSound("Sounds/DirtDigNoise.wav");

            game.Dig(index);

            UpdateMinePanel();
        }

        private void FadeAnimationCompleted(UIElement element)
        {
            element.Visibility = Visibility.Collapsed;
            clickLock = false;
            UpdateMinePanel();
        }

        private void UpdateMinePanel()
        {
            
            foreach (var cover in covers)
            {
                var index = (int)cover.Tag;
                var tile = game.GetTile(index);
                if (tile.Visited)
                    cover.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (game.Status == GameStatus.LOSE)
            {
                showOverlay(OverlayStates.LOSE);
            }
            else if (game.Status == GameStatus.WIN)
            {
                showOverlay(OverlayStates.WIN);
            }
            else
            {
                SaveGameState();
            }
        }

        private void showOverlay(OverlayStates mode)
        {
            clickLock = true;

            

            // Just incase make sure we stop the buttons
            restartStoryboard.Stop();
            continueStoryboard.Stop();

            pauseLayer.Opacity = 0;
            pauseLayer.Visibility = Visibility.Visible;
            finalStats.Visibility = Visibility.Collapsed;

            TweenUtil.AnimateOutStageLeft(gameGuy);

            gameGuyLose.Visibility = gameGuyWin.Visibility = Visibility.Collapsed;

            // update display text
            String message = "";
            elapsedTime.Text = "Total Time: " + secs.ToString() + "s";
            switch (mode)
            {
                case OverlayStates.LOSE:
                    {
                        message = "FAILED DIG!";
                        finalBoneText.Text = "NO BONES COLLECTED";
                        retyButton.Visibility = Visibility.Visible;
                        continueButton.Visibility = Visibility.Collapsed;
                        restartStoryboard.Begin();
                        var mySolidColorBrush = new SolidColorBrush();
                        mySolidColorBrush.Color = Color.FromArgb(255, 230, 224, 59);
                        finalMessageText.Foreground = mySolidColorBrush;
                        gameState.ClearActiveGame();
                        gameGuyLose.Visibility = Visibility.Visible;
                        TweenUtil.AnimateInStageLeft(gameGuyLose, .3);
                        SoundUtil.PlayBackgroundLoop("Sounds/DirtSweeperLose.wav", 0.8f, false);
                        break;
                    }
                case OverlayStates.WIN:
                    {

                        foreach (var cover in covers)
                        {
                            if (cover.Visibility == Visibility.Visible)
                                cover.Visibility = Visibility.Collapsed;
                        }

                        finalBoneText.Text = "TOTAL BONES COLLECTED " + game.Bones;
                        message = "RATING";
                        //TODO need to do star rating
                        retyButton.Visibility = Visibility.Collapsed;
                        continueButton.Visibility = Visibility.Visible;
                        continueStoryboard.Begin();
                        var mySolidColorBrush = new SolidColorBrush();
                        mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 255);
                        finalMessageText.Foreground = mySolidColorBrush;
                        gameState.UpdateTotalBones(game.Bones);
                        gameState.ClearActiveGame();
                        gameGuyWin.Visibility = Visibility.Visible;
                        CalculateRating(game.DifficultyLevel);

                        TweenUtil.AnimateInStageLeft(gameGuyWin, .3);
                        SoundUtil.PlayBackgroundLoop("Sounds/DirtSweeperWin.wav", 0.8f, false);

                        


                        break;
                    }
            }

            finalMessageText.Text = message;

            var sb = new Storyboard();
            var fade = new DoubleAnimation
                           {
                               Duration = new Duration(TimeSpan.FromSeconds(.3)),
                               From = 0.0,
                               To = 1.0,
                               RepeatBehavior = new RepeatBehavior(1)
                           };

            fade.Completed += (s, _) => onShowOverlay(pauseLayer);

            Storyboard.SetTargetProperty(fade, new PropertyPath(OpacityProperty));
            sb.Children.Add(fade);
            Storyboard.SetTarget(sb, pauseLayer);

            sb.Begin();
        }

        private void CalculateRating(int difficltyLevel)
        {
            ratings.Visibility = Visibility.Visible;
            var starInstances = new Image[] {Star1, Star2, Star3};
            double totalTiles = game.tiles.Length;
            double timePerTile = secs / totalTiles;
            var rating = 0;

            if (timePerTile <= .2)
            {
                rating = 3;
            }
            else if (timePerTile > .2 && timePerTile <= .5)
            {
                rating = 2;
            }
            else if (timePerTile > .5 && timePerTile <= .8)
            {
                rating = 1;
            }

            if (rating == 0)
                return;

            for (int i = 0; i < rating; i++)
            {
                starInstances[i].Source = new BitmapImage(new Uri("../../Assets/star_on.png", UriKind.Relative));
            }

        }

        private void hideOverlay()
        {
            if (LayoutRoot.Resources.Contains("s"))
                LayoutRoot.Resources.Remove("s");

            var sb = new Storyboard();
            var fade = new DoubleAnimation
                           {
                               Duration = new Duration(TimeSpan.FromSeconds(.3)),
                               From = 1.0,
                               To = 0.0,
                               RepeatBehavior = new RepeatBehavior(1)
                           };
            fade.Completed += (s, _) => onOverlayHide();

            TweenUtil.AnimateInStageLeft(gameGuy, .3);

            if (gameGuyLose.Visibility == Visibility.Visible)
                TweenUtil.AnimateOutStageLeft(gameGuyLose);
            else
                TweenUtil.AnimateOutStageLeft(gameGuyWin);

            Storyboard.SetTargetProperty(fade, new PropertyPath(OpacityProperty));
            sb.Children.Add(fade);
            Storyboard.SetTarget(sb, pauseLayer);

            sb.Begin();

            SoundUtil.PlayBackgroundLoop("Sounds/HappyDigGuyTheme.wav");
        }

        private void onOverlayHide()
        {
            clickLock = false;
            pauseLayer.Visibility = Visibility.Collapsed;
            finalStats.Visibility = Visibility.Collapsed;
            // Just incase make sure we stop the buttons
            restartStoryboard.Stop();
            continueStoryboard.Stop();
        }

        private void onShowOverlay(UIElement element)
        {
            finalStats.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            level = 0;
            InitByLevel(level);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            level = 1;
            InitByLevel(level);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            level = 2;
            InitByLevel(level);
        }

        private void continue_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            SoundUtil.LoadSound("Sounds/DirtSweeperButtonPress.wav");
            SaveGameState();

            NavigationService.GoBack();
        }

        private void restart_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            SoundUtil.LoadSound("Sounds/DirtSweeperButtonPress.wav");
            hideOverlay();
            gameState.UpdateTotalGames();
            InitByLevel(level);
        }

        private void SaveGameState()
        {
            // serialization
            var ms = new MemoryStream();

            DataContractJSONSerializationUtil.Serialize(ms, game);
            ms.Position = 0;

            var reader = new StreamReader(ms);
            String jsonData = reader.ReadToEnd();
            gameState.SetGameState(jsonData);

            ms.Close();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            var soundMessage = MessageBox.Show("Are you sure you want to leave this game?", "Leave Game", MessageBoxButton.OKCancel);
            if (soundMessage == MessageBoxResult.OK)
            {
                //gameState.ClearActiveGame();
                NavigationService.GoBack();
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}