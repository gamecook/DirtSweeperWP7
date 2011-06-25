using System;
using System.Windows;
using System.Windows.Resources;
using System.Windows.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace GameCook.DirtSweeper.Utils
{
    public class SoundUtil
    {
        private static SoundEffectInstance _ambienceInstance;
        private static String _currentBackgroundSoundPath;

        public static bool mute = false;

        /// <summary>
        /// Loads a wav file into an XNA Framework SoundEffect.
        /// </summary>
        /// <param name="soundFilePath">Relative path to the wav file.</param>
        /// <param name="Sound">The SoundEffect to load the audio into.</param>
        public static SoundEffect LoadSound(String soundFilePath, bool autoPlay = true)
        {
            if (mute)
                return null;

            // For error checking, assume we'll fail to load the file.
            SoundEffect sound = null;

            try
            {
                // Holds informations about a file stream.
                StreamResourceInfo soundFileInfo =
                    Application.GetResourceStream(new Uri(soundFilePath, UriKind.Relative));

                // Create the SoundEffect from the Stream
                sound = SoundEffect.FromStream(soundFileInfo.Stream);
                if (autoPlay)
                {
                    FrameworkDispatcher.Update();
                    sound.Play();
                }
            }
            catch (NullReferenceException)
            {
                // Display an error message
                MessageBox.Show("Couldn't load sound " + soundFilePath);
            }

            return sound;
        }


        /// <summary>
        /// Loads a wav file into an XNA Framework SoundEffect.
        /// Then, creates a SoundEffectInstance from the SoundEffect.
        /// </summary>
        /// <param name="SoundFilePath">Relative path to the wav file.</param>
        /// <param name="Sound">The SoundEffect to load the audio into.</param>
        /// <param name="SoundInstance">The SoundEffectInstance to create from Sound.</param>
        public static SoundEffectInstance LoadSoundInstance(String SoundFilePath, bool autoPlay = true)
        {
            if (mute)
                return null;

            // For error checking, assume we'll fail to load the file.
            SoundEffect sound = null;
            SoundEffectInstance soundInstance = null;

            try
            {
                sound = LoadSound(SoundFilePath, false);
                //FrameworkDispatcher.Update();
                soundInstance = sound.CreateInstance();
                if (autoPlay)
                {
                    FrameworkDispatcher.Update();
                    soundInstance.Play();
                }
            }
            catch (NullReferenceException)
            {
                // Display an error message
                MessageBox.Show("Couldn't load sound instance " + SoundFilePath);
            }

            return soundInstance;
        }

        public static void PlayBackgroundLoop(String sourcePath, float volume = 0.8f, bool loop = true)
        {
            if (mute)
                return;

            //TODO need to make sure sound doesn't play when user has music open

            if (_currentBackgroundSoundPath == sourcePath)
                return;

            if (_ambienceInstance != null)
            {
                _ambienceInstance.Pause();
                _currentBackgroundSoundPath = null;
            }

            _ambienceInstance = LoadSoundInstance(sourcePath, false);
            if (_ambienceInstance != null)
            {
                _ambienceInstance.IsLooped = loop;
                _ambienceInstance.Volume = volume;
                FrameworkDispatcher.Update();
                _ambienceInstance.Play();

                _currentBackgroundSoundPath = sourcePath;
            }
        }

        public static void KillBackgroundLoop()
        {
            if (_ambienceInstance != null) _ambienceInstance.Stop(true);
            _currentBackgroundSoundPath = null;
        }

    }
}