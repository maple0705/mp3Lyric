using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Shell32;

namespace mp3Lyric {
    public class myDirectory {

        /// <summary>
        /// 指定したディレクトリ以下の全てのディレクトリから指定した種類(.mp3/.m4a)のファイルの一覧を取得します。
        /// </summary>
        /// <param name="RootDir">ディレクトリ</param>
        /// <param name="type">ファイルの種類</param>
        /// <param name="FileInfoList">(out)ファイルの一覧</param>
        public static void GetAllFilesToBottom(string RootDir, string type, ref List<FileInfo> FileInfoList) {
            List<FileInfo> fiary;
            string[] subDirAry;

            if (RootDir == "") {
                RootDir = System.Environment.CurrentDirectory;
            }
            fiary = GetAllFiles(RootDir, type);
            if (fiary != null) {
                // ばらしてリストに追加
                foreach (FileInfo fi in fiary) {
                    FileInfoList.Add(fi);
                }
            }
            if (!Directory.Exists(RootDir)) return;
            subDirAry = Directory.GetDirectories(RootDir, "*");
            if (subDirAry.Length == 0) return;

            foreach (string subDir in subDirAry) {
                GetAllFilesToBottom(subDir, type, ref FileInfoList);
            }

            return;
        }

        /// <summary>
        /// 指定したディレクトリから指定した種類(.mp3/.m4a)のファイルの一覧を取得します。
        /// </summary>
        /// <param name="Dir">ディレクトリ</param>
        /// <param name="type">ファイルの種類</param>
        /// <returns>ファイルの一覧</returns>
        private static List<FileInfo> GetAllFiles(string Dir, string type) {

            // Comment out to get All type file.
            //if (type != "mp3" && type != "m4a") {
            //    Console.Error.WriteLine("! mp3かm4aのどちらかを指定してください。");
            //    return null;
            //}
            if (Dir == "") {
                Dir = System.Environment.CurrentDirectory;
            }
            // 相対パスを絶対パスに
            if (!Path.IsPathRooted(Dir)) {
                Dir = Path.GetFullPath(Dir);
            }
            ShellClass shell = new ShellClass();
            Folder folder = shell.NameSpace(Dir);
            DirectoryInfo di = new DirectoryInfo(Dir);
            FileInfo[] fiary;

            try {
                fiary = di.GetFiles(string.Format("*.{0}", type));
            } catch (DirectoryNotFoundException) {
                Console.Error.WriteLine("\r\n! 指定したディレクトリが存在しません。");
                Logger.Write("\r\n! 指定したディレクトリが存在しません。\r\n");
                return null;
            }
            if (fiary.Length == 0) {
                Console.Error.WriteLine(
                    string.Format("\r\n! {0}\r\n! このディレクトリに{1}ファイルは存在しません。", Dir, type)
                );
                Logger.Write(
                    string.Format("\r\n! {0}\r\n! このディレクトリに{1}ファイルは存在しません。\r\n", Dir, type)
                );
                return null;
            }

            return fiary.ToList();
        }

        /// <summary>
        /// 与えられたファイル情報一覧からファイルタグの情報を取り出します。
        /// </summary>
        /// <param name="FileInfoList">ファイル一覧情報</param>
        /// <returns>.mp3/m4aファイルタグの情報の一覧</returns>
        public static MusicInfo[] GetID3(List<FileInfo> FileInfoList) {
            ShellClass shell = new ShellClass();
            List<MusicInfo> info = new List<MusicInfo>();

            foreach (FileInfo fi in FileInfoList) {
                Folder folder = shell.NameSpace(fi.DirectoryName);
                MusicInfo mi = new MusicInfo();
                mi.Artist = folder.GetDetailsOf(folder.ParseName(fi.Name), 13); // アーティスト名
                mi.Album = folder.GetDetailsOf(folder.ParseName(fi.Name), 14);  // アルバム名
                mi.Title = folder.GetDetailsOf(folder.ParseName(fi.Name), 21);  // 曲名
                info.Add(mi);
            }

            return info.ToArray();
        }
        
        /// <summary>
        /// 指定したディレクトリから指定した種類(.mp3/.m4a)のファイルタグの情報を取り出します。
        /// </summary>
        /// <param name="Dir">ディレクトリ</param>
        /// <param name="type">ファイルの種類</param>
        /// <returns>.mp3/.m4aファイルタグの情報の一覧</returns>
        public static MusicInfo[] GetID3(string Dir, string type) {
            ShellClass shell = new ShellClass();
            Folder folder = shell.NameSpace(Dir);
            List<FileInfo> fiary = GetAllFiles(Dir, type);

            MusicInfo[] info = new MusicInfo[fiary.Count];
            for (int i = 0; i < fiary.Count; i++) {
                info[i] = new MusicInfo();
                info[i].Artist = folder.GetDetailsOf(folder.ParseName(fiary[i].Name), 13); // アーティスト名
                info[i].Album = folder.GetDetailsOf(folder.ParseName (fiary[i].Name), 14);  // アルバム名
                info[i].Title = folder.GetDetailsOf(folder.ParseName(fiary[i].Name), 21);  // 曲名
            }

            return info;
        }
    }
}
