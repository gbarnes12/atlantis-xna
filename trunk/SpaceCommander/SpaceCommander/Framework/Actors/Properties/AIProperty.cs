namespace GameApplicationTools.Actors.Properties
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Structures.StateMachine;


    public class AIProperty : ActorProperty
    {
        public FiniteStateMachine StateMachine { get; set; }
        public bool IsActive { get; set; }

        public AIProperty()
        {
            IsActive = true;
            StateMachine = new FiniteStateMachine();

            // basic behaviors!
            StateMachine.AddState("idle", 2, 3);
            StateMachine.AddState("move", 2, 2);
            StateMachine.AddTransition("idle", "move", .5);
            StateMachine.AddTransition("move", "idle", .5);

       	    // Pick our first state.
            StateMachine.SetState("idle");
        }
    }
}
