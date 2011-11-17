namespace GameApplicationTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The delegate which you have to use 
    /// for any script you want to use!
    /// </summary>
    /// <returns></returns>
    public delegate IEnumerator<float> Script();

    /// <summary>
    /// ScriptManager and a ScriptStateManager
    /// which allows to dynamically create Scripts within 
    /// C#. 
    /// 
    /// This code is based upon Nick Gravlyn's example 
    /// which you can find at: http://blog.nickgravelyn.com/2010/02/the-magic-of-yield/
    /// 
    /// Edited by: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class ScriptManager
    {
        #region Private
        // the currently executing scripts
        private readonly List<ScriptState> scripts = new List<ScriptState>();
        private static ScriptManager instance;
        #endregion

        /// <summary>
        /// Retrieves the current and only instance
        /// of the ScriptManager Object within our 
        /// project.
        /// 
        /// This is represents the singleton pattern.
        /// </summary>
        public static ScriptManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ScriptManager();
                }
                return instance;
            }
        }

        public ScriptManager() {}

        public void ExecuteScript(Script script)
        {
            // wrap the script in our state
            ScriptState scriptState = new ScriptState(script);

            // the script may complete in one go
            scriptState.Execute(null);

            // if not, add it to our list
            if (!scriptState.IsComplete)
            {
                scripts.Add(scriptState);
            }
        }

        public void Update(GameTime gameTime)
        {
            // execute all of our scripts
            foreach (var scriptState in scripts)
            {
                scriptState.Execute(gameTime);
            }

            // remove any completed scripts
            Utils.RemoveAll<ScriptState>(scripts,s => s.IsComplete);
        }

        // a wrapper over the Script delegate to manage sleeping and the enumerator
        private class ScriptState
        {
            private float sleepLength;
            private Script script;
            private IEnumerator<float> scriptEnumerator;

            // the script is complete when we null out our script
            public bool IsComplete { get { return script == null; } }

            public ScriptState(Script script)
            {
                if (script == null)
                    throw new ArgumentNullException("script");

                this.script = script;
            }

            // executes the script until the next sleep time.
            public void Execute(GameTime gameTime)
            {
                // the first run needs to get the script enumerator and first sleepLength (if any)
                if (scriptEnumerator == null)
                {
                    scriptEnumerator = script();
                    sleepLength = scriptEnumerator.Current;
                }

                // if we are sleeping, subtract the time from our timer
                if (sleepLength > 0 && gameTime != null)
                {
                    sleepLength -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                // if the sleep timer is done...
                if (sleepLength <= 0)
                {
                    bool unfinished = false;
                    do
                    {
                        // MoveNext continues execution of our script until the end or until
                        // the next yield return. MoveNext returns true if a yield return is
                        // hit or false if the method is complete.
                        unfinished = scriptEnumerator.MoveNext();
                        sleepLength = scriptEnumerator.Current;

                        // as soon as we are finished or we need to sleep, we exit our loop
                    } while (sleepLength <= 0 && unfinished);

                    // if the script is not unfinished (i.e. is complete), we null out our
                    // script and enumerator which flags the script as IsComplete and lets
                    // the engine clean it up.
                    if (!unfinished)
                    {
                        script = null;
                        scriptEnumerator = null;
                    }
                }
            }
        }
    }
}
