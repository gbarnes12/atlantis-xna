namespace SpaceCommander.GameViews.MainMenu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;

    using GameApplicationTools;
    using GameApplicationTools.Actors.Cameras;
    using GameApplicationTools.Actors.Models;
    using GameApplicationTools.Structures;
    using GameApplicationTools.Interfaces;
    using GameApplicationTools.UI;

    using Actors;
    using Microsoft.Xna.Framework.Graphics;
    

    public partial class MainMenuGameView
    {
        public void LoadActors()
        {
            #region 3D Stuff
            Camera camera = new Camera("camera", ID, new Vector3(0, 0, 1000), Vector3.Zero);
            camera.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(camera);

            Planet planet = new Planet("PlanetEarth", ID, new Vector3(-700, 0, 0), 400f);
            planet.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(planet);

            SkySphere sky = new SkySphere("SkySphereSky", ID, Vector3.Zero, "Textures\\space", 10000f);
            sky.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(sky);
            #endregion

            #region UI Stuff
            TextElement headline = new TextElement("TextElementHeadline", ID, new Vector2(400, 100), Color.Yellow, "Space Commander", GameApplication.Instance.GetGame().Content.Load<SpriteFont>("Fonts\\Arial"));
            headline.Scale = 1f;
            WorldManager.Instance.AddActor(headline);

            Button startNewGameButton = new Button("ButtonStartNewGame", ID, new Vector2(400, 150), GameApplication.Instance.GetGame().Content.Load<Texture2D>("UI\\Buttons\\startnewgame_button"), 312, 83);
            startNewGameButton.IsVisible = true;
            startNewGameButton.IsUpdateable = true;
            startNewGameButton.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(startNewGameButton);

            #endregion
        }
    }
}
