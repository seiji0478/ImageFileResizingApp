using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageFileResizingApp.Views
{
    /// <summary>
    /// インタラクションロジック
    /// </summary>
    public partial class ImageViewerWindow : Window
    {
        public ImageViewerWindow(string imagePath)
        {
            InitializeComponent();
            DisplayedImage.Source = new BitmapImage(new Uri(imagePath));
        }
    }
}
