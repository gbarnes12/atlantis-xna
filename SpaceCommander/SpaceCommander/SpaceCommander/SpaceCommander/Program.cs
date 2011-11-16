using System;

namespace SpaceCommander
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (SpaceCommander game = new SpaceCommander())
            {
                game.Run();
            }
        }
    }
#endif
}

