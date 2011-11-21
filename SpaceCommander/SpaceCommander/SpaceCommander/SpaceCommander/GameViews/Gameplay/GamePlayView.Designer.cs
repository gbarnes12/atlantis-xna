namespace SpaceCommander.GameViews.Gameplay
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

	public partial class GamePlayView
	{
        public void RegisterEvents()
        {
         
        }

        public void RegisterActors()
        {
            #region 3D Stuff

            Ship ship = new Ship("SpaceShip", ID, Vector3.Zero);
            ship.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(ship);

            Planet planet = new Planet("GamePlanetEarth", ID, new Vector3(-700, 0, -600), 700f);
            planet.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(planet);

            ThirdPersonCamera tpcamera = new ThirdPersonCamera("GamePlayCamera",new Vector3(0,100,300), ship);
            tpcamera.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(tpcamera);
            

            SkySphere sky = new SkySphere("SkySphereSkyGameView", ID, Vector3.Zero, GameApplication.Instance.TexturePath + "space", 10000f);
            sky.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(sky);
            #endregion

            #region UI Stuff
            TextElement headline = new TextElement("TextElementHeadline2", ID, new Vector2(400, 100), Color.Yellow, "GamePlayView", GameApplication.Instance.GetGame().Content.Load<SpriteFont>(GameApplication.Instance.FontPath + "Arial"));
            headline.Scale = 1f;
            WorldManager.Instance.AddActor(headline);


            #endregion
        }
	}
}
