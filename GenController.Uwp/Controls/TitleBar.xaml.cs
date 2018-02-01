using Commonality;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GenController.Uwp.Controls
{
    /// <summary>
    /// A common title bar control so all the screens look the same
    /// </summary>
    public sealed partial class TitleBar : UserControl
    {
        public Portable.Models.Controller Controller => Portable.Models.Controller.Current as Portable.Models.Controller;

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
                var app = Service.Get<IApplicationInfo>();
                var pagetitle = string.Empty;
                if (!string.IsNullOrEmpty(Title))
                {
                    pagetitle = $": {Title}";
                }
                return $"{app.Title} {app.Version}{pagetitle}";
            }
        }

        public TitleBar()
        {
            this.InitializeComponent();
        }
    }
}
