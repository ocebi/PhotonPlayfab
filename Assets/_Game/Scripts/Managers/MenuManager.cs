using Sirenix.OdinInspector;
using StateSystem;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField, ReadOnly]
    private StateMachine m_MenuStateMachine;

    [Button]
    private void SetRefs()
    {
        m_MenuStateMachine = GetComponentInChildren<StateMachine>();
    }

    [Button]
    public void OpenScreen(string screenName)
    {
        m_MenuStateMachine.SetNewState(screenName);
    }
}