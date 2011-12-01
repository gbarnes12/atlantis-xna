namespace GameApplicationTools.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;

    using Actors;
    using Collections;
    using Actors.Cameras;
    using Actors.Properties;

    using Microsoft.Xna.Framework;


    /// <summary>
    /// This interface is used together with the
    /// actor graph implementation which is basically
    /// just a scene graph. 
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public interface IActorNode
    {
        #region Properties
        ActorNodeCollection Children { get; }

        Vector3 Position { get; set; }

        Vector3 Offset { get; set; }

        Quaternion Rotation { get; set; }

        bool Visible { get; set; }

        bool Updateable { get; set; }

        BoundingSphere BoundingSphere { get; }

        Matrix AbsoluteTransform { get; set; }

        IController Controller { get; set; }

        Vector3 Scale { get; set; }
        #endregion

        #region Methods
        BoundingSphere GetBoundingSphere();
        void PreRender();
        void Update(SceneGraphManager sceneGraph);
        void Render(SceneGraphManager sceneGraph);
        #endregion

    }
}
