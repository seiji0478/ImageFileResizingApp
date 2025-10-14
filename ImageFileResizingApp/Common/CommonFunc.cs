using System.Diagnostics;
using System.IO;

namespace ImageFileResizingApp.Common
{
    class CommonFunc
    {

        /// <summary>
        /// リサイズ値計算
        /// </summary>
        /// <param name="resizeVal"></param>
        /// <param name="orgVal_1"></param>
        /// <param name="orgVal_2"></param>
        /// <returns></returns>
        public static int CalculateResizeValue(int resizeVal, int orgVal_1, int orgVal_2)
        {
            int calResizeVal = (int)Math.Round((double)resizeVal * orgVal_1 / orgVal_2);
            return calResizeVal;
        }

        /// <summary>
        /// JPEG最適化
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public static void JsonOptimize(string args)
        {
            try
            {
                var process = new Process();
                process.StartInfo.FileName = CommonDef.EXE_FILE_CJPEG;
                process.StartInfo.Arguments = args;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"jpegtran failed:\n{error}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("jpegtranの実行に失敗しました: " + ex.Message);
            }
        }

        /// <summary>
        /// JPEG圧縮
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        /// <param name="quality"></param>
        /// <exception cref="Exception"></exception>
        public static void JsonQualityChange(string args)
        {
            try
            {
                var process = new Process();
                process.StartInfo.FileName = CommonDef.EXE_FILE_CJPEG;
                process.StartInfo.Arguments = args;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new Exception($"cjpeg failed:\n{error}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("cjpegの実行に失敗しました: " + ex.Message);
            }
        }
    }
}
