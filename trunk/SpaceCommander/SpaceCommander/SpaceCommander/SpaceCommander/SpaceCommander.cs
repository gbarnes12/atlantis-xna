namespace SpaceCommander
{
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
    using GameApplication;
    using GameApplication.Actors.Primitives;
    using GameApplication.Actors.Cameras;


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SpaceCommander : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        public SpaceCommander()
        {
            graphics = new GraphicsDeviceManager(this);
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
            // set our necessary files for the game
            GameApplication.Instance.SetGame(this);
            GameApplication.Instance.SetGraphicsDevice(GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Camera camera = new Camera("camera", new Vector3(0, 3, 3), Vector3.Zero);
            camera.LoadContent(Content);
            WorldManager.Instance.AddActor(camera);

            Axis axis = new Axis("axis", Vector3.Zero, 1f);
            axis.LoadContent(Content);
            WorldManager.Instance.AddActor(axis);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
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

            WorldManager.Instance.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            WorldManager.Instance.Render(gameTime);

            base.Draw(gameTime);
        }
    }
}
