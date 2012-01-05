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
using GameApplicationTools.Actors.Properties;


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
        float angle = 0f;

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
            // set our necessary classes for the game
            GameApplication.Instance.SetGame(this);
            GameApplication.Instance.SetGraphicsDevice(GraphicsDevice);
            ResourceManager.Instance.Content = Content;

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

            sceneGraph = new SceneGraphManager();
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
                    Name = "wedge_p1_diff_v1",
                    Path = GameApplication.Instance.TexturePath + "SpaceShip\\",
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "Kachel2_bump",
                    Path = GameApplication.Instance.TexturePath,
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "p1_wedge",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                }
            };

            ResourceManager.Instance.LoadResources(resources);
            #endregion

            #region CAMERA
            Camera cam = new Camera("fps", new Vector3(0, 0, 4), new Vector3(0, 0.0f, 0));
            cam.LoadContent();
            CameraManager.Instance.CurrentCamera = "fps";
            #endregion

            #region ACTORS
            //Box box = new Box("box", "crate", 1f);
            //box.Position = Vector3.Zero;
            //box.LoadContent();

            //Terrain terrain = new Terrain("terrain", "chunkheightmap", "Kachel2_bump", 1f);
            //terrain.LoadContent();

            EffectProperty effProp = new EffectProperty();
            effProp.Effect = ResourceManager.Instance.GetResource<Effect>("TextureMappingEffect");
            effProp.Effect.Parameters["DiffuseTexture"].SetValue(ResourceManager.Instance.GetResource<Texture2D>("wedge_p1_diff_v1"));
            effProp.Effect.Parameters["CameraPosition"].SetValue(cam.Position);
            effProp.Effect.Parameters["LightDirection"].SetValue(new Vector3(0, 1, -1));
            effProp.Effect.Parameters["SpecularColor"].SetValue(new Vector4(1f, 1f, 1f, 0.1f));
            effProp.Effect.Parameters["SpecularColorActive"].SetValue(true);
            
            MeshObject mesh = new MeshObject("ship", "p1_wedge", 0.001f);
            mesh.Properties.Add(ActorPropertyType.EFFECT, effProp);
            
            mesh.Position = Vector3.Zero;
            
            mesh.Scale = new Vector3(0.001f);
            
            mesh.LoadContent();
            sceneGraph.RootNode.Children.Add(mesh);
            #endregion

            MouseDevice.Instance.ResetMouseAfterUpdate = false;
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

            angle += 0.005f;
            WorldManager.Instance.GetActor("ship").Rotation = Quaternion.CreateFromYawPitchRoll(angle, 0, 0);


            sceneGraph.Update(gameTime);

            KeyboardDevice.Instance.Update();
            MouseDevice.Instance.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            sceneGraph.Render();

            UIManager.Instance.Render(gameTime);

            //make a screenshot
            if ((KeyboardDevice.Instance.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.PrintScreen) && KeyboardDevice.Instance.WasKeyUp(Microsoft.Xna.Framework.Input.Keys.PrintScreen)))
            {
                Utils.makeScreenshot();
            }
        
            base.Draw(gameTime);
        }
    }
}
