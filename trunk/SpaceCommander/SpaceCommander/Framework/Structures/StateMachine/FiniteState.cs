/////////////////////////////////////////////////////////////////////////
// FiniteState.cs
// FiniteState class to be used with FiniteStateMachine
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
namespace GameApplicationTools.Structures.StateMachine
{
    using System;
    using System.Collections.Generic;

    public class FiniteState
    {
        /////////////////////////////////////////////////////////////////////////
        // Variables
        string mName;
        double mMinDuration;
        double mMaxDuration;
        Dictionary<string, double> mTransitions;

        /////////////////////////////////////////////////////////////////////////
        // Constructors
        /// <summary>
        /// Creates an instance of the FiniteState object.
        /// </summary>
        /// <param name="pName">The name of the state.</param>
        /// <param name="pMinDuration">The minimum duration, in seconds, of the state.</param>
        /// <param name="pMaxDuration">The maximum duration, in seconds, of the state.</param>
        public FiniteState(string pName, double pMinDuration, double pMaxDuration)
        {
            if (!string.IsNullOrEmpty(pName))
                mName = pName;
            else
                throw new ArgumentNullException("NULL or empty string when attempting to create FSM state"); // Can you throw in constructor?

            mMinDuration = Math.Max(0, pMinDuration);

            if (pMaxDuration >= mMinDuration)
                mMaxDuration = Math.Min(pMaxDuration, double.MaxValue);
            else
                throw new ArgumentException("pMaxDuration < pMinDuration when creating FSM state.");

            mTransitions = new Dictionary<string, double>();
        }
        /////////////////////////////////////////////////////////////////////////
        // Property Accessors
        public double MinDuration
        {
            get { return mMinDuration; }
        }

        public double MaxDuration
        {
            get { return mMaxDuration; }
        }

        /////////////////////////////////////////////////////////////////////////
        // Public Methods
        /// <summary>
        /// Adds a transition to the transition table, or updates the transition 
        /// weight if it already exists.
        /// </summary>
        /// <param name="pStateName">The state to transition to.</param>
        /// <param name="pWeight">The percentage chance of this being chosen.</param>
        public void AddTransition(string pStateName, double pWeight)
        {
            if (pWeight <= 0)
                throw new ArgumentOutOfRangeException("Transition weights must be a positive, non-zero number");

            if (mTransitions.ContainsKey(pStateName))
                mTransitions[pStateName] = pWeight;
            else
                mTransitions.Add(pStateName, pWeight);

            // Balance if we've just made the weight > 1.0
            balanceTransitionWeight();
        }

        /// <summary>
        /// Removes the transition weight from the transition table.
        /// </summary>
        /// <param name="pStateName">The state to remove from the table.</param>
        /// <returns>True if the removal was successful; false otherwise.</returns>
        public bool RemoveTransition(string pStateName)
        {
            return mTransitions.Remove(pStateName);
        }

        /// <summary>
        /// Checks to see if there are any transitions for the given state.
        /// </summary>
        /// <returns>True if there are available transitions; false otherwise.</returns>
        public bool HasTransition()
        {
            return (mTransitions.Count > 0);
        }

        /// <summary>
        /// Checks to see if a specific transition exists.
        /// </summary>
        /// <param name="pStateName">The state to check in the transition table.</param>
        /// <returns>True if the transition already exists in the state; false otherwise.</returns>
        public bool HasTransition(string pStateName)
        {
            return mTransitions.ContainsKey(pStateName);
        }

        /// <summary>
        /// Picks the next transition from the table, taking the weights into account.
        /// </summary>
        /// <returns>The name of the next transition.</returns>
        public string PickTransition()
        {
            double lRandom = FiniteStateMachine.RNG.NextDouble();
            double lTotalWeight = 0;

            foreach (var pair in mTransitions)
                if ((lTotalWeight += pair.Value) >= lRandom)
                    return pair.Key;

            return string.Empty;
        }

        /////////////////////////////////////////////////////////////////////////
        // Private Methods
        /// <summary>
        /// Sums up the weight of the 6ransitions. 
        /// </summary>
        /// <returns>A double representing the sum of all the transition weights.</returns>
        private double getTotalTransitionWeight()
        {
            double lTotalWeight = 0.0;

            foreach (var pair in mTransitions)
                lTotalWeight += pair.Value;

            return lTotalWeight;
        }

        /// <summary>
        /// This method ensures that the total transition weight equals 1.0, and 
        /// will balance the weight of the individual transitions if the total
        /// ever exceeds 1.0. The balance will happen relative to the total, 
        /// attempting to keep the relative balance that existed between the 
        /// previous states intact.
        /// </summary>
        private void balanceTransitionWeight()
        {
            double lTotalWeight = getTotalTransitionWeight();

            if (lTotalWeight == 1.0)
                return;
            else if (lTotalWeight > 1.0)
            {
                //TODO: message that we are rebalancing?
                foreach (var pair in mTransitions)
                    mTransitions[pair.Key] = pair.Value / lTotalWeight;
            }
        }
    }
}
