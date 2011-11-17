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

    using Actors;
    using GameApplicationTools.Structures;
    using GameApplicationTools.Interfaces;

    public partial class MainMenuGameView
    {
        public void LoadActors()
        {
            FPSCamera camera = new FPSCamera("camera", "MainMenu", new Vector3(0, 0, 1000), Vector3.Zero);
            camera.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(camera);

            Planet planet = new Planet("earth", "MainMenu", Vector3.Zero, 400f);
            planet.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(planet);

            SkySphere sky = new SkySphere("skysphere", "MainMenu", Vector3.Zero, "Textures\\space", 10000f);
            sky.LoadContent(GameApplication.Instance.GetGame().Content);
            WorldManager.Instance.AddActor(sky);
        }
    }
}
