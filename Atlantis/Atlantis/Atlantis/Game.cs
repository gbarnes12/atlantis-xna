﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xen;
using Xen.Camera;
using Xen.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Xen.Ex.Graphics;
using Xen.Ex.Graphics.Display;
using Xen.Ex.Graphics.Content;
using JigLibX.Physics;
using JigLibX.Collision;

namespace Atlantis
{
    public class Game : Application
    {
        private PhysicsSystem physics;
        private DrawTargetScreen drawToScreen;
        private Actor actor;
        private Skydome skydome;


#if DEBUG
        private Console console;


        //this is a special object that displays a large number of debug graphs
        //this is very useful for debugging performance problems at runtime
        private Xen.Ex.Graphics2D.Statistics.DrawStatisticsDisplay statisticsOverlay;
#endif

        protected override void Initialise()
        {
            //initialize the physic stuff!
            physics = new PhysicsSystem();
            physics.CollisionSystem = new CollisionSystemSAP();

            //all draw targets need a default camera.
            //create a 3D camera
            var camera = new Xen.Camera.FirstPersonControlledCamera3D(this.UpdateManager, Vector3.Zero, false);


            //don't allow the camera to move too fast
            camera.MovementSensitivity *= 0.1f;
            camera.LookAt(new Vector3(0.0f, 100.0f, 0.0f), new Vector3(640.0f, 300.0f, 640.0f), Vector3.Up);

            console = new Console(400, 200);

            //create the draw target.
            this.drawToScreen = new DrawTargetScreen(camera);

            //Set the screen clear colour to blue
            //(Draw targets have a built in ClearBuffer object)
            this.drawToScreen.ClearBuffer.ClearColour = Color.CornflowerBlue;



            //create new actor ("tiny")
            actor = new Actor(this.Content, "tiny_4anim", Vector3.Zero, 1f);

            skydome = new Skydome(Content, new Vector3(500,100,700), 1000f);

            //at runtime, pressing 'F12' will toggle the overlay (or holding both thumbsticks on x360)
            this.statisticsOverlay = new Xen.Ex.Graphics2D.Statistics.DrawStatisticsDisplay(this.UpdateManager);


            TerrainDrawer terrain = new TerrainDrawer(this.Content, Vector3.Zero);
      
            drawToScreen.Add(terrain);

            drawToScreen.Add(skydome);

            //add statistics to screen
            drawToScreen.Add(statisticsOverlay);

            //add console to screen
            drawToScreen.Add(console.getTextElementRect());

            //add actor to the screen
            drawToScreen.Add(actor);

        }

        protected override void LoadContent(ContentState state)
        {
            //Load a normal XNA sprite font
            var xnaSpriteFont = state.Load<SpriteFont>("Arial");

            //both elements require the font to be set before they are drawn
            console.setFont(xnaSpriteFont);

            this.statisticsOverlay.Font = xnaSpriteFont;


            //NEW CODE
            //Mask rendering for the different particle drawers.
            //This allows the snow and fog particles to be drawn using different drawing methods


            base.LoadContent(state);
        }

        protected override void Update(UpdateState state)
        {
            //update logic
            float timeStep = (float)state.DeltaTimeTicks / TimeSpan.TicksPerSecond;
            PhysicsSystem.CurrentPhysicsSystem.Integrate(timeStep);

#if DEBUG
            console.clear();
            //console.add("position: " + terrain.Position.ToString());
#endif

        }

        protected override void Frame(FrameState state)
        {
            //perform the draw to the screen.
            drawToScreen.Draw(state);
        }
    }
}