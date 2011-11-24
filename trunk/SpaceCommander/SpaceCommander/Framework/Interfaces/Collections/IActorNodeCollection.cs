namespace GameApplicationTools.Interfaces.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using GameApplicationTools.Actors;

    public class ActorNodeCollection : ICollection<Actor>
    {
        #region Private Fields

        List<Actor> sceneNodes = new List<Actor>();
        Actor parent;
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the entry to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public Actor this[int index]
        {
            get
            {
                if (index < 0 || index > sceneNodes.Count - 1)
                    throw new ArgumentOutOfRangeException("index");

                return sceneNodes[index];
            }

            set
            {
                if (index < 0 || index > sceneNodes.Count - 1)
                    throw new ArgumentOutOfRangeException("index");

                sceneNodes[index] = value;
            }
        }

        #endregion

        #region Default Constructor

        /// <summary>
        /// Intializes an instance of the <see cref="ActorNodeCollection"/> class.
        /// </summary>
        public ActorNodeCollection(Actor parent)
        {
            this.parent = parent;
        }

        #endregion

        #region ICollection<SceneNode> Members

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The item to add to the collection.</param>
        public void Add(Actor item)
        {
            item.Parent = this.parent;
            WorldManager.Instance.AddActor(item);
            sceneNodes.Add(item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            sceneNodes.Clear();
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>true if item is found in the collection; otherwise, false.</returns>
        public bool Contains(Actor item)
        {
            return sceneNodes.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the collection to an <see cref="System.Array"/>, starting at a particular index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the elements copied from the collection. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Actor[] array, int arrayIndex)
        {
            sceneNodes.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get { return sceneNodes.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the collection.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        /// <returns>true if item was successfully removed from the collection; otherwise, false. This method also returns false if item is not found in the collection.</returns>
        public bool Remove(Actor item)
        {
            return sceneNodes.Remove(item);
        }

        #endregion

        #region IEnumerable<SceneNode> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="System.Collections.Generic.IEnumerator{SceneNode}"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<Actor> GetEnumerator()
        {
            return sceneNodes.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="System.Collections.IEnumerator"/> that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return sceneNodes.GetEnumerator();
        }

        #endregion
    }
}
