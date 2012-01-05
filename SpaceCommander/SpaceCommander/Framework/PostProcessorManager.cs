namespace GameApplicationTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna.Framework.Graphics;

    using Resources.PostProcessors;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// 
    /// </summary>
    public class PostProcessorManager
    {
        #region Private
        RenderTarget2D sceneTarget;
        RenderTarget2D tempTarget;
        SpriteBatch sceneDrawer;
        private static PostProcessorManager instance;
        Dictionary<string, PostProcessor> processors;
        public Texture2D finalResult;
        #endregion

        #region Public
        public static PostProcessorManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PostProcessorManager();
                }
                return instance;
            }
        }

        public bool IsEnabled { get; set; }
        #endregion


        private PostProcessorManager()
        {
            processors = new Dictionary<string, PostProcessor>();
            PresentationParameters pp = GameApplication.Instance.GetGraphics().PresentationParameters;
            sceneTarget = new RenderTarget2D(GameApplication.Instance.GetGraphics(), pp.BackBufferWidth, pp.BackBufferHeight, false, 
                GameApplication.Instance.GetGraphics().DisplayMode.Format, DepthFormat.Depth24Stencil8);

            tempTarget = new RenderTarget2D(GameApplication.Instance.GetGraphics(), pp.BackBufferWidth, pp.BackBufferHeight, false,
                GameApplication.Instance.GetGraphics().DisplayMode.Format, DepthFormat.Depth24Stencil8);

            sceneDrawer = new SpriteBatch(GameApplication.Instance.GetGraphics());
            IsEnabled = true;
        }


        /// <summary>
        /// Retrieves an actor within our 
        /// PostProcessor list and casts him to a
        /// given PostProcessor class. 
        /// 
        /// You could use GetActor(ID) instead.
        /// </summary>
        /// <typeparam name="T">The PostProcessor type to be casted to</typeparam>
        /// <param name="ID">The id of the PostProcessor you want to retrieve</param>
        /// <returns>Will return a object of the Type T</returns>
        public T GetProcessor<T>(String ID)
        {
            try
            {
                // we need to pass over null as third argument 
                // because due to the fact that XBOX 360 needs this 
                // third parameter. 
                return (T)Convert.ChangeType(processors[ID], typeof(T), null);
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Returns an processor of the list 
        /// just with the Base PostProcessor class. 
        /// This won't handle casting issues for 
        /// you. 
        /// </summary>
        /// <param name="ID">The id of the PostProcessor you want to retrieve</param>
        /// <returns>Instance of the PostProcessor class within our dictionary.</returns>
        public PostProcessor GetProcessor(String ID)
        {
            if (processors.ContainsKey(ID))
                return processors[ID];
            else
                return null;
        }

        /// <summary>
        /// Returns the whole dictionary of processors 
        /// to some circumstances this can be very
        /// useful and will come in handy. 
        /// </summary>
        /// <returns>The Dictionary<String, Actor></returns>
        public Dictionary<String, PostProcessor> GetProcessors()
        {
            return this.processors;
        }

        /// <summary>
        /// Adds any processor class to the dictionary.
        /// You have to assign an id on your own 
        /// otherwise it will fail.
        /// </summary>
        /// <param name="processor">The instance of the PostProcessor class you want to pass over</param>
        public void AddProcessor(PostProcessor processor)
        {
            if (processors != null)
            {
                if (processor.ID != null || processor.ID != "" || processor.ID != " ")
                {
                    if (!processors.ContainsKey(processor.ID))
                        this.processors.Add(processor.ID, processor);
                    else
                        throw new Exception("There is already an processor with the id: " + processor.ID);
                }
                else
                    throw new Exception("Your processor has no valid ID or even no ID assigned. Please assign some id to it!");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void BeginRender()
        {
            if(IsEnabled)
                GameApplication.Instance.GetGraphics().SetRenderTarget(sceneTarget);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Render()
        {
            if (IsEnabled)
            {
                GameApplication.Instance.GetGraphics().SetRenderTarget(null);
                finalResult = (Texture2D)sceneTarget;
                GameApplication.Instance.GetGraphics().Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Coral, 1.0f, 0);


                foreach (PostProcessor processor in processors.Values)
                {
                    if (processor.Enabled)
                    {
                        GameApplication.Instance.GetGraphics().SetRenderTarget(processor.Target);
                        processor.Update(finalResult); // Update our processor
                        //finalResult = processor.Render(finalResult, sceneDrawer);

                        sceneDrawer.Begin(0, BlendState.Opaque, null, null, null, processor.Effect);
                        sceneDrawer.Draw(finalResult, Vector2.Zero, Color.White);
                        sceneDrawer.End();
                        GameApplication.Instance.GetGraphics().SetRenderTarget(null);
                        finalResult = (Texture2D)processor.Target;
                    }
                }


                sceneDrawer.Begin();
                sceneDrawer.Draw(finalResult, Vector2.Zero, Color.White);
                sceneDrawer.End();
            }
        }

    }
}
