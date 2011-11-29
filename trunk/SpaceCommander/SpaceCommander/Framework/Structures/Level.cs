namespace GameApplicationTools.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The basic structure of a level. 
    /// Use this class to load or save a level file.
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class Level
    {
        #region Public
        public string Name { get; set; }
        #endregion

        public Level() { }

        public void Load(string fileName) { }

        public void Save() { }
    }
}
