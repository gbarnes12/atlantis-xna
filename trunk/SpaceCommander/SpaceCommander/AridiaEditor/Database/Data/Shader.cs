using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AridiaEditor.Databases.Data
{
    /// <summary>
    /// 
    /// </summary>
    public struct Shader
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string File { get; set; }
        public string Content { get; set; }
        public string Namespace { get; set; }
        public MaterialType Type { get; set; }
        public List<ShaderAssembly> Assemblies { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum MaterialType
    {
        None,
        Engine,
        Compile
    }

    public struct ShaderAssembly
    {
        public string Name;
    }
}
