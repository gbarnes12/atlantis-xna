using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameApplicationTools.Actors.Cameras;
using Microsoft.Xna.Framework;

namespace GameApplicationTools.Cameras
{
    public class PathCamera : Camera
    {
        double time = 0;
        Path cameraCurvePosition;
        Path cameraCurveLookat;

        public PathCamera(String ID,Path targetPath,Vector3 cameraStartPosition)
            : base(ID, Vector3.Zero, Vector3.Zero)
        {
            targetPath.SetTangents();
            
            this.cameraCurveLookat = targetPath;

          

            time = 0;
        }

        public PathCamera(String ID, Path targetPath, Path positionPath)
            : base(ID, Vector3.Zero, Vector3.Zero)
        {
            targetPath.SetTangents();
            positionPath.SetTangents();

            this.cameraCurveLookat = targetPath;

            this.cameraCurvePosition = positionPath;

            time = 0;
        }


        public override void Update(GameTime gameTime)
        {
            time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            this.Position = cameraCurvePosition.GetPointOnCurve((float)time);
            this.Target = cameraCurveLookat.GetPointOnCurve((float)time);
         //   this.Up = Vector3.Up;

            base.Update(gameTime);
        }
    }
}
