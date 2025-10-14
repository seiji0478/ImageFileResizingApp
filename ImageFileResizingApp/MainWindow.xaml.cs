using ImageFileResizingApp.Common;
using ImageFileResizingApp.Models;
using ImageFileResizingApp.Views;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace ImageFileResizingApp
{
    /// <summary>
    /// インタラクションロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            OptimizeCheckBox.IsChecked = true;
            ImgQualityCheckBox.IsChecked = false;
            ImgQualityUpDown.IsEnabled = false;
        }

        /// <summary>
        /// メンバーフィールド
        /// </summary>
        private double currentX = 10;                                           // 現在のX座標
        private double currentY = 10;                                           // 現在のY座標
        private const double imageSpacing = 30;                                 // 間隔
        private const double imageWidth = 240;                                  // 画像幅
        private const double imageHeight = 135;                                 // 画像高さ
        private HashSet<ImageData> imageDataList = new HashSet<ImageData>();    // 画像データリスト
        private HashSet<Grid> imgContainerList = new HashSet<Grid>();           // 画像コンテナーリスト

        /// <summary>
        /// 画像読込みボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoadImages_Click(object sender, RoutedEventArgs e)
        {
            // 画像読込みダイアログ
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files | *.png;*.jpg;*.jpeg;",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // 選択されたファイル数
                int totalFiles = openFileDialog.FileNames.Length;

                // ProgressWindow生成/表示
                ProgressWindow progressWindow = new ProgressWindow
                { 
                    Owner = this,
                };
                progressWindow.Show();

                await Task.Run(() =>
                {
                    int processCount = 0;
                    foreach (var file in openFileDialog.FileNames)
                    {
                        Dispatcher.Invoke(() => AddImageToCanvas(file));
                        processCount++;
                        double progressValue = (double)processCount / totalFiles * 100;
                        Dispatcher.Invoke(() => progressWindow.UpdateProgress(progressValue));
                        Thread.Sleep(100);

                        if (processCount == totalFiles)
                        {
                            Dispatcher.Invoke(() => progressWindow.Close());
                        }
                    }
                });
                MessageBox.Show("画像を読み込みました。", CommonDef.MSG_BOX_CAPTION_QUESTION);
            }
        }

        /// <summary>
        /// 画像追加
        /// </summary>
        /// <param name="filePath"></param>
        private void AddImageToCanvas(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("ファイルが存在しません。", CommonDef.MSG_BOX_CAPTION_ERROR);
                return;
            };

            if (imageDataList.Any(img => img.FilePath == filePath))
            {
                MessageBox.Show("このファイルは既に読込み済みです。", CommonDef.MSG_BOX_CAPTION_ERROR);
                return;
            }
            
            // 画像生成
            BitmapImage bitmap = new BitmapImage(new Uri(filePath));

            // 画像コントロール生成
            Image img = new Image
            {
                Source = bitmap,
                Stretch = Stretch.Uniform,
                Tag = filePath,
                Height= imageHeight,
                Cursor = Cursors.Hand,
                Margin = new Thickness(10),
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = new ScaleTransform(1.0, 1.0),
            };
            img.MouseLeftButtonUp += Image_Click;
            img.MouseEnter += (s, e) =>
            {
                if (s is Image image && image.RenderTransform is ScaleTransform scale)
                {
                    DoubleAnimation animX = new DoubleAnimation(1.1, TimeSpan.FromMilliseconds(120));
                    DoubleAnimation animY = new DoubleAnimation(1.1, TimeSpan.FromMilliseconds(120));
                    scale.BeginAnimation(ScaleTransform.ScaleXProperty, animX);
                    scale.BeginAnimation(ScaleTransform.ScaleYProperty, animY);

                    DoubleAnimation fade = new DoubleAnimation(0.9, TimeSpan.FromMilliseconds(150));
                    image.BeginAnimation(UIElement.OpacityProperty, fade);
                }
            };
            img.MouseLeave += (s, e) =>
            {
                if (s is Image image && image.RenderTransform is ScaleTransform scale)
                {
                    DoubleAnimation animX = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(120));
                    DoubleAnimation animY = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(120));
                    scale.BeginAnimation(ScaleTransform.ScaleXProperty, animX);
                    scale.BeginAnimation(ScaleTransform.ScaleYProperty, animY);

                    DoubleAnimation fade = new DoubleAnimation(1.0, TimeSpan.FromMilliseconds(150));
                    image.BeginAnimation(OpacityProperty, fade);
                }
            };

            // ファイル情報取得
            FileInfo fileInfo = new FileInfo(filePath);

            // ファイル情報テキストブロック
            TextBlock fileInfoTextBlock = new TextBlock
            {
                Text = string.Empty,
                TextAlignment = TextAlignment.Left,
                FontSize = 12,
                Margin = new Thickness(10),
            };

            // 画像データ
            ImageData imageData = new ImageData()
            {
                FilePath = filePath,
                FileName = fileInfo.Name,
                FileSize = fileInfo.Length / 1024 / 1024,
                Width = bitmap.PixelWidth,
                Height = bitmap.PixelHeight,
                ImageControl = img,
                FileInfoTextBlock = fileInfoTextBlock
            };

            // 画像用パネル
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width=270,
                Height = 250,
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 320,
                    ShadowDepth = 5,
                    Opacity = 0.5,
                    BlurRadius = 8
                },
            };
            stackPanel.Children.Add(img);
            stackPanel.Children.Add(fileInfoTextBlock);
            stackPanel.Background = Brushes.White;

            // 変更値取得
            int resizeWidthUpDownVal = ResizeWidthUpDown.Value ?? 0;
            int resizeHeightUpDownVal = ResizeHeightUpDown.Value ?? 0;

            // 有効値確認
            bool resizeByWidth = resizeWidthUpDownVal >= 1;
            bool resizeByHeight = resizeHeightUpDownVal >= 1;

            if (resizeByWidth)
            {
                // 幅指定リサイズ
                int h = CommonFunc.CalculateResizeValue(
                    resizeWidthUpDownVal, 
                    bitmap.PixelHeight, 
                    bitmap.PixelWidth
                    );
                imageData.FileInfoTextBlock.Text = string.Format(
                    CommonDef.FILE_INFO_FORMAT_2,
                    imageData.FileName,
                    imageData.Width,
                    imageData.Height,
                    imageData.FileSize,
                    resizeWidthUpDownVal,
                    h);
                imageData.ResizedWidth = resizeWidthUpDownVal;
                imageData.ResizedHeight = h;
            }
            else if (resizeByHeight)
            {
                // 高さ指定リサイズ
                int calWidth = CommonFunc.CalculateResizeValue(
                  resizeHeightUpDownVal,
                  bitmap.PixelWidth,
                  bitmap.PixelHeight
                  );
                imageData.FileInfoTextBlock.Text = string.Format(
                    CommonDef.FILE_INFO_FORMAT_2,
                    imageData.FileName,
                    imageData.Width,
                    imageData.Height,
                    imageData.FileSize,
                    calWidth,
                    resizeHeightUpDownVal
                    );
                imageData.ResizedWidth = calWidth;
                imageData.ResizedHeight = resizeHeightUpDownVal;
            }
            else
            {
                // リサイズ指定なし
                imageData.FileInfoTextBlock.Text = String.Format(
                    CommonDef.FILE_INFO_FORMAT_1,
                    imageData.FileName,
                    imageData.Width,
                    imageData.Height,
                    imageData.FileSize
                    );
            }

            // 画像データリスト
            imageDataList.Add(imageData);

            // 閉じるボタン
            Button closeButton = new Button
            {
                Content = "X",
                Width = 20,
                Height = 20,
                FontSize = 10,
                Background = Brushes.Red,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                Cursor = Cursors.Hand,
            };
            closeButton .Click += CloseButton_Click;

            // コンテナー
            Grid imgContainer = new Grid
            {
                Width = imageWidth + 20,
                Height = 250,
                Margin = new Thickness(10),
                Tag = filePath
            };
            imgContainer.Children.Add(stackPanel);
            imgContainer.Children.Add(closeButton);
            imgContainerList.Add(imgContainer);

            // キャンバス追加
            Canvas.SetLeft(imgContainer, currentX);
            Canvas.SetTop(imgContainer, currentY);
            ImageCanvas.Children.Add(imgContainer);

            // 座標調整
            currentX += imageWidth + imageSpacing;

            if (currentX + imageWidth > ImageScrollViewer.ViewportWidth 
                && ImageScrollViewer.ViewportWidth > 0)
            {
                currentX = 10;
                currentY += 300;
            }

            double neededHeight = currentY + 250 + 20;
            double neededWidth = currentX + imageWidth + 20;
            ImageCanvas.Height = Math.Max(ImageCanvas.Height, neededHeight);
            ImageCanvas.Width = Math.Max(ImageCanvas.Width, neededWidth);
        }

        /// <summary>
        /// 画像イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image img && img.Tag is string filePath)
            {
                var viewer = new ImageViewerWindow(filePath);
                viewer.Show();
            }
        }

        /// <summary>
        /// 閉じるボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn 
                && btn.Parent is Grid container
                && container.Children[0] is StackPanel panel
                && panel.Children[0] is Image img 
                && img.Tag is string filePath)
            {
                double test = Canvas.GetLeft(container);
                double test2 = Canvas.GetTop(container);
                imageDataList.RemoveWhere(data => data.FilePath == filePath);
                imgContainerList.RemoveWhere(data => data.Tag as string == filePath);
                ImageCanvas.Children.Clear();
                currentX = 10;
                currentY = 10;
                foreach (var imgContainer in imgContainerList)
                {
                    Canvas.SetLeft(imgContainer, currentX);
                    Canvas.SetTop(imgContainer, currentY);
                    ImageCanvas.Children.Add(imgContainer);
                    currentX += imageWidth + imageSpacing;
                    if (currentX + imageWidth > ImageScrollViewer.ViewportWidth
                        && ImageScrollViewer.ViewportWidth > 0)
                    {
                        currentX = 10;
                        currentY += 300;
                    }
                }   
            }
        }

        /// <summary>
        /// リサイズ値変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Null確認
            if (ResizeHeightUpDown == null || ResizeWidthUpDown == null) return;

            // イベント発生元確認
            if (sender is not Xceed.Wpf.Toolkit.IntegerUpDown updown) return;

            // 変更値取得
            int resizeWidthUpDownVal = ResizeWidthUpDown.Value ?? 0;
            int resizeHeightUpDownVal = ResizeHeightUpDown.Value ?? 0;

            // 有効値確認
            bool resizeByWidth = resizeWidthUpDownVal >= 1;
            bool resizeByHeight = resizeHeightUpDownVal >= 1;
            
            // 有効/無効設定
            ResizeHeightUpDown.IsEnabled = !resizeByWidth;
            ResizeWidthUpDown.IsEnabled = !resizeByHeight;

            foreach (ImageData imgData in imageDataList)
            {
                if (resizeByWidth)
                {
                    // 幅指定リサイズ
                    
                    int calHeight = CommonFunc.CalculateResizeValue(resizeWidthUpDownVal, imgData.Height, imgData.Width);
                    imgData.FileInfoTextBlock.Text = "";
                    imgData.FileInfoTextBlock.Text = string.Format(
                        CommonDef.FILE_INFO_FORMAT_2,
                        imgData.FileName,
                        imgData.Width,
                        imgData.Height,
                        imgData.FileSize,
                        resizeWidthUpDownVal,
                        calHeight);
                    imgData.ResizedWidth = resizeWidthUpDownVal;
                    imgData.ResizedHeight = calHeight;
                }
                else if (resizeByHeight)
                {
                    // 高さ指定リサイズ
                    int calWidth = CommonFunc.CalculateResizeValue(resizeHeightUpDownVal, imgData.Width, imgData.Height);
                    imgData.FileInfoTextBlock.Text = "";
                    imgData.FileInfoTextBlock.Text = string.Format(
                        CommonDef.FILE_INFO_FORMAT_2,
                        imgData.FileName,
                        imgData.Width,
                        imgData.Height,
                        imgData.FileSize,
                        calWidth,
                        resizeHeightUpDownVal
                        );
                    imgData.ResizedWidth = calWidth;
                    imgData.ResizedHeight = resizeHeightUpDownVal;
                }
                else
                {
                    // リサイズ指定なし
                    imgData.FileInfoTextBlock.Text = String.Format(
                       CommonDef.FILE_INFO_FORMAT_1,
                       imgData.FileName,
                       imgData.Width,
                       imgData.Height,
                       imgData.FileSize
                       );
                    imgData.ResizedWidth = null;
                    imgData.ResizedHeight = null;
                }
            }
        }

        /// <summary>
        /// 最適化チェックボックス
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptimizeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ImgQualityCheckBox.IsEnabled = false;
            ImgQualityUpDown.IsEnabled = false;
        }

        /// <summary>
        /// 最適化チェックボックス解除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptimizeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ImgQualityCheckBox.IsEnabled = true;
        }

        /// <summary>
        /// 品質変更チェックボックス
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgQualityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ImgQualityUpDown.IsEnabled = true;
            OptimizeCheckBox.IsEnabled = false;
        }

        /// <summary>
        /// 品質変更チェックボックス解除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgQualityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ImgQualityUpDown.IsEnabled = false;
            OptimizeCheckBox.IsEnabled = true; 
        }

        /// <summary>
        /// クリアボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ImageCanvas.Children.Clear();
            imageDataList.Clear();
            currentX = 10;
            currentY = 10;
            ImageCanvas.Height = ImageScrollViewer.ViewportHeight;
            ImageCanvas.Width = ImageScrollViewer.ViewportWidth;
        }

        /// <summary>
        /// 実行ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            // 保存フォルダパス
            string saveFolderPath = CommonDef.DIRECTORY_PATH_SAVE;

            if (imageDataList.Count == 0)
            {
                MessageBox.Show("対象の画像がありません。", CommonDef.MSG_BOX_CAPTION_ERROR);
                return;
            }

            if (Directory.Exists(saveFolderPath))
            {
                MessageBoxResult msgBoxResult = MessageBox.Show(
                    "既にフォルダが存在します。新しいフォルダを生成しますか？",
                    CommonDef.MSG_BOX_CAPTION_QUESTION,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                    );

                if (msgBoxResult == MessageBoxResult.No) return;

                int directoryCnt = 1;
                while (Directory.Exists(saveFolderPath + $"({directoryCnt})"))
                {
                    directoryCnt++;
                }
                saveFolderPath += $"({directoryCnt})";
            }
            Directory.CreateDirectory(saveFolderPath);

            // ProgressWindow生成/表示
            ProgressWindow progressWindow = new ProgressWindow
            {
                Owner = this,
            };
            progressWindow.Show();

            await Task.Run(() =>
            {
                int totalFiles = imageDataList.Count;
                int imgCnt = 1;
                int processCount = 0;
                foreach (var imgData in imageDataList)
                {
                    Dispatcher.Invoke(() => {
                        string extension = Path.GetExtension(imgData.FileName);         // 拡張子
                        bool isJpeg = CommonDef.JPEG_EXTENSIONS.Contains(extension);    // JPEGフラグ
                        string baseName = !string.IsNullOrEmpty(FileNameTextBox.Text)   // ファイル名の指定又は元のファイル名
                            ? FileNameTextBox.Text + "_" + (imgCnt++) + extension
                            : imgData.FileName;
                        string resizeFileSavePath = saveFolderPath + "\\";              // リサイズファイル保存先
                        string processingFileSavePath = saveFolderPath + "\\";          // 品質変更・最適化ファイル保存先

                        // ファイル名決定（共通化）
                        if (ImgQualityCheckBox.IsChecked == true || OptimizeCheckBox.IsChecked == true)
                        {
                            resizeFileSavePath += (!string.IsNullOrEmpty(FileNameTextBox.Text) ?
                                imgData.FileName : "resizeed_" + imgData.FileName);
                            processingFileSavePath += baseName;
                        }
                        else
                        {
                            resizeFileSavePath += baseName;
                        }

                        // BitmapImage インスタンス生成（リサイズ指定がある場合は設定）
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(imgData.FilePath);
                        bitmap.DecodePixelWidth = imgData.ResizedWidth ?? imgData.Width;
                        bitmap.DecodePixelHeight = imgData.ResizedHeight ?? imgData.Height;
                        bitmap.EndInit();

                        // Bitmap エンコーダー生成・保存
                        BitmapEncoder encoder;
                        encoder = isJpeg ? new JpegBitmapEncoder() : new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmap));
                        using (var stream = new FileStream(resizeFileSavePath, FileMode.Create))
                        {
                            encoder.Save(stream);
                        }

                        if (isJpeg)
                        {
                            if (OptimizeCheckBox.IsChecked == true)
                            {
                                // 最適化
                                string args = String.Format(
                                    CommonDef.CJPEG_ARGUMENT_OPTIMIZE,
                                    processingFileSavePath,
                                    resizeFileSavePath
                                    );
                                CommonFunc.JsonOptimize(args);
                                File.Delete(resizeFileSavePath);
                            }
                            else if (ImgQualityCheckBox.IsChecked == true)
                            {
                                // 品質変更
                                int imgQuality = ImgQualityUpDown.Value ?? 100;
                                string args = String.Format(
                                    CommonDef.CJPEG_ARGUMENT_QUALITY_CHANGE,
                                    imgQuality,
                                    processingFileSavePath,
                                    resizeFileSavePath
                                    );
                                CommonFunc.JsonQualityChange(args);
                                File.Delete(resizeFileSavePath);
                            }
                        }

                    });
                    processCount++;
                    double progressValue = (double)processCount / totalFiles * 100;
                    Dispatcher.Invoke(() => progressWindow.UpdateProgress(progressValue));
                    Thread.Sleep(100);

                    if (processCount == totalFiles)
                    {
                        Dispatcher.Invoke(() => progressWindow.Close());
                    }
                }
            });
            MessageBox.Show("画像処理が終了しました。", CommonDef.MSG_BOX_CAPTION_QUESTION);
        }
    }
}