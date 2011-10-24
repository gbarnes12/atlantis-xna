using System;
using Xen;

namespace Atlantis
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Application app = new Game())
            {
                app.Run();
            }
        }
    }
#endif
}

