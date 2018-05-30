using Commonality;
using GenController.Portable.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GenController.Uwp.Controls
{
    /// <summary>
    /// A common title bar control so all the screens look the same
    /// </summary>
    /// <remarks>
    /// Uses these services:
    ///     * IController
    /// </remarks>
    public sealed partial class TitleBar : UserControl
    {
        public Controller Controller => App.ControllerCurrent;

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(String), typeof(TitleBar), new PropertyMetadata(string.Empty));
        public String Title
        {
            get { return (String)GetValue(TitleProperty); }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public string DisplayTitle
        {
            get
            {
                var pagetitle = string.Empty;
                if (!string.IsNullOrEmpty(Title))
                {
                    pagetitle = $": {Title}";
                }
                return $"{ApplicationInfo.Title} {ApplicationInfo.Version}{pagetitle}";
            }
        }

        public TitleBar()
        {
            this.InitializeComponent();
        }

        #region Service Locator services
        private IApplicationInfo ApplicationInfo => Service.Get<IApplicationInfo>();
        #endregion

    }
}
