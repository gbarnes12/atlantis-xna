/////////////////////////////////////////////////////////////////////////
// FiniteStateMachine.cs
// FiniteStateMachine to be basis for simple AI.
// 02.15.2010
/////////////////////////////////////////////////////////////////////////
// Copyright (c) 2010 Zapdot Interactive, LLC.
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
/////////////////////////////////////////////////////////////////////////
namespace Structures.StateMachine
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    class FiniteStateMachine
    {
        public static Random RNG = new Random();

        /////////////////////////////////////////////////////////////////////////
        // Variables
        Dictionary<string, FiniteState> mStates;
        string mCurrentState;
        double mRemainingDuration;

        /////////////////////////////////////////////////////////////////////////
        // Constructors
        /// <summary>
        /// Creates the FiniteStateMachine with an empty-set of states.
        /// </summary>
        public FiniteStateMachine()
        {
            mStates = new Dictionary<string, FiniteState>();
            mCurrentState = null;
            mRemainingDuration = 0;
        }

        /////////////////////////////////////////////////////////////////////////
        // Property Accessors
        public string CurrentState
        {
            get { return mCurrentState; }
        }

        /////////////////////////////////////////////////////////////////////////
        // XNA Methods
        /// <summary>
        /// Function that should be run every frame. Updates the remaining duration
        /// of the current state, and chooses the next state if the current state
        /// has expired.
        /// </summary>
        /// <param name="gameTime">XNA's overload of a TimeSpan object.</param>
        public void Update(GameTime gameTime)
        {
            if (string.IsNullOrEmpty(mCurrentState))
                return;

            mRemainingDuration -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (mRemainingDuration <= 0)
                NextState();
        }

        /////////////////////////////////////////////////////////////////////////
        // Public Methods
        /// <summary>
        /// Adds a State to the FiniteStateMachine.
        /// </summary>
        /// <param name="pStateName">The name of the state.</param>
        /// <param name="pMinSeconds">The minimum duration (in seconds) of this state.
        /// Must be a positive, non-zero number.</param>
        /// <param name="pMaxSeconds">The maximum duration (in seconds) of this state.
        /// Must be a positive, non-zero number, greater than or equal two pMinSeconds.</param>
        public void AddState(string pStateName, double pMinSeconds, double pMaxSeconds)
        {
            mStates.Add(pStateName, new FiniteState(pStateName, pMinSeconds, pMaxSeconds));
        }

        /// <summary>
        /// Removes a state, and any transitions to that state from the FiniteStateMachine.
        /// </summary>
        /// <param name="pStateName">The state to remove.</param>
        public void RemoveState(string pStateName)
        {
            if (mStates.ContainsKey(pStateName))
            {
                foreach (var pair in mStates)
                    pair.Value.RemoveTransition(pStateName);

                mStates.Remove(pStateName);
            }
        }

        /// <summary>
        /// If there are transitions in this current state, choose one.
        /// </summary>
        public void NextState()
        {
            if (mStates[mCurrentState].HasTransition())
                SetState(mStates[mCurrentState].PickTransition());
        }

        /// <summary>
        /// Forces the FiniteStateMachine to a state. This should be called under
        /// special circumstances, such as the initial state of an object, or
        /// when a specific action occurs to the object that forces it to action.
        /// </summary>
        /// <param name="pNewState">The state to change to.</param>
        public void SetState(string pNewState)
        {
            FiniteState lNewState;
            // Grabs the value and does the proper assignments with only one hit to the dictionary,
            // not three, if we used ContainsKey(...). If TryGetValue fails, set the mCurrentState
            // to empty, which will cause the object to wait indefinitely until another SetState()
            // call.
            if (mStates.TryGetValue(pNewState, out lNewState))
            {
                mCurrentState = pNewState;
                mRemainingDuration = getNewDuration(lNewState.MinDuration, lNewState.MaxDuration);
            }
            else
                mCurrentState = string.Empty;
        }

        /// <summary>
        /// Attempts to add a transition to a state's transition table with a weighted value. If
        /// the transition already exists, it will be updated with a new weight. Both states must
        /// exist in order for this method to execute successfully.
        /// </summary>
        /// <param name="pStartState">The state to add the transition to.</param>
        /// <param name="pEndState">The state pStartState can transition to.</param>
        /// <param name="pWeight">The weighted percentage that this transition will be chosen.</param>
        public void AddTransition(string pStartState, string pEndState, double pWeight)
        {
            if (mStates.ContainsKey(pStartState) && mStates.ContainsKey(pEndState))
                mStates[pStartState].AddTransition(pEndState, pWeight);
            else if (!mStates.ContainsKey(pStartState))
                throw new ArgumentException("State " + pStartState + " does not exist for this object!");
            else if (!mStates.ContainsKey(pEndState))
                throw new ArgumentException("State " + pEndState + " does not exist for this object!");
        }

        /// <summary>
        /// Removes a transition from a state.
        /// </summary>
        /// <param name="pState">The state to remove the transition from.</param>
        /// <param name="pEndState">The transition to remove from the state.</param>
        public void RemoveTransition(string pState, string pEndState)
        {
            if (mStates.ContainsKey(pState))
                mStates[pState].RemoveTransition(pEndState);
        }

        /////////////////////////////////////////////////////////////////////////
        // Private Methods
        /// <summary>
        /// Returns a new duration in milliseconds between a min and max. This method 
        /// should only be called on numbers grabbed from a FiniteState object to
        /// ensure their acceptability.
        /// </summary>
        /// <param name="pMin">The minimum duration, in seconds.</param>
        /// <param name="pMax">The maximum duration, in seconds.</param>
        /// <returns></returns>
        private double getNewDuration(double pMin, double pMax)
        {
            double lDifference = pMax - pMin;

            return 1000 * (pMin + (RNG.NextDouble() * lDifference));
        }
    }

}
