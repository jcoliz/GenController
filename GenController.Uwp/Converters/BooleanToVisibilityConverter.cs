using System;
using Windows.UI.Xaml;
using Commonality.Converters;

namespace GenController.Uwp.Converters
{
    public class DefaultToVisibilityConverter : DefaultConverter<Visibility>
    {
        public DefaultToVisibilityConverter(): base(Visibility.Visible, Visibility.Collapsed)
        {}

    }

    public class DefaultVisibleXaml: XamlValueConverter<DefaultToVisibilityConverter>
    {
    }

    public class DefaultToHiddenConverter : DefaultConverter<Visibility>
    {
        public DefaultToHiddenConverter(): base(Visibility.Collapsed, Visibility.Visible)
        { }
    }

    public class DefaultHiddenXaml : XamlValueConverter<DefaultToHiddenConverter>
    {
    }
}
