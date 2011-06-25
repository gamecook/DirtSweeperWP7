using System.Windows.Media;
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
            var adControl = new AdControl("db41e494-3528-4b15-9df7-5f006ec1511b", // ApplicationID
                                          "10019326", // AdUnitID
                                          AdModel.Contextual, // AdModel
                                          true); // RotationEnabled
            // Make the AdControl size large enough that it can contain the image.
            adControl.Width = 480;
            adControl.Height = 80;
            adControl.Background = new SolidColorBrush(Colors.Transparent);

            return adControl;
        }
    }
}