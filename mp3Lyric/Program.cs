using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace mp3Lyric {
    static class Program {
        static void Main() {
            string currentDir = System.Environment.CurrentDirectory;
            string logPath = currentDir + "\\" + "log.txt";
            string Dir;
            string type;
            string bottom;    // 指定したディレクトリ以下を探索する
            string artistReplace;   // Replacement.txtにinfo.Artistが見つかった時に置き換える
            List<string> error = new List<string>();    // 失敗した曲の情報
            Dictionary<string, string> replacement = new Dictionary<string, string>();
            MyiTunes itunes = new MyiTunes();
            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");

            if ( File.Exists("Replacement.txt")) {
                string[] lines = File.ReadAllLines("Replacement.txt", sjisEnc);
                string[] lineSplit;
                char[] delim = {','};
                foreach (string line in lines) {
                    lineSplit = line.Split(delim);
                    replacement.Add(lineSplit[0], lineSplit[1]);
                }
            }

            Console.WriteLine("////////////////// {0} 実行 //////////////////", DateTime.Now.ToString("F"));
            Logger.Write(string.Format("////////////////// {0} 実行 //////////////////\r\n", DateTime.Now.ToString("F")));

            Console.WriteLine("* ディレクトリとファイルの種類(mp3/m4a)、サブディレクトリ以下も探索対象とするか(y/n)を指定してください(Ctrl+Zで終了)。");
            Console.WriteLine("* 指定しない場合、このプログラムのあるディレクトリが指定されます。");
            Console.Write("ディレクトリ? > ");
            while ((Dir = Console.ReadLine()) != null) {
                List<FileInfo> fileInfoList = new List<FileInfo>();

                Console.Write("種類? > ");
                type = Console.ReadLine();
                if( type == null ) break;
                Console.Write("サブディレクトリ以下も？ > ");
                bottom = Console.ReadLine();
                if (bottom == null) break;

                MusicInfo[] infoAry;
                if (bottom == "y" || bottom == "Y" || bottom == "yes" || bottom == "Yes") {
                    myDirectory.GetAllFilesToBottom(Dir, type, ref fileInfoList);
                    infoAry = myDirectory.GetID3(fileInfoList);
                } else {
                    infoAry = myDirectory.GetID3(Dir, type);
                }
                if (infoAry == null) {
                    Console.WriteLine();
                    Console.Write("ディレクトリ? > ");
                    continue;
                }

                foreach (var info in infoAry) {
                    if (replacement.ContainsKey(info.Artist)) artistReplace = replacement[info.Artist];
                    else artistReplace = info.Artist;

                    Console.WriteLine();
                    Console.WriteLine("アーティスト名 : {0}", artistReplace);
                    Console.WriteLine("アルバム名　　 : {0}", info.Album);
                    Console.WriteLine("曲名　　　　　 : {0}", info.Title);
                    Logger.Write(string.Format(
                               "\r\nアーティスト名 : {0}\r\nアルバム名　　 : {1}\r\n曲名　　　　　 : {2}\r\n"
                               , artistReplace, info.Album, info.Title));

                    LyricGetter lg = new LyricGetter(artistReplace, info.Title);
                    info.Lyric = lg.GetLyric();
                    if (info.Lyric == null) {
                        error.Add(string.Format(
                               "アーティスト名 : {0}\r\nアルバム名　　 : {1}\r\n曲名　　　　　 : {2}\r\n{3}\r\n"
                               , artistReplace, info.Album, info.Title, lg.ProcessRslt));
                        continue;
                    }

                    itunes.SetInfo(info);

                    if (itunes.SetLyric()) {
                        Console.WriteLine("* 成功!");
                        Logger.Write("* 成功!\r\n");
                    } else {
                        error.Add(string.Format(
                               "アーティスト名 : {0}\r\nアルバム名　　 : {1}\r\n曲名　　　　　 : {2}\r\n{3}\r\n"
                               , info.Artist, info.Album, info.Title, itunes.ProcessRslt));
                        continue;
                    }
                }
                Console.WriteLine();
                Console.Write("ディレクトリ? > ");
            }

            Console.WriteLine();
            Console.WriteLine("!---エラー情報---");
            Logger.Write("!---エラー情報---\r\n");
            Console.WriteLine();
            foreach (string item in error) {
                Console.WriteLine(item);
                Console.WriteLine();
                Logger.Write(item + "\r\n");
                Logger.Write("\r\n");
            }
            Logger.Write("\r\n");
        }
    }
}