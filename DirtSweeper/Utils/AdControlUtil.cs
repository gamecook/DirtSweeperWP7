using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Advertising.Mobile.UI;

namespace GameCook.DirtSweeper.Utils
{
    public class AdControlUtil
    {
        public static AdControl CreateNewAd()
        {
            // AD Logic
            // ApplicationID = "Test_client", AdUnitID = "Image480_80", 
            // AdModel = Contextual, RotationEnabled = true
            var adControl = new AdControl("test_client",      // ApplicationID
                                      "Image480_80",      // AdUnitID
                                      AdModel.Contextual, // AdModel
                                      true);              // RotationEnabled
            // Make the AdControl size large enough that it can contain the image.
            adControl.Width = 480;
            adControl.Height = 80;
            adControl.Background = new SolidColorBrush(Colors.Transparent);
            
            return adControl;
        }
    }
}
