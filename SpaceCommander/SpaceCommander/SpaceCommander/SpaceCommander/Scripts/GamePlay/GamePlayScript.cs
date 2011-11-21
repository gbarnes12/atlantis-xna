namespace SpaceCommander.Scripts.GamePlay
{
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameApplicationTools.UI;
using GameApplicationTools.Misc;
using GameApplicationTools;
    using Microsoft.Xna.Framework.Media;


    
    public class GamePlayScript
    {
        public static IEnumerator<float> OnCreateEvent()
        {
            #if !XBOX360
            GameApplication.Instance.GetGame().IsMouseVisible = false;
            Logger.Instance.Write("Created GamePlayView", LogType.Info);
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
            GameConsole.Instance.WriteLine("Loaded Content GamePlayView", Color.White);

            #if !XBOX360
            Logger.Instance.Write("Loaded Content GamePlayView", LogType.Info);
            #endif

            yield return 0.0f;
        }

        public static IEnumerator<float> OnUpdateEvent()
        {
            yield return 0.0f;
        }
    }
}
