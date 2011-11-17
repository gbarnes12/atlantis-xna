namespace GameApplicationTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;


    using Microsoft.Xna.Framework.Media;
    using Microsoft.Xna.Framework.Audio;

    public class MediaManager
    {
        #region Private
        private static MediaManager _instance;
        private VideoPlayer _videoPlayer;
        private Dictionary<string, Song> _musics;
        private Dictionary<string, SoundEffect> _sounds;
        private Dictionary<string, Video> _videos;
        #endregion

        #region Public
        /// <summary>
        /// Retrieves the current and only instance
        /// of the MediaManager Object within our 
        /// project.
        /// 
        /// This is represents the singleton pattern.
        /// </summary>
        public static MediaManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MediaManager();
                }

                return _instance;
            }
        }

        public string CurrentSong { get; set; }
        #endregion

        private MediaManager()
        {
            _musics = new Dictionary<string, Song>();
            _sounds = new Dictionary<string, SoundEffect>();
            _videoPlayer = new VideoPlayer();
            CurrentSong = string.Empty;
        }

        /// <summary>
        /// Adds a new music track to the list of 
        /// musics!
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        public void AddMusic(string name, Song file)
        {
            if (_musics != null)
            {
                if (!_musics.ContainsKey(name))
                    _musics.Add(name, file);
                else
                    throw new Exception("There is already a music track with the name: " + name);
            }
            else
                throw new Exception("Sorry but the music dictionary hasn't been initialized just yet.");
        }


        /// <summary>
        /// Adds a new sound effect to the list of 
        /// sounds!
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        public void AddSound(string name, SoundEffect file)
        {
            if (_sounds != null)
            {
                if (!_sounds.ContainsKey(name))
                    _sounds.Add(name, file);
                else
                    throw new Exception("There is already a sound effect with the name: " + name);
            }
            else
                throw new Exception("Sorry but the sound dictionary hasn't been initialized just yet.");
        }

        /// <summary>
        /// Adds a new video to the list of 
        /// videos!
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        public void AddVideo(string name, Video file)
        {
            if (_videos != null)
            {
                if (!_videos.ContainsKey(name))
                    _videos.Add(name, file);
                else
                    throw new Exception("There is already a video with the name: " + name);
            }
            else
                throw new Exception("Sorry but the video dictionary hasn't been initialized just yet.");
        }

        /// <summary>
        /// Just take care of playing a music track.
        /// </summary>
        /// <param name="name"></param>
        public void PlayMusic(string name)
        {
            if (CurrentSong == string.Empty)
            {
                CurrentSong = name;
            }
            else
            {
                if (name != CurrentSong)
                {
                    MediaPlayer.Stop();
                    CurrentSong = name;
                }
                else
                    return;
            }

            MediaPlayer.Play(_musics[CurrentSong]);
        }

        /// <summary>
        /// Will playback a sound effect!
        /// </summary>
        /// <param name="name"></param>
        public void PlaySound(string name)
        {
            _sounds[name].Play();
        }

        /// <summary>
        /// Not implemented yet!
        /// </summary>
        /// <param name="name"></param>
        public void PlayVideo(string name)
        {
            throw new NotImplementedException("This methods hasn't been implemented yet. It will be implemented in some upcoming builds");
        }
    }
}
