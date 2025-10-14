
namespace ImageFileResizingApp.Common
{
    internal class CommonDef
    {
        // ---------------------------------
        // 各種パス
        // ---------------------------------
        public const string DIRECTORY_PATH_SAVE = @"C:\Users\nishi\Downloads\ResizedImages";

        // ---------------------------------
        // 各種外部ツール実行ファイル
        // ---------------------------------
        public const string EXE_FILE_JPEGTRAN = @".\MOZJPEG\jpegtran-static.exe";   // ロスレス変換ツール
        public const string EXE_FILE_CJPEG = @".\MOZJPEG\cjpeg-static.exe";         // JPEGエンコーダー
        public const string EXE_FILE_DJPEG = @".\MOZJPEG\djpeg-static.exe";         // JPEGデコーダー
        public const string EXE_FILE_RDJPGCOM = @".\MOZJPEG\rdjpgcom.exe";          // JPEGコメントリーダー
        public const string EXE_FILE_WDJPGCOM = @".\MOZJPEG\wdjpgcom.exe";          // JPEGコメントライター

        public static readonly List<string> JPEG_EXTENSIONS = new List<string> { ".jpg", ".jpeg", ".JPG", ".JPEG" };

        // ---------------------------------
        // コマンド関連
        // ---------------------------------
        public const string CJPEG_ARGUMENT_OPTIMIZE = @"-optimize -outfile {0} {1}";
        public const string CJPEG_ARGUMENT_QUALITY_CHANGE = @"-quality {0} -outfile {1} {2}";

        // ---------------------------------
        // メッセージ関連
        // ---------------------------------
        public const string MSG_BOX_CAPTION_ERROR = "エラー";
        public const string MSG_BOX_CAPTION_WARNING = "警告";
        public const string MSG_BOX_CAPTION_QUESTION = "確認";

        // ---------------------------------
        // フォーマット
        // ---------------------------------
        public const string FILE_INFO_FORMAT_1 = @"File Name: {0}" +
                                        "\nDimensions: {1}x{2}px" +
                                        "\nSize: {3}MB";
        public const string FILE_INFO_FORMAT_2 = @"File Name: {0}" +
                                       "\nDimensions: {1}x{2}px" +
                                       "\nSize: {3}MB" +
                                       "\nResize: {4}x{5}px";
    }
}
