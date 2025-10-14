using System.Windows;

namespace ImageFileResizingApp.Views
{
    /// <summary>
    /// インタラクションロジック
    /// </summary>
    public partial class ProgressWindow : Window
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// プログレスバー更新
        /// </summary>
        /// <param name="value"></param>
        public void UpdateProgress(double value)
        {
            LoadImagesProgressBar.Value = value;
        }
    }
}
