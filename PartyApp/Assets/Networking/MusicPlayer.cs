using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Networking
{
    public class MusicPlayer : MonoBehaviour
    {
        public static MusicPlayer Instance;
        private AudioSource audioSource;

        private string[] AllSongs;
        public MP3File[] asMP3;

        public string CurrentSongFile;
        public string NextSongFile;

        private Queue<AudioClip> playlist;

        public void Start()
        {
            //Init our Instance
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(this);

            playlist = new Queue<AudioClip>();

            //Add an audio source to this game object
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            //Should populate Songs
            HashSet<string> songs = new HashSet<string>();
            GetAllSongs(Directory.GetCurrentDirectory(), new HashSet<string>(), songs);
            List<string> asList = songs.ToList();
            //Sort alphabetically
            asList.Sort(delegate (string songOne, string songTwo) { return Path.GetFileName(songOne).CompareTo(Path.GetFileName(songTwo)); });
            AllSongs = asList.ToArray();
            generateMP3();
        }

        public void Update()
        {
            if (!audioSource.isPlaying)
            {
                PlayNext();
            }
        }

        /// <summary>
        /// Begins the game
        /// </summary>
        public void PlayNext()
        {
            if (playlist.Count < 1)
                return;

            audioSource.clip = playlist.Dequeue();
            audioSource.Play();
            Debug.Log("Playing next");
        }

        /// <summary>
        /// Loads the next song in a coroutine
        /// </summary>
        /// <param name="songIndex"></param>
        public void PickNextSong(int songIndex)
        {
            StartCoroutine(LoadSong(asMP3[songIndex]));
        }

        /// <summary>
        /// Loads a desired song into the INQUEUE
        /// </summary>
        /// <param name="songName"></param>
        /// <returns></returns>
        IEnumerator LoadSong(MP3File file)
        {

            string url = string.Format("file://{0}", file.filePath);
            WWW www = new WWW(url);
            Debug.Log(www.error);
            yield return www;
            Debug.Log("Past yield return");
            AudioClip clip = www.GetAudioClip(false, false);

            CurrentSongFile = Path.GetFileName(file.filePath);
            Debug.Log(clip.length);
            playlist.Enqueue(clip);

        }

        #region MP3 Helpers

        public void generateMP3()
        {
            asMP3 = new MP3File[AllSongs.Length];
            for (int i = 0; i < AllSongs.Length; i++)
            {
                StartCoroutine(GenerateHeader(i));
            }
        }

        IEnumerator GenerateHeader(int songIndex)
        {
            MP3File mp3 = new MP3File(AllSongs[songIndex]);
            mp3.GenerateMetaData();
            asMP3[songIndex] = mp3;
            yield return null;
        }

        #endregion

        #region Directory Mapping

        /// <summary>
        /// Using a directory, navigates through it, finding all media files playable in unity
        /// </summary>
        /// <param name="directory"></param>
        public void GetAllSongs(string directory, HashSet<string> _Processed, HashSet<string> Songs)
        {
            //return if we've been visited
            if (_Processed.Contains(directory))
                return;

            //Add all media files in this directory to song list
            foreach (string song in (GetSongsInCurrentDirectory(directory)))
            {
                Songs.Add(song);
            }
            _Processed.Add(directory);

            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                if (!_Processed.Contains(subDirectory))
                {
                    GetAllSongs(subDirectory, _Processed, Songs);
                }
            }

        }

        /// <summary>
        /// Returns all media files from a current directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public string[] GetSongsInCurrentDirectory(string directory)
        {
            //Get all media files in current directory
            return Directory.GetFiles(directory).Where(x => isMediaFile(Path.GetExtension(x))).ToArray();
        }

        /// <summary>
        /// Checks if an extension is of media file type
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public bool isMediaFile(string fileExtension)
        {
            if (fileExtension.Contains(".mp3"))
                return true;
            return false;
        }

        #endregion

    }
}
