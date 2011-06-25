using System;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Xml.Linq;
using GameCook.DirtSweeper.State;
using GameCook.DirtSweeper.Utils;
using Microsoft.Phone.Controls;
using MineSweeper;

namespace GameCook.DirtSweeper.Pages
{
    public partial class Museum : PhoneApplicationPage
    {
        private readonly GameState gameState;

        public Museum()
        {
            InitializeComponent();
            gameState = new GameState();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            gameState.SetActivePage("CreditsPage.xaml");

            string Version = XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value;
            versionNumber.Text = "Ver " + Version;
        }

    }
}