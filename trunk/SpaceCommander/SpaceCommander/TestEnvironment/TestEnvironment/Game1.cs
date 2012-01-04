using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GameApplicationTools;
using GameApplicationTools.Input;
using GameApplicationTools.Misc;
using GameApplicationTools.Resources;
using GameApplicationTools.Actors.Cameras;
using GameApplicationTools.Actors.Primitives;
using GameApplicationTools.Actors.Advanced;
using TestEnvironment.UserInterface;
using AwesomiumSharp;

namespace TestEnvironment
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SceneGraphManager sceneGraph;
        UIManagerEditor uiManager;


        MouseState thisMouseState;
        MouseState lastMoustState;

        KeyboardState thisKeyboardState;
        KeyboardState lastKeybosrdState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Draw as fast as possible :)



            // set our necessary classes for the game
            GameApplication.Instance.SetGame(this);
            GameApplication.Instance.SetGraphicsDevice(GraphicsDevice);
            //ResourceManager.Instance.Content = Content;

            uiManager = new UIManagerEditor(this, "http://www.youtube.com");
            uiManager.TransparentBackground = true;
            Components.Add(uiManager);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Services.AddService(typeof(SpriteBatch), spriteBatch);

            uiManager.CreateObject("UIEventmanager", "click", webEventManager);
            uiManager.CreateObject("UIEventmanager", "slide", webEventManager);


            /*sceneGraph = new SceneGraphManager();
            sceneGraph.CullingActive = false;

            #region RESOURCES
            List<Resource> resources = new List<Resource>()
            {
                new Resource() {
                    Name = "crate",
                    Path = GameApplication.Instance.TexturePath,
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "DefaultEffect",
                    Path = GameApplication.Instance.EffectPath,
                    Type = ResourceType.Effect
                },
                new Resource() {
                    Name = "TextureMappingEffect",
                    Path = GameApplication.Instance.EffectPath,
                    Type = ResourceType.Effect
                },
                new Resource() {
                    Name = "chunkheightmap",
                    Path = GameApplication.Instance.TexturePath,
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "Kachel2_bump",
                    Path = GameApplication.Instance.TexturePath,
                    Type = ResourceType.Texture2D
                }
            };

            ResourceManager.Instance.LoadResources(resources);
            #endregion

            #region CAMERA
            FPSCamera cam = new FPSCamera("fps", new Vector3(640.0f, 640.0f, 1280.0f), new Vector3(640.0f, 0.0f, 640.0f));
            cam.LoadContent();
            CameraManager.Instance.CurrentCamera = "fps";
            #endregion

            #region ACTORS
            //Box box = new Box("box", "crate", 1f);
            //box.Position = Vector3.Zero;
            //box.LoadContent();

            Terrain terrain = new Terrain("terrain", "chunkheightmap", "Kachel2_bump", 1f);
            terrain.LoadContent();

            sceneGraph.RootNode.Children.Add(terrain);
            #endregion

            MouseDevice.Instance.ResetMouseAfterUpdate = false;*/
        }

        bool RotateModel = false;
        Color diffuse = Color.White;

        public void webEventManager(object sender, WebView.JSCallbackEventArgs e)
        {
            if (e.args[0].ToString() == "rot")
                RotateModel = !RotateModel;

            if (e.args[0].ToString() == "zoom")
                //test.Position = new Vector3(0, 0, -float.Parse(e.args[1].ToString()));

            if (e.args[0].ToString() == "red")
                diffuse.R = byte.Parse(e.args[1].ToString());
            if (e.args[0].ToString() == "green")
                diffuse.B = byte.Parse(e.args[1].ToString());
            if (e.args[0].ToString() == "blue")
                diffuse.G = byte.Parse(e.args[1].ToString());

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (KeyboardDevice.Instance.WasKeyPressed(Keys.Escape))
                this.Exit();

            thisMouseState = Mouse.GetState();
            thisKeyboardState = Keyboard.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Manage the mouse and keyboard for the UI
            if (thisMouseState.LeftButton == ButtonState.Pressed)
                uiManager.LeftButtonDown();

            if (thisMouseState.LeftButton == ButtonState.Released && lastMoustState.LeftButton == ButtonState.Pressed)
                uiManager.LeftButtonUp();

            uiManager.MouseMoved(thisMouseState.X, thisMouseState.Y);
            uiManager.ScrollWheel(thisMouseState.ScrollWheelValue - lastMoustState.ScrollWheelValue);

            if (thisKeyboardState.GetPressedKeys().Length > 0)
                uiManager.KeyPressed(thisKeyboardState.GetPressedKeys()[0]);
            
            uiManager.PushData("", "ShowCubePosition", new JSValue(0), new JSValue(0), new JSValue(0));

            base.Update(gameTime);

            lastMoustState = thisMouseState;
            lastKeybosrdState = thisKeyboardState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

           // sceneGraph.Render();

            //UIManager.Instance.Render(gameTime);

            //make a screenshot
            //if ((KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.PrintScreen) && KeyboardDevice.Instance.WasKeyUp(Microsoft.Xna.Framework.Input.Keys.PrintScreen)))
           // {
           //     Utils.makeScreenshot();
//}
        //
            base.Draw(gameTime);
        }
    }
}
