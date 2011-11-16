﻿namespace GameApplication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using System.Reflection;

    /// <summary>
    /// This class contains some useful methods
    /// we can use to ease our lives.
    /// You can find various types of methods in here
    /// there can be methods for math but as well as 
    /// methods that just check for a certain interface.
    /// 
    /// Author: Gavin Barnes
    /// Version: 1.0
    /// </summary>
    public class Utils
    {

        /// <summary>
        /// Checks if the given object inherits from a 
        /// given Interface. 
        /// 
        /// Example usage: Utils.IsInterfaceImplemented T (object);
        /// </summary>
        /// <typeparam name="I">The interface you want to check on!</typeparam>
        /// <param name="obj">The object you want to check on</param>
        /// <returns>Just returns whether the object inherits the interface or not</returns>
        public static bool IsInterfaceImplemented<I>(object obj) where I : class
        {
            return obj as I != null;
        }

        /// <summary>
        /// Checks the WorldManager if the actor with 
        /// the given ID exists.
        /// </summary>
        /// <param name="ID">The id of the Actor</param>
        /// <returns>True or False</returns>
        public static bool CheckIfActorExists(String ID)
        {
            if (WorldManager.Instance.GetActor(ID) != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a Projection matrix you won't need
        /// this function more often than once since we
        /// save it within our camera class. 
        /// 
        /// The nearPlane is set to: .01f and the far plane
        /// to: 5000. 
        /// </summary>
        /// <returns>Matrix</returns>
        public static Matrix CreateProjectionMatrix()
        {
            return Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                            (float)GameApplication.Instance.GetGraphics().Viewport.Width /
                            (float)GameApplication.Instance.GetGraphics().Viewport.Height,
                            .01f, 5000
                        );
        }

        /// <summary>
        /// Creates a world matrix with the Translation
        /// you want to have. All the other values are 
        /// set to either Matrix.Identity for the Rotation
        /// and 1f for the scale. 
        /// </summary>
        /// <param name="Translation">A 3d vector which decides in which direction to move</param>
        /// <returns>Matrix</returns>
        public static Matrix CreateWorldMatrix(Vector3 Translation)
        {
            return CreateWorldMatrix(Translation, Matrix.Identity);
        }

        /// <summary>
        /// Creates a world matrix with the Translation
        /// and the Rotation matrix you want to have. 
        /// The Scale is Set to 1f. 
        /// </summary>
        /// <param name="Translation">A 3d vector which decides in which direction to move</param>
        /// <param name="Rotation">The rotation matrix</param>
        /// <returns>Matrix</returns>
        public static Matrix CreateWorldMatrix(Vector3 Translation, Matrix Rotation)
        {
            return CreateWorldMatrix(Translation, Rotation, Vector3.One);
        }

        /// <summary>
        /// Creates a world matrix with the Translation
        /// and the Rotation matrix you want to have and finally
        /// the scale of the object.
        /// </summary>
        /// <param name="Translation">A 3d vector which decides in which direction to move</param>
        /// <param name="Rotation">The rotation matrix</param>
        /// <param name="Scale">A 3d vector representing the scale of this object.</param>
        /// <returns>Matrix</returns>
        public static Matrix CreateWorldMatrix(Vector3 Translation, Matrix Rotation, Vector3 Scale)
        {
            return Matrix.CreateScale(Scale) * Rotation * Matrix.CreateTranslation(Translation);
        }

        /// <summary>
        /// Basically converts a rotation vector into a
        /// rotation matrix. 
        /// </summary>
        /// <param name="Rotation">The 3D vector for the rotation</param>
        /// <returns></returns>
        public static Matrix Vector3ToMatrix(Vector3 Rotation)
        {
            return Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
        }

        /// <summary>
        /// Converts the rotation matrix into a rotation vector.
        /// </summary>
        /// <param name="Rotation">The matrix holding the rotations</param>
        /// <returns></returns>
        public static Vector3 MatrixToVector3(Matrix Rotation)
        {
            Quaternion q = Quaternion.CreateFromRotationMatrix(Rotation);
            return new Vector3(q.X, q.Y, q.Z);
        }

        /// <summary>
        /// Toggles an boolean value to true or false
        /// depending on its current state.
        /// </summary>
        /// <param name="currentValue">The boolean you want to transform</param>
        /// <returns>a boolean value</returns>
        public static bool ToggleBool(bool currentValue)
        {
            if (currentValue)
                currentValue = false;
            else
                currentValue = true;

            return currentValue;
        }

        /// <summary>
        /// This is just needed for the keyboard devices.
        /// So you don't have to matter about it!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetEnumValues<T>()
        {
            Type currentEnum = typeof(T);
            List<T> resultSet = new List<T>();

            if (currentEnum.IsEnum)
            {
                FieldInfo[] fields = currentEnum.GetFields(BindingFlags.Static | BindingFlags.Public);
                foreach (FieldInfo field in fields)
                    resultSet.Add((T)field.GetValue(null));
            }
            else
                throw new ArgumentException("The argument must of type Enum or of a type derived from Enum", "T");

            return resultSet;
        }

    }
}
