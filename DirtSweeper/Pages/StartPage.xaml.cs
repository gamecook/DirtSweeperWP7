using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using GameCook.DirtSweeper.State;
using GameCook.DirtSweeper.Utils;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;

namespace GameCook.DirtSweeper.Pages
{
    public partial class StartPage : PhoneApplicationPage
    {
        private readonly GameState gameState;

        public StartPage()
        {
            InitializeComponent();

            gameState = new GameState();
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

        private void credits_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SoundUtil.LoadSound("Sounds/DirtSweeperButtonPress.wav");
            NavigationService.Navigate(new Uri("/DirtSweeper/Pages/CreditsPage.xaml", UriKind.Relative));
        }

        private void toggleMute_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bool muteValue = !gameState.GetSoundMute();
            gameState.SetSoundMute(muteValue);
            SoundUtil.mute = muteValue;
            refreshMuteButtonDisplay();
        }

        private void refreshMuteButtonDisplay()
        {
            bool muteValue = gameState.GetSoundMute();
            MuteButton.Source =
                new BitmapImage(new Uri("../../Assets/start_page_sound_" + ((!muteValue) ? "on" : "off") + "_up.png",
                                        UriKind.Relative));

            if (muteValue)
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