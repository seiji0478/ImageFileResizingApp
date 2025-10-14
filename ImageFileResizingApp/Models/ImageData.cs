using System.Windows.Controls;

namespace ImageFileResizingApp.Models
{
    public class ImageData
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int? ResizedWidth { get; set; }
        public int? ResizedHeight { get; set; }
        public Image ImageControl { get; set; } = new Image();
        public TextBlock FileInfoTextBlock { get; set; } = new TextBlock();
    }
}
