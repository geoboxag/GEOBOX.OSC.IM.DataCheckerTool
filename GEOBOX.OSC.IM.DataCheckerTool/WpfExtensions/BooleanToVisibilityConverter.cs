using System.Windows;

namespace GEOBOX.OSC.IM.DataCheckerTool.WpfExtensions
{
    public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public BooleanToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed)
        { }
    }
}
