using UnityEngine;

namespace StateSystem
{
    public abstract class State : MonoBehaviour
    {
        protected StateMachine StateMachine;
        protected GameObject Owner;
        protected float StateStartTime;

        public virtual void Initialize(StateMachine i_StateMachine, GameObject i_Owner)
        {
            StateMachine = i_StateMachine;
            Owner = i_Owner;
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
