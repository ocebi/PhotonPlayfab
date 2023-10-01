using Sirenix.OdinInspector;
using StateSystem;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    [ReadOnly]
    public StateMachine MenuStateMachine;

    [Button]
    private void SetRefs()
    {
        MenuStateMachine = GetComponentInChildren<StateMachine>();
    }

    [Button]
    public void OpenScreen(string screenName)
    {
        MenuStateMachine.SetNewState(screenName);
    }

    public bool IsScreenOpened(string screenName)
    {
        return MenuStateMachine.CurrentStateName == screenName;
    }
}