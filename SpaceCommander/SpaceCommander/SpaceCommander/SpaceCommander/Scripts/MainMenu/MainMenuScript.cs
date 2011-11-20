namespace SpaceCommander.Scripts.MainMenu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using GameApplicationTools;
    using GameApplicationTools.Interfaces;
    using GameApplicationTools.Misc;
    using Microsoft.Xna.Framework.Media;
    using GameApplicationTools.Input;
    using GameApplicationTools.UI;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// This is just an example of a Script file 
    /// for the MenuGameView which creates two functions.
    /// Those just write some message to the Console.
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public static class MainMenuScript
    {
        /// <summary>
        /// Gets called once the game view is created
        /// and just prints it to the console!
        /// </summary>
        /// <returns>float</returns>
        public static IEnumerator<float> OnCreateEvent()
        {
            #if !XBOX360
            MouseDevice.Instance.ResetMouseAfterUpdate = false;
            GameApplication.Instance.GetGame().IsMouseVisible = true;
            Logger.Instance.Write("Created MainMenuGameView", LogType.Info);
            #endif
            yield return 0.0f;
        }

        /// <summary>
        /// Gets called once the game view loads it content
        /// and just prints it to the console!
        /// </summary>
        /// <returns></returns>
        public static IEnumerator<float> OnLoadEvent()
        {
            GameConsole.Instance.WriteLine("Loaded Content MainMenuGameView", Color.White);

            MediaManager.Instance.AddMusic("MainMenu", GameApplication.Instance.GetGame().Content.Load<Song>(GameApplication.Instance.AudioPath + "Sleep Away"));
            yield return 2.0f;

            MediaManager.Instance.PlayMusic("MainMenu");

            #if !XBOX360
            Logger.Instance.Write("Loaded Content MainMenuGameView", LogType.Info);
            #endif
            yield return 0.0f;
        }

        public static IEnumerator<float> OnUpdateEvent()
        {
            yield return 0.0f;   
        }
    }
}
