using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace IotHello.Uwp.Controls
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
                throw new NotImplementedException("Need a way to get the app title!");
                //var app = Service.Get<ManiaLabs.IApp>();
                var pagetitle = string.Empty;
                if (!string.IsNullOrEmpty(Title))
                {
                    pagetitle = $": {Title}";
                }
                //return $"{app.Title} {app.Version}{pagetitle}";
            }
        }

        public TitleBar()
        {
            this.InitializeComponent();
        }
    }
}
