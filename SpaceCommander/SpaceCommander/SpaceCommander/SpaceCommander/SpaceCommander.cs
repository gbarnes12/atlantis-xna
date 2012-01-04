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

    using GameApplicationTools;
    using GameApplicationTools.Actors.Primitives;
    using GameApplicationTools.Actors.Cameras;
    using GameApplicationTools.Events;
    using GameApplicationTools.Interfaces;
    using GameApplicationTools.Input;
    using GameApplicationTools.UI;
    using GameApplicationTools.Structures;

    using Events;
    using Scripts;
    using GameViews.MainMenu;
    using GameViews.Gameplay;
    using GameApplicationTools.Resources.PostProcessors;
    using GameApplicationTools.Misc;

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SpaceCommander : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        KeyboardInputService keyboardInputService;
        Color BackgroundColor = Color.CornflowerBlue;

        public SpaceCommander()
        {
            graphics = new GraphicsDeviceManager(this);
           // graphics.PreferredBackBufferWidth = 1600;
           // graphics.PreferredBackBufferHeight = 900;
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
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();


            // set our necessary classes for the game
            GameApplication.Instance.SetGame(this);
            GameApplication.Instance.SetGraphicsDevice(GraphicsDevice);
            ResourceManager.Instance.Content = Content;

            // the console breaks with our current structure but its still a Singleton
            // thus its not that bad. This needs to have some more workarounds but 
            // currently its ok the way it is!
            keyboardInputService = new KeyboardInputService();
            Services.AddService(typeof(GameConsole.IKeyboardInputService), keyboardInputService);
            Components.Add(new GameConsole(Services, "Content\\" + GameApplication.Instance.FontPath + "ConsoleFont"));
            GameConsole.Instance.StringParser = new StringParser();
            GameConsole.Instance.ToggleKey = Keys.F11;

            //GameConsole.Instance.SelectedObjects.Add(WorldManager.Instance);
            GameConsole.Instance.SelectedObjects.Add(this);

            // set up our game views
            
            GamePlayView gamePlayView = new GamePlayView();
         //   MainMenuGameView mainMenu = new MainMenuGameView();

        //    GameViewManager.Instance.AddGameView(mainMenu);
            GameViewManager.Instance.AddGameView(gamePlayView);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // load the contents we need to the resource manager!
            DefaultScript.LoadContents();

            GameViewManager.Instance.LoadContent();


            PostProcessor processor = new PostProcessor("BlurPostProcessor", "BlurPostProcessor");
            processor.Enabled = false;
            processor.Effect.Parameters["targetSize"].SetValue(new Vector2(GameApplication.Instance.GetGraphics().PresentationParameters.BackBufferWidth,
                GameApplication.Instance.GetGraphics().PresentationParameters.BackBufferHeight));
            PostProcessorManager.Instance.AddProcessor(processor);


            //GameView menueView = GameViewManager.Instance.GetGameView("MainMenu") as GameView;
            //menueView.BlocksInput = true;
            //menueView.BlocksLoading = false;
            //menueView.BlocksRendering = true;
            //menueView.BlocksUpdating = true;

            //GameView gamplayView = GameViewManager.Instance.GetGameView("GamePlay") as GameView;
            //gamplayView.BlocksInput = false;
            //gamplayView.BlocksLoading = false;
            //gamplayView.BlocksRendering = false;
            //gamplayView.BlocksUpdating = false;
        }

        public void ChangeColor(Color color)
        {
            BackgroundColor = color;
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
            // this is necessary for our console settings!
            keyboardInputService.previousKeyboard = keyboardInputService.currentKeyboard;
            keyboardInputService.currentKeyboard = Keyboard.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                GameApplication.Instance.ExitGame();
            else if (KeyboardDevice.Instance.WasKeyPressed(Keys.Escape))
                GameApplication.Instance.ExitGame();

            
            ScriptManager.Instance.Update(gameTime);
            GameViewManager.Instance.Update(gameTime);
            UIManager.Instance.Update(gameTime);

            KeyboardDevice.Instance.Update();
            MouseDevice.Instance.Update();
            EventManager.Instance.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(BackgroundColor);

            PostProcessorManager.Instance.BeginRender();

            GameViewManager.Instance.Render();

            PostProcessorManager.Instance.Render();

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
