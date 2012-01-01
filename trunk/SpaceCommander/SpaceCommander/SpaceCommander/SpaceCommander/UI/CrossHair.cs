using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameApplicationTools.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameApplicationTools;
using SpaceCommander.Actors;
using GameApplicationTools.Input;

namespace SpaceCommander.UI
{
    public class CrossHair : TextureElement
    {

        private String actorID;

        private float bulletRange = 1800;

        private String filename;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="position"></param>
        /// <param name="texture"></param>
        /// <param name="actorID">which actor's target shall be drawn</param>
        public CrossHair(String ID,  String actorID,String filename,float distance)
            : base(ID, Vector2.Zero, null)
        {
            this.actorID = actorID;
            this.Color = Color.White;
            this.bulletRange = distance;
            this.filename = filename;
        }

        public override void LoadContent()
        {
            this.texture = ResourceManager.Instance.GetResource<Texture2D>(filename);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            //Vector3 fireDirection = Vector3.Transform(Vector3.UnitZ, WorldManager.Instance.GetActor("SpaceShip").Rotation);
          //  fireDirection.Normalize();

            Vector3 fireDirection = CameraManager.Instance.GetCurrentCamera().GetMouseRay(MouseDevice.Instance.Position).Direction;
            fireDirection.Normalize();


          /*  Vector3 target = WorldManager.Instance.GetActor(actorID).Position - fireDirection * bulletRange;

            Vector3 screenPosition = GameApplication.Instance.GetGraphics().Viewport.Project(target,
                CameraManager.Instance.GetCurrentCamera().Projection,
                CameraManager.Instance.GetCurrentCamera().View,
                Matrix.Identity);

            this.Position = new Vector2(screenPosition.X, screenPosition.Y);
            */

            this.Position = MouseDevice.Instance.Position;

            //center crosshair
            this.Position = new Vector2(Position.X - texture.Width / 2, Position.Y - texture.Height / 2);


            base.Update(gameTime);
        }

        public override void Render(GameTime gameTime)
        {
            base.Render(gameTime);
        }
    }
}
