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
        private Dictionary<string, Camera> _cameras;
        #endregion

        private CameraManager()
        {
            _cameras = new Dictionary<string, Camera>();
        }

        /// <summary>
        /// Just adds a specific camera id to our internal 
        /// list. 
        /// </summary>
        /// <param name="ID"></param>
        public void AddCamera(String ID, Camera obj)
        {
            _cameras.Add(ID, obj);
        }

        /// <summary>
        /// Returns a specific camera with 
        /// the ID you have passed over.
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Camera GetCamera(String ID)
        {
            if (_cameras.ContainsKey(ID))
            {
                return _cameras[ID];
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
                if (_cameras.ContainsKey(CurrentCamera))
                {
                    return _cameras[CurrentCamera];
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
