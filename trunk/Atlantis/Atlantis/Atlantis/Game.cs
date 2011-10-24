using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xen;
using Xen.Camera;
using Xen.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlantis
{
    public class Game : Application
    {
        private DrawTargetScreen drawToScreen;


        protected override void Initialise()
        {
            //all draw targets need a default camera.
            //create a 3D camera
            var camera = new Camera3D();

            //create the draw target.
            this.drawToScreen = new DrawTargetScreen(camera);

            //Set the screen clear colour to blue
            //(Draw targets have a built in ClearBuffer object)
            this.drawToScreen.ClearBuffer.ClearColour = Color.CornflowerBlue;

        }

        protected override void Update(UpdateState state)
        {
           //update logic
        }

        protected override void Frame(FrameState state)
        {
            //perform the draw to the screen.
            drawToScreen.Draw(state);

            //at this point the screen has been drawn...
        }
    }
}
