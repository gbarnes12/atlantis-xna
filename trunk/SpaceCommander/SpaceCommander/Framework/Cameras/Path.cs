using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameApplicationTools.Actors;
using GameApplicationTools.Resources;
using Microsoft.Xna.Framework.Graphics;
using GameApplicationTools.Actors.Cameras;

namespace GameApplicationTools.Cameras
{
    public class Path : Actor
    {
        #region Private

        DefaultEffect effect;
        
        VertexBuffer vertexBuffer;
        
        VertexPositionColor[] vertices;

        Color color = Color.Gold;

        #endregion

        #region Public 

        public Curve curveX = new Curve();
        public Curve curveY = new Curve();
        public Curve curveZ = new Curve();

        #endregion


        public Path(String ID,String gameViewID)
            :base(ID,gameViewID)
        {
            curveX.PostLoop = CurveLoopType.Cycle;
            curveY.PostLoop = CurveLoopType.Cycle;
            curveZ.PostLoop = CurveLoopType.Cycle;

            curveX.PreLoop = CurveLoopType.Cycle;
            curveY.PreLoop = CurveLoopType.Cycle;
            curveZ.PreLoop = CurveLoopType.Cycle;
        }

        public Path(String ID, String gameViewID,Color color)
            : base(ID, gameViewID)
        {
            this.color = color;

            curveX.PostLoop = CurveLoopType.Cycle;
            curveY.PostLoop = CurveLoopType.Cycle;
            curveZ.PostLoop = CurveLoopType.Cycle;

            curveX.PreLoop = CurveLoopType.Cycle;
            curveY.PreLoop = CurveLoopType.Cycle;
            curveZ.PreLoop = CurveLoopType.Cycle;
        }

        public override void LoadContent()
        {
            // load some kind of basicEffect
            effect = new DefaultEffect(ResourceManager.Instance.GetResource<Effect>("DefaultEffect").Clone());

            //set visible
            this.Visible = true;

            base.LoadContent();
        }
       
        public void SetTangents()
        {
           
            //set tangents
            CurveKey prev;
            CurveKey current;
            CurveKey next;
            int prevIndex;
            int nextIndex;
            for (int i = 0; i < curveX.Keys.Count; i++)
            {
                prevIndex = i - 1;
                if (prevIndex < 0) prevIndex = i;

                nextIndex = i + 1;
                if (nextIndex == curveX.Keys.Count) nextIndex = i;

                prev = curveX.Keys[prevIndex];
                next = curveX.Keys[nextIndex];
                current = curveX.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                curveX.Keys[i] = current;
                prev = curveY.Keys[prevIndex];
                next = curveY.Keys[nextIndex];
                current = curveY.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                curveY.Keys[i] = current;

                prev = curveZ.Keys[prevIndex];
                next = curveZ.Keys[nextIndex];
                current = curveZ.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                curveZ.Keys[i] = current;
            }

            //init vertices
            initVertices();

        }

        private void initVertices()
        {
            // set up our vertices (8 vertices per second)
            vertices = new VertexPositionColor[(int)curveX.Keys[curveX.Keys.Count-1].Position];

            for (int i = 0; i < (int)curveX.Keys[curveX.Keys.Count - 1].Position; i++)
            {
                vertices[i] = new VertexPositionColor(new Vector3(curveX.Evaluate(i),curveY.Evaluate(i),curveZ.Evaluate(i)),color);
            }

            //create vertexbuffer
            vertexBuffer = new VertexBuffer(GameApplication.Instance.GetGraphics(), typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

        }

        //rebuilds the path. usefull for the editor while creating a path
        public void refresh()
        {
            SetTangents();
        }

        static void SetCurveKeyTangent(ref CurveKey prev, ref CurveKey cur, ref CurveKey next)
        {
            float dt = next.Position - prev.Position;
            float dv = next.Value - prev.Value;
            if (Math.Abs(dv) < float.Epsilon)
            {
                cur.TangentIn = 0;
                cur.TangentOut = 0;
            }
            else
            {
                // The in and out tangents should be equal to the 
                // slope between the adjacent keys.
                cur.TangentIn = dv * (cur.Position - prev.Position) / dt;
                cur.TangentOut = dv * (next.Position - cur.Position) / dt;
            }
        }

        //add a new point to the path. TODO: should be modified so that all points are children (necessary for translation in the editor!)
        public void AddPoint(Vector3 point, float time)
        {
            curveX.Keys.Add(new CurveKey(time, point.X));
            curveY.Keys.Add(new CurveKey(time, point.Y));
            curveZ.Keys.Add(new CurveKey(time, point.Z));
        }
     
        //get the current point at a specified moment
        public Vector3 GetPointOnCurve(float time)
        {
            Vector3 point = new Vector3();
            point.X = curveX.Evaluate(time);
            point.Y = curveY.Evaluate(time);
            point.Z = curveZ.Evaluate(time);
            return point;
        }

        /// <summary>
        /// fail !!!!!!!!!!!
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="startposition"></param>
        /// <returns></returns>
        
        /*
        public static Path getPositionPathFromTargetPath(Path targetPath,Vector3 startposition)
        {
            Path positionPath = new Path();

            //set new startposition
            positionPath.curveX.Keys.Add(new CurveKey( startposition.X,0));
            positionPath.curveY.Keys.Add(new CurveKey( startposition.Y,0));
            positionPath.curveZ.Keys.Add(new CurveKey( startposition.Z,0));

            //recalculate keys
            foreach (CurveKey key in targetPath.curveX.Keys)
            {
                positionPath.curveX.Keys.Add(new CurveKey(key.Position,key.Value+intervall));
            }
            foreach (CurveKey key in targetPath.curveY.Keys)
            {
                positionPath.curveY.Keys.Add(new CurveKey(key.Position, key.Value + intervall));
            }
            foreach (CurveKey key in targetPath.curveZ.Keys)
            {
                positionPath.curveZ.Keys.Add(new CurveKey(key.Position, key.Value + intervall));
            }

            positionPath.SetTangents();

            return positionPath;
        }
         * */

        //rendering settings
        public override void PreRender()
        {
            base.PreRender();
        }

        //render path
        public override void Render(SceneGraphManager sceneGraph)
        {
            Camera camera = CameraManager.Instance.GetCurrentCamera();

            effect.World = AbsoluteTransform;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.CurrentTechnique.Passes[0].Apply();


            GameApplication.Instance.GetGraphics().SetVertexBuffer(vertexBuffer);
            GameApplication.Instance.GetGraphics().DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip,vertices, 0,vertices.Length-1);

            base.Render(sceneGraph);
        }
    }
}
