using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace mp3Lyric {
    public static class Logger {
        // ログファイルのパス
        private static readonly string logPath = System.Environment.CurrentDirectory + "log.txt";

        /// <summary>
        /// 指定したファイルへログ情報を追加します。
        /// </summary>
        /// <param name="path">ログファイルのパス</param>
        /// <param name="log">ログ情報</param>
        public static void Write(string path, string log) {
            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");

            File.AppendAllText(path, log, sjisEnc);
        }

        /// <summary>
        /// 実行中のプログラムと同じディレクトリのログファイルにログ情報を追加します。
        /// </summary>
        /// <param name="log">ログ情報</param>
        public static void Write(string log) {
            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");

            File.AppendAllText(logPath, log, sjisEnc);
        }
    }
}
