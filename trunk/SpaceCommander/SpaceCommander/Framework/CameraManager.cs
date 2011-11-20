using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameApplicationTools.Actors.Cameras;

namespace GameApplicationTools
{
    public class CameraManager
    {

        #region Public
        /// <summary>
        /// Returns the singleton instance of the CameraManager
        /// </summary>
        public static CameraManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CameraManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// With this property you can determine
        /// which camera is used currently!
        /// </summary>
        public String CurrentCamera { get; set; }
        #endregion

        #region Private
        private static CameraManager instance;
        private List<string> _cameras;
        #endregion

        private CameraManager()
        {
            _cameras = new List<string>();
        }

        /// <summary>
        /// Just adds a specific camera id to our internal 
        /// list. 
        /// </summary>
        /// <param name="ID"></param>
        public void AddCamera(String ID)
        {
            _cameras.Add(ID);
        }

        /// <summary>
        /// Returns a specific camera with 
        /// the ID you have passed over.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Camera GetCamera(String ID)
        {
            if (WorldManager.Instance.GetActors().ContainsKey(ID))
            {
                return WorldManager.Instance.GetActor<Camera>(ID);
            }
            else
                throw new Exception("There is no camera with the id: " + ID);
        }

        /// <summary>
        /// Returns the current camera!
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Camera GetCurrentCamera()
        {
            if (CurrentCamera != null || CurrentCamera == string.Empty)
            {
                if (WorldManager.Instance.GetActors().ContainsKey(CurrentCamera))
                {
                    return WorldManager.Instance.GetActor(CurrentCamera) as Camera;
                }
                else
                    throw new Exception("There is no camera with the id: " + CurrentCamera);
            }
            else
            {
                throw new Exception("There is no camera with the id: " + CurrentCamera);
            }
        }
    }
}
