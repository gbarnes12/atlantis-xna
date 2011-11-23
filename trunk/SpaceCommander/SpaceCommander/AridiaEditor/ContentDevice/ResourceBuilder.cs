using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AridiaEditor.Properties;
using System.IO;
using GameApplicationTools.Resources;
using System.Diagnostics;
using GameApplicationTools;


namespace AridiaEditor.ContentDevice
{
    /// <summary>
    /// 
    /// </summary>
    public class ResourceBuilder : ResourceInformation
    {
        #region Private
        private static ResourceBuilder instance;
        #endregion

        #region Public
        public static ResourceBuilder Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ResourceBuilder();
                }
                return instance;
            }
        }

        public ContentBuilder ContentBuilder { get; set; }

        #region EventDelegates
        public event EventHandler<OnPercentChangedEventArgs> OnPercentChanged;
        public event EventHandler<EventArgs> OnBuildFailed;
        #endregion
        #endregion


        private ResourceBuilder() 
            : base()
        {

        }

        /// <summary>
        /// Loads the entire directory
        /// structure of a project with 
        /// the help of the specified content
        /// directory in Settings. 
        /// </summary>
        private List<ResourceFileTemplate> LoadDirectory()
        {
           return TraverseTree(Settings.Default.ContentPath);
        }
        
        /// <summary>
        /// Builds the entire content files
        /// and creates the temporary directory
        /// for it. This should only be used if
        /// you are aware of what you're doing. 
        /// 
        /// Its main purpose is to build the whole
        /// contents when the editor first runs!
        /// </summary>
        public void BuildContent()
        {
            List<ResourceFileTemplate> files = LoadDirectory();
            if (ContentBuilder != null)
            {
                ContentBuilder.Clear();

                foreach (ResourceFileTemplate file in files)
                {
                    string fullPath = file.File.FullName;
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fullPath);
                    ContentBuilder.Add(fullPath, fileNameWithoutExtension, file.ContentBuilderInformation.Importer, file.ContentBuilderInformation.Processor);
                }

                string buildError = ContentBuilder.Build();

                if (OnPercentChanged != null)
                {
                    OnPercentChanged(this, new OnPercentChangedEventArgs(50));
                }

                if (string.IsNullOrEmpty(buildError))
                {
                    LoadResources(files);
                }
                else
                {
                    // If the build failed, display an error message.
                    Output.AddToError(new Error()
                    {
                        Name = "ResourceBuilder_FAILED_TO_BUILD_FILES",
                        Description = buildError,
                        Type = ErrorType.FATAL
                    });

                    if (OnBuildFailed != null)
                    {
                        OnBuildFailed(this, new EventArgs());
                    }
                }
            }
            else
            {
                Output.AddToError(new Error()
                {
                    Name = "ResourceBuilder_MISSING_CONTENTBUILDER",
                    Description = "It seems that there was no valid content builder supplied!",
                    Type = ErrorType.FATAL
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadResources(List<ResourceFileTemplate> files)
        {
            int pos = files.Count;
            foreach (ResourceFileTemplate file in files)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.File.FullName);
                ResourceManager.Instance.AddResourceEditor(new Resource()
                {
                    Name = fileNameWithoutExtension,
                    Path = GameApplication.Instance.AssetPath,
                    Type = file.ContentBuilderInformation.Type
                });

                if (OnPercentChanged != null)
                {
                    OnPercentChanged(this, new OnPercentChangedEventArgs((100 / (files.Count * 2)) * pos));
                }

                 pos++;
            }
        }


        #region Private Methods
        private List<ResourceFileTemplate> TraverseTree(string root)
        {
            List<ResourceFileTemplate> fileList = new List<ResourceFileTemplate>();

            // Data structure to hold names of subfolders to be
            // examined for files.
            Stack<string> dirs = new Stack<string>(20);

            if (!System.IO.Directory.Exists(root))
            {
                throw new ArgumentException();
            }
            dirs.Push(root);

            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                string[] subDirs;
                try
                {
                    subDirs = System.IO.Directory.GetDirectories(currentDir);
                }
                // An UnauthorizedAccessException exception will be thrown if we do not have
                // discovery permission on a folder or file. It may or may not be acceptable 
                // to ignore the exception and continue enumerating the remaining files and 
                // folders. It is also possible (but unlikely) that a DirectoryNotFound exception 
                // will be raised. This will happen if currentDir has been deleted by
                // another application or thread after our call to Directory.Exists. The 
                // choice of which exceptions to catch depends entirely on the specific task 
                // you are intending to perform and also on how much you know with certainty 
                // about the systems on which this code will run.
                catch (UnauthorizedAccessException e)
                {
                    Output.AddToError(new Error()
                    {
                        Type = ErrorType.FATAL,
                        Name = "UnauthorizedAccessException",
                        Description = e.Message
                    });
                    continue;
                }
                catch (System.IO.DirectoryNotFoundException e)
                {
                    Output.AddToError(new Error()
                    {
                        Type = ErrorType.FATAL,
                        Name = "DirectoryNotFoundException",
                        Description = e.Message
                    });
                    continue;
                }

                string[] files = null;
                try
                {
                    files = System.IO.Directory.GetFiles(currentDir);
                }

                catch (UnauthorizedAccessException e)
                {

                    Output.AddToError(new Error()
                    {
                        Type = ErrorType.FATAL,
                        Name = "UnauthorizedAccessException",
                        Description = e.Message
                    });

                    continue;
                }

                catch (System.IO.DirectoryNotFoundException e)
                {
                    Output.AddToError(new Error()
                    {
                        Type = ErrorType.FATAL,
                        Name = "DirectoryNotFoundException",
                        Description = e.Message
                    });

                    continue;
                }
                // Perform the required action on each file here.
                // Modify this block to perform your required task.
                foreach (string file in files)
                {
                    try
                    {
                        // Perform whatever action is required in your scenario.
                        System.IO.FileInfo fi = new System.IO.FileInfo(file);
                        string Extension = fi.Extension.Substring(1);

                        if(extensions.ContainsKey(Extension))
                            fileList.Add(new ResourceFileTemplate() {
                               ContentBuilderInformation = extensions[Extension],
                               File = fi
                            });
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        // If file was deleted by a separate application
                        //  or thread since the call to TraverseTree()
                        // then just continue.
                        Output.AddToError(new Error()
                        {
                            Type = ErrorType.FATAL,
                            Name = "FileNotFoundException",
                            Description = e.Message
                        });
                        continue;
                    }
                }

                // Push the subdirectories onto the stack for traversal.
                // This could also be done before handing the files.
                foreach (string str in subDirs)
                    dirs.Push(str);
            }

            return fileList;
        }
        #endregion
    }

    /// <summary>
    /// Holds the necessary information about the allowed extensions
    /// we can build in 
    /// </summary>
    public class ResourceInformation
    {
        #region Private
        protected Dictionary<string, ResourceConnectedContent> extensions;


        public struct ResourceConnectedContent
        {
            public String Processor { get; set; }
            public String Importer { get; set; }  
            public ResourceType Type { get; set; }
        }

        public struct ResourceFileTemplate
        {
            public ResourceConnectedContent ContentBuilderInformation;
            public System.IO.FileInfo File;
        }
        #endregion

        #region Generation of Resources
        protected ResourceInformation()
        {
            extensions = new Dictionary<string, ResourceConnectedContent>();
            // first we need to create all the resource types
            ResourceConnectedContent Texture2DContent = new ResourceConnectedContent()
            {
                Processor = "TextureProcessor",
                Importer = "TextureImporter",
                Type = ResourceType.Texture2D
            };
            extensions.Add("bmp", Texture2DContent);
            extensions.Add("dds", Texture2DContent);
            extensions.Add("dib", Texture2DContent);
            extensions.Add("hdr", Texture2DContent);
            extensions.Add("jpg", Texture2DContent);
            extensions.Add("png", Texture2DContent);
            extensions.Add("ppm", Texture2DContent);
            extensions.Add("tga", Texture2DContent);


            ResourceConnectedContent ModelFBXContent = new ResourceConnectedContent()
            {
                Processor = "ModelProcessor",
                Importer = "FbxImporter",
                Type = ResourceType.Model
            };
            ResourceConnectedContent ModelXContent = new ResourceConnectedContent()
            {
                Processor = "ModelProcessor",
                Importer = "XImporter",
                Type = ResourceType.Model
            };

            extensions.Add("x", ModelXContent);
            extensions.Add("fbx", ModelFBXContent);

            ResourceConnectedContent EffectContent = new ResourceConnectedContent()
            {
                Processor = "EffectProcessor",
                Importer = "EffectImporter",
                Type = ResourceType.Effect
            };
            extensions.Add("fx", EffectContent);

            ResourceConnectedContent SpriteFontContent = new ResourceConnectedContent()
            {
                Processor = "FontDescriptionProcessor",
                Importer = "FontDescriptionImporter",
                Type = ResourceType.SpriteFont
            };
            extensions.Add("spritefont", SpriteFontContent);

        }
        #endregion
    }

    #region EventArgs
    public class OnPercentChangedEventArgs : EventArgs
    {
        private int m_Percent = 0;

        public int Percent
        {
            get { return m_Percent; }
 
        }


        internal OnPercentChangedEventArgs(int percent)
        {
            Debug.Assert(percent != null);

            m_Percent = percent;
        }
    }
    #endregion
}
