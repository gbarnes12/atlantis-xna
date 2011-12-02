namespace GameApplicationTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Media;
    
    using Resources;

    /// <summary>
    /// This Manager is used to handle the resources within 
    /// our game. This is necessary to provide better loading 
    /// processes and to make loading available within our editor.
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class ResourceManager
    {
        #region Private
        private static ResourceManager instance;
        private Dictionary<string, object> _resources;
        #endregion

        #region Public
        /// <summary>
        /// Returns the singleton instance of the ResourceManager
        /// </summary>
        public static ResourceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ResourceManager();
                }
                return instance;
            }
        }

        public ContentManager Content { get; set; }
        #endregion

        private ResourceManager()
        {
            _resources = new Dictionary<string, object>();
        }

        /// <summary>
        /// Loads a list of resources and adds them
        /// to the Resource Manager!
        /// </summary>
        /// <param name="resources"></param>
        public void LoadResources(List<Resource> resources)
        {
            foreach (Resource resource in resources)
            {
               AddResource(resource); 
            }
        }

        /// <summary>
        /// You can add a new resource with this 
        /// method.
        /// </summary>
        /// <param name="resource"></param>
        public void AddResource(Resource resource)
        {
            try
            {
                // if file isn't already an valid resource 
                // load it into the memory
                if (Content != null)
                {
                    if (!_resources.ContainsKey(resource.Name))
                    {
                        switch (resource.Type)
                        {
                            case ResourceType.Effect:
                                _resources.Add(resource.Name, Content.Load<Effect>(resource.Path + resource.Name));
                                break;
                            case ResourceType.Model:
                                _resources.Add(resource.Name, Content.Load<Model>(resource.Path + resource.Name));
                                break;
                            case ResourceType.Texture2D:
                                _resources.Add(resource.Name, Content.Load<Texture2D>(resource.Path + resource.Name));
                                break;
                            case ResourceType.Texture3D:
                                _resources.Add(resource.Name, Content.Load<Texture3D>(resource.Path + resource.Name));
                                break;
                            case ResourceType.SoundEffect:
                                _resources.Add(resource.Name, Content.Load<SoundEffect>(resource.Path + resource.Name));
                                break;
                            case ResourceType.Song:
                                _resources.Add(resource.Name, Content.Load<Song>(resource.Path + resource.Name));
                                break;
                            case ResourceType.Video:
                                _resources.Add(resource.Name, Content.Load<Video>(resource.Path + resource.Name));
                                break;
                            case ResourceType.SpriteFont:
                                _resources.Add(resource.Name, Content.Load<SpriteFont>(resource.Path + resource.Name));
                                break;
                        }
                    }
                }
                else
                {
                    throw new Exception("There is no valid instance of the ContentManager");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// You can add a new resource with this 
        /// method.
        /// </summary>
        /// <param name="resource"></param>
        public void AddResourceEditor(Resource resource)
        {
            try
            {
                // if file isn't already an valid resource 
                // load it into the memory
                if (Content != null)
                {
                    if (!_resources.ContainsKey(resource.Name))
                    {
                        switch (resource.Type)
                        {
                            case ResourceType.Effect:
                                _resources.Add(resource.Name, Content.Load<Effect>(resource.Name));
                                break;
                            case ResourceType.Model:
                                _resources.Add(resource.Name, Content.Load<Model>(resource.Name));
                                break;
                            case ResourceType.Texture2D:
                                _resources.Add(resource.Name, Content.Load<Texture2D>(resource.Name));
                                break;
                            case ResourceType.Texture3D:
                                _resources.Add(resource.Name, Content.Load<Texture3D>(resource.Name));
                                break;
                            case ResourceType.SoundEffect:
                                _resources.Add(resource.Name, Content.Load<SoundEffect>(resource.Name));
                                break;
                            case ResourceType.Song:
                                _resources.Add(resource.Name, Content.Load<Song>(resource.Name));
                                break;
                            case ResourceType.Video:
                                _resources.Add(resource.Name, Content.Load<Video>(resource.Name));
                                break;
                            case ResourceType.SpriteFont:
                                _resources.Add(resource.Name, Content.Load<SpriteFont>(resource.Name));
                                break;
                        }
                    }
                }
                else
                {
                    throw new Exception("There is no valid instance of the ContentManager");
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Returns a resource object with the given 
        /// type you want. Please make sure that the 
        /// object is of the right type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ID"></param>
        /// <returns></returns>
        public T GetResource<T>(String name)
        {
            try
            {
                // we need to pass over null as third argument 
                // because due to the fact that XBOX 360 needs this 
                // third parameter. 
                return (T)Convert.ChangeType(_resources[name], typeof(T), null);
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Returns a dictionary of resources of the given type
        /// you want to have!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Dictionary<string, T> GetResourcesOfType<T>()
        {
            Dictionary<string, T> dictionary = new Dictionary<string, T>();

            foreach (KeyValuePair<string, object> Obj in _resources)
            {
                if (Obj.Value is T)
                    dictionary.Add(Obj.Key, (T)Convert.ChangeType(Obj.Value, typeof(T), null));
            }

            return dictionary;
        }

        /// <summary>
        /// Returns the whole dictionary of resources
        /// that are loaded.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetResources()
        {
            return _resources;
        }

    }
}
