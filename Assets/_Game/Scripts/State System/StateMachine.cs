using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StateSystem
{
    public class StateMachine : MonoBehaviour
    {
        public Action<string> OnStateChanged;
        public bool ActivateCurrentStateGameObject;
        public bool AutoStartStateMachine = true;
        
        public State CurrentState { get; private set; }
        public bool IsInitialized => m_AvailableStatesInitialized;

        [SerializeField, ReadOnly] 
        private StateDictionary m_AvailableStates = new StateDictionary();
        private bool m_AvailableStatesInitialized = false;
        private bool m_IsStateMachinePaused = true;
        
        [ReadOnly]
        public string CurrentStateName;

        private void Awake()
        {
            var statesEnumerator = m_AvailableStates.GetEnumerator();
            while (statesEnumerator.MoveNext())
            {
                statesEnumerator.Current.Value.Initialize(this, gameObject);
                if (ActivateCurrentStateGameObject)
                    statesEnumerator.Current.Value.gameObject.SetActive(false);
            }
            statesEnumerator.Dispose();
            
            m_AvailableStatesInitialized = true;
            if (AutoStartStateMachine)
                m_IsStateMachinePaused = false;
        }

        private void Update()
        {
            if (m_IsStateMachinePaused)
                return;
            if (!m_AvailableStatesInitialized) 
                return;
            if (CurrentState == null)
            {
                SetNewState(GetDefaultState());
            }

            CurrentState.OnStateUpdate();
        }
        
        [Button]
        private void SetStates()
        {
            m_AvailableStates.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                var type = transform.GetChild(i).GetComponent<State>().GetType().ToString();
                var state = transform.GetChild(i).GetComponent<State>();
                if (!m_AvailableStates.ContainsKey(type))
                    m_AvailableStates.Add(type, state);
                if (ActivateCurrentStateGameObject)
                    state.gameObject.SetActive(false);
            }
        }

        public void ActivateStateMachine()
        {
            m_AvailableStatesInitialized = true;
            m_IsStateMachinePaused = false;
        }

        public void ResetStateMachine()
        {
            m_AvailableStatesInitialized = false;
            m_IsStateMachinePaused = true;
            CurrentState = null;
            CurrentStateName = "";
        }
        
        public void SetNewState(string newState)
        {
            if (CurrentState != null)
            {
                CurrentState.OnStateExit();
                if (ActivateCurrentStateGameObject)
                    CurrentState.gameObject.SetActive(false);
            }
            if (m_AvailableStates.ContainsKey(newState))
            {
                CurrentState = m_AvailableStates[newState];
                CurrentState.OnStateEnter();
                if (ActivateCurrentStateGameObject)
                    CurrentState.gameObject.SetActive(true);
                CurrentStateName = newState.ToString();
                OnStateChanged.InvokeSafe(newState);
            }
            else
                Debug.LogError($"State {newState} does not exist in the dictionary");
        }

        public string GetDefaultState()
        {
            return m_AvailableStates.First().Key;
        }

        private void OnDestroy()
        {
            OnStateChanged = null;
        }
    }
    
    
    [Serializable] public class StateDictionary : UnitySerializedDictionary<string, State> { }
}
