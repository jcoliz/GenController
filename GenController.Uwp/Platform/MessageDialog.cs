using Windows.UI.Xaml.Controls;

namespace GenController.Uwp.Platform
{
    /// <summary>
    /// IoT doesn't have Windows.UI.Popups, so we need this
    /// </summary>
    public class MessageDialog: ContentDialog
    {
        public MessageDialog(string message, string title)
        {
            Title = title;
            Content = new TextBlock() { Text = message };
            PrimaryButtonText = "OK";
            IsPrimaryButtonEnabled = true;
        }
    }
}
