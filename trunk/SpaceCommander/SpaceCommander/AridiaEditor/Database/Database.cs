using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AridiaEditor.Databases.Data;
using System.IO;
using System.Xml;


namespace AridiaEditor.Databases
{
    public class Database
    {
        #region Private
        private static Database instance;
        #endregion

        #region Public
        public static Database Instance
        {
            get
            {
                if (instance == null)
                    instance = new Database();

                return instance;
            }
        }
        public List<Shader> Shaders;
        public Dictionary<String, ModelProcessorData> ModelProcessorData;
        #endregion


        private Database()
        {
            Shaders = new List<Shader>();
            ModelProcessorData = new Dictionary<String, ModelProcessorData>();
        }

        public void LoadData()
        {
            LoadShaders();
            LoadModelProcessor();
        }

        private void LoadModelProcessor()
        {
            XmlDocument processor = new XmlDocument();
            processor.Load("Content\\ModelProcessorData.xml");

            foreach (XmlNode node in processor["Models"].ChildNodes)
            {
                ModelProcessorData data = new ModelProcessorData();
                data.Name = node.Attributes["Name"].InnerText;
                data.GenerateTangentFrames = bool.Parse(node.Attributes["GenerateTangentFrames"].InnerText);
                ModelProcessorData.Add(data.Name, data);
            }
        }

        private void LoadShaders()
        {
            string[] fileEntries = Directory.GetFiles("Content\\Shader\\");
            foreach (string fileName in fileEntries)
            {
                // do something with fileName
                XmlDocument shader = new XmlDocument();
                shader.Load(fileName);
                
                Shader shaderObject = new Shader();
                shaderObject.Name = shader["Shader"]["Name"].InnerText;
                shaderObject.Description = shader["Shader"]["Description"].InnerText;
                shaderObject.File = shader["Shader"]["File"].InnerText;

                string type = shader["Shader"]["Material"]["Type"].InnerText;
                switch (type)
                {
                    case "Engine":
                        shaderObject.Type = MaterialType.Engine;
                        shaderObject.Content = shader["Shader"]["Material"]["Content"].InnerText;
                        break;
                    case "None":
                        shaderObject.Type = MaterialType.None;
                        shaderObject.Content = shader["Shader"]["Material"]["Content"].InnerText;
                        break;
                    case "Compile":
                        shaderObject.Type = MaterialType.Compile;
                        shaderObject.Assemblies = new List<ShaderAssembly>();
                        shaderObject.Namespace = shader["Shader"]["Material"]["Content"]["Namespace"].InnerText;
                        shaderObject.Content = shader["Shader"]["Material"]["Content"]["Code"].InnerText;
                        break;
                }



                this.Shaders.Add(shaderObject);
            }
        }
    }
}
