using System;
using System.Device.Location;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using GameCook.DirtSweeper.State;
using GameCook.DirtSweeper.Utils;
using Microsoft.Advertising.Mobile.UI;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using MineSweeper;

namespace GameCook.DirtSweeper.Pages
{
    public partial class StartPage : PhoneApplicationPage
    {
        private readonly GameState gameState;
        private GeoCoordinateWatcher gcw = null;
        private AdControl adControl;

        public StartPage()
        {
            InitializeComponent();

            gameState = new GameState();

            adControl = AdControlUtil.CreateNewAd();
            adControl.HorizontalAlignment = HorizontalAlignment.Left;
            adControl.VerticalAlignment = VerticalAlignment.Bottom;
            adControl.Margin = new Thickness(10, 0, 0, 10);
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
            gameState.SetActivePage("StartPage.xaml");

            refreshMuteButtonDisplay();

            // Page Animations
            TweenUtil.AnimateInStageLeft(Logo, 1);
            TweenUtil.AnimateInStageBottom(Guy, .8);
            TweenUtil.AnimateInStageLeft(NewGameButton, 1.5);
            TweenUtil.AnimateInStageLeft(MuteButton, 1.8);
            TweenUtil.AnimateInStageLeft(CreditsButton, 2);

            base.OnNavigatedTo(e);
            
        }

        private void newGame_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SoundUtil.LoadSound("Sounds/DirtSweeperButtonPress.wav");
            NavigationService.Navigate(new Uri("/DirtSweeper/Pages/NewGamePage.xaml", UriKind.Relative));
        }

        /*private void continue_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SoundUtil.LoadSound("Sounds/DirtSweeperButtonPress.wav");
            NavigationService.Navigate(new Uri("/DirtSweeper/Pages/GamePage.xaml", UriKind.Relative));
        }*/

        private void credits_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SoundUtil.LoadSound("Sounds/DirtSweeperButtonPress.wav");
            NavigationService.Navigate(new Uri("/DirtSweeper/Pages/CreditsPage.xaml", UriKind.Relative));
        }

        private void toggleMute_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var muteValue = !gameState.GetSoundMute();
            gameState.SetSoundMute(muteValue);
            SoundUtil.mute = muteValue;
            refreshMuteButtonDisplay();
        }

        private void refreshMuteButtonDisplay()
        {

            var muteValue = gameState.GetSoundMute();
            MuteButton.Source = new BitmapImage(new Uri("../../Assets/start_page_sound_" + ((!muteValue) ? "on" : "off") + "_up.png", UriKind.Relative));

            if(muteValue)
            {
                SoundUtil.KillBackgroundLoop();
            }
            else
            {
                if (!MediaPlayer.GameHasControl)
                {
                    MediaPlayer.Pause();
                    App.restoreMusic = true;
                }

                SoundUtil.PlayBackgroundLoop("Sounds/HappyDigGuyTheme.wav");
            }
        }
    }
}