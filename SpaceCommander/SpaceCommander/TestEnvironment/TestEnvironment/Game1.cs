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
using GameApplicationTools.UI;
using GameApplicationTools.Resources.Shader;


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
        Vector3 lightDirection;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferMultiSampling = true;
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
            sceneGraph.LightingActive = true;

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
                    Name = "NormalMappingEffect",
                    Path = GameApplication.Instance.EffectPath,
                    Type = ResourceType.Effect
                },
                new Resource() {
                    Name = "FogEffect",
                    Path = GameApplication.Instance.EffectPath,
                    Type = ResourceType.Effect
                },
                new Resource() {
                    Name = "PPModel",
                    Path = GameApplication.Instance.EffectPath,
                    Type = ResourceType.Effect
                },
                new Resource() {
                    Name = "PPLight",
                    Path = GameApplication.Instance.EffectPath,
                    Type = ResourceType.Effect
                },
                new Resource() {
                    Name = "PPDepthNormal",
                    Path = GameApplication.Instance.EffectPath,
                    Type = ResourceType.Effect
                },
                new Resource() {
                    Name = "chunkheightmap",
                    Path = GameApplication.Instance.TexturePath,
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "PPLightMesh",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                new Resource() {
                    Name = "brick_normal_map",
                    Path = GameApplication.Instance.TexturePath,
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "Spaceship_nor_hälfte_fertig_NRM",
                    Path = GameApplication.Instance.TexturePath,
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "wedge_p1_diff_v1",
                    Path = GameApplication.Instance.TexturePath + "SpaceShip\\",
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "wedge_p1_diff_v1_normal",
                    Path = GameApplication.Instance.TexturePath + "SpaceShip\\",
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "Kachel2_bump",
                    Path = GameApplication.Instance.TexturePath,
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "Checker",
                    Path = GameApplication.Instance.TexturePath,
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "masonry-wall-normal-map",
                    Path = GameApplication.Instance.TexturePath,
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "level1_skymap",
                    Path = GameApplication.Instance.TexturePath,
                    Type = ResourceType.Texture2D
                },
                new Resource() {
                    Name = "p1_wedge",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                new Resource() {
                    Name = "Raumschiff_tex",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                new Resource() {
                    Name = "Ground",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                new Resource() {
                    Name = "brick_wall",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                new Resource() {
                    Name = "sphere",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                new Resource() {
                    Name = "raumschiff7",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                new Resource() {
                    Name = "wall",
                    Path = GameApplication.Instance.ModelPath,
                    Type = ResourceType.Model
                },
                new Resource() {
                    Name = "Arial",
                    Path = GameApplication.Instance.FontPath,
                    Type = ResourceType.SpriteFont
                }
            };

            ResourceManager.Instance.LoadResources(resources);

            sceneGraph.Lighting.Lights.Add(new PointLight("pointLight01", null, new Vector3(2, 1, 0), Color.Red * .85f, 5000));
            sceneGraph.Lighting.Lights.Add(new PointLight("pointLight02", null, new Vector3(-2, 2, 0), Color.Green * .85f, 5000));
            sceneGraph.Lighting.Lights.Add(new PointLight("pointLight02", null, new Vector3(10, 1, 0), Color.White * .85f, 5000));
            sceneGraph.Lighting.LoadContent();
            #endregion

            #region CAMERA
            FPSCamera cam = new FPSCamera("fps", new Vector3(0, 5, -5), new Vector3(0, 3, 0));
            cam.LoadContent();
            CameraManager.Instance.CurrentCamera = "fps";
            #endregion

            #region ACTORS
            FogMaterial mat = new FogMaterial();
            mat.FogStart = 2000;
            mat.LightDirection = new Vector3(1f, 1f, 1);
            mat.LightColor = Vector3.One;

            NormalMapMaterial normal = new NormalMapMaterial();
            normal.NormalMap = ResourceManager.Instance.GetResource<Texture2D>("Spaceship_nor_hälfte_fertig_NRM");
            normal.LightDirection = new Vector3(1f, 1f, .5f);
            normal.LightColor = Vector3.One;
            normal.SpecularPower = 32;

            LightingMaterial lighting = new LightingMaterial();
            lighting.LightDirection = new Vector3(.5f, .5f, .5f);
            //lighting.LightColor = Color.Aqua.ToVector3();
            
            lighting.SpecularPower = 1;

            MeshObject mesh = new MeshObject("ship", "p1_wedge", 0.01f);
            mesh.Position = new Vector3(0, 3, 0);
            mesh.Scale = new Vector3(0.01f);
            mesh.LoadContent();
            mesh.SetModelEffect(ResourceManager.Instance.GetResource<Effect>("PPModel"), true);
            mesh.Material = lighting;
            sceneGraph.RootNode.Children.Add(mesh);

            MeshObject plane = new MeshObject("plane", "Ground", 0.01f);
            plane.Position = new Vector3(0, 0, 0);
            plane.Scale = new Vector3(0.01f);
            plane.LoadContent();
            plane.Material = lighting;
            plane.SetModelEffect(ResourceManager.Instance.GetResource<Effect>("PPModel"), true);
            sceneGraph.RootNode.Children.Add(plane);

            TextElement element = new TextElement("text", new Vector2(1, 1), Color.Black, "Test", ResourceManager.Instance.GetResource<SpriteFont>("Arial"));
            element.LoadContent();
            UIManager.Instance.AddActor(element);
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

            UIManager.Instance.Update(gameTime);

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
