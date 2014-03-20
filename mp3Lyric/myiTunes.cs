using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesLib;

namespace mp3Lyric {
    public class MusicInfo {
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }
        public string Lyric { get; set; }
    }

    /// <summary>
    /// iTunesをどうにかするクラス
    /// </summary>
    public class MyiTunes {
        #region プロパティ

        /// <summary>
        /// 曲情報
        /// </summary>
        public MusicInfo Info {
            get;
            private set;
        }
        private MusicInfo oldInfo {
            get;
            set;
        }

        /// <summary>
        /// 処理結果
        /// </summary>
        public string ProcessRslt {
            get;
            private set;
        }

        /// <summary>
        /// すでにアルバムを検索したかどうか
        /// </summary>
        private bool _isSearchAlbum = false;

        /// <summary>
        /// iTunesAppのインスタンス
        /// </summary>
        private iTunesApp _iTunes = null;

        /// <summary>
        /// アルバムの検索結果
        /// </summary>
        private IITTrackCollection _searchRsltAlbum;

        #endregion

        #region コンストラクタ

        public MyiTunes() {
            _iTunes = new iTunesApp();
            Info = new MusicInfo();
            oldInfo = new MusicInfo();
            Info.Artist = Info.Album = Info.Lyric = Info.Title = null;
            oldInfo = null;
            ProcessRslt = "";
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 処理結果を返します。
        /// </summary>
        /// <returns>処理結果</returns>
        public string GetProcessRslt() {

            return ProcessRslt;
        }

        /// <summary>
        /// 曲情報を入力します。
        /// </summary>
        /// <param name="info">曲情報</param>
        public void SetInfo(MusicInfo info) {
            Info = info;
            ProcessRslt = "";
        }

        /// <summary>
        /// 歌詞を曲に追加します。
        /// </summary>
        /// <returns>成功したかどうか</returns>
        public bool SetLyric() {

            if (!isSet()) {
                Console.Error.WriteLine("! SetInfo()を使って曲情報を入力してください。");
                Logger.Write("! SetInfo()を使って曲情報を入力してください。\r\n");
                ProcessRslt = "SetInfo()を使って曲情報を入力してください。";
                return false;
            }

            // その曲が収録されてるアルバムを検索
            if (!_isSearchAlbum || oldInfo != null && oldInfo.Album != Info.Album) {
                _searchRsltAlbum = _iTunes.LibraryPlaylist.Search(Info.Album, ITPlaylistSearchField.ITPlaylistSearchFieldAlbums);
                if (_searchRsltAlbum == null) {
                    Console.Error.WriteLine("! 指定したアルバムが存在しません。");
                    Logger.Write("! 指定したアルバムが存在しません。\r\n");
                    ProcessRslt = "指定したアルバムが存在しません。";
                    _isSearchAlbum = false;
                    return false;
                }
                _isSearchAlbum = true;
            }

            var song = _searchRsltAlbum.get_ItemByName(Info.Title);
            if (song == null) {
                Console.Error.WriteLine("! 指定した曲が存在しません。");
                Logger.Write("! 指定した曲が存在しません。\r\n");
                ProcessRslt = "指定した曲が存在しません。";
                return false;
            }
            if (((IITFileOrCDTrack)song).Lyrics != null) {
                Console.Error.WriteLine("! すでに歌詞が付けられています。");
                Logger.Write("! すでに歌詞が付けられています。\r\n");
                ProcessRslt = "すでに歌詞が付けられています。";
                return false;
            }
            ((IITFileOrCDTrack)song).Lyrics = Info.Lyric;

            oldInfo = Info;
            return true;
        }

        /// <summary>
        /// 曲情報がすでに入力されているかどうか
        /// </summary>
        /// <returns></returns>
        private bool isSet() {

            return Info.Artist != null;
        }

        #endregion
    }
}
