using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xen;
using Xen.Camera;
using Xen.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Atlantis
{
    public class Game : Application
    {
        private DrawTargetScreen drawToScreen;
        private Actor actor;

#if DEBUG
        private Console console;


        //this is a special object that displays a large number of debug graphs
        //this is very useful for debugging performance problems at runtime
        private Xen.Ex.Graphics2D.Statistics.DrawStatisticsDisplay statisticsOverlay;
#endif

        protected override void Initialise()
        {
            //all draw targets need a default camera.
            //create a 3D camera
            var camera = new Camera3D();
            camera.LookAt(Vector3.Zero, new Vector3(4, 6, -2), new Vector3(0, 0, -1));

            console = new Console(400, 200);

            //create the draw target.
            this.drawToScreen = new DrawTargetScreen(camera);

            //Set the screen clear colour to blue
            //(Draw targets have a built in ClearBuffer object)
            this.drawToScreen.ClearBuffer.ClearColour = Color.CornflowerBlue;


            //create new actor ("tiny")
            actor = new Actor(this.Content, "tiny_4anim", Vector3.Zero, 0.01f);

            //at runtime, pressing 'F12' will toggle the overlay (or holding both thumbsticks on x360)
            this.statisticsOverlay = new Xen.Ex.Graphics2D.Statistics.DrawStatisticsDisplay(this.UpdateManager);

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

            base.LoadContent(state);
        }

        protected override void Update(UpdateState state)
        {
            //update logic

#if DEBUG
            console.clear();
            console.add("position: " + actor.Position.ToString());
#endif

        }

        protected override void Frame(FrameState state)
        {
            //perform the draw to the screen.
            drawToScreen.Draw(state);

            //at this point the screen has been drawn...
        }
    }
}
