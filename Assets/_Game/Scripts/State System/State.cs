using UnityEngine;

namespace StateSystem
{
    public abstract class State : MonoBehaviour
    {
        protected StateMachine StateMachine;
        protected GameObject Owner;
        protected float StateStartTime;

        public virtual void Initialize(StateMachine stateMachine, GameObject owner)
        {
            StateMachine = stateMachine;
            Owner = owner;
        }

        public virtual void OnStateEnter()
        {
            StateStartTime = Time.time;
        }

        public virtual void OnStateUpdate()
        {

        }

        public virtual void OnStateExit()
        {

        }
    }
}
