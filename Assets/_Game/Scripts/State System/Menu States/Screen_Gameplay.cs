using Sirenix.OdinInspector;
using StateSystem;
using TMPro;
using UnityEngine;

public class Screen_Gameplay : State
{
    [SerializeField, ReadOnly]
    private TMP_Text m_InitialTimerText;
    [SerializeField, ReadOnly]
    private TMP_Text m_MatchTimerText;
    
    [SerializeField, ReadOnly]
    private Timer m_InitialTimer;
    [SerializeField, ReadOnly]
    private Timer m_MatchTimer;

    [Button]
    private void SetRefs()
    {
        m_InitialTimerText = transform.FindDeepChild<TMP_Text>("InitialTimerText");
        m_MatchTimerText = transform.FindDeepChild<TMP_Text>("MatchTimerText");
        
        m_InitialTimer = transform.FindDeepChild<Timer>("InitialTimer");
        m_MatchTimer = transform.FindDeepChild<Timer>("MatchTimer");
    }

    private void OnEnable()
    {
        m_InitialTimer.OnTimerFinished += OnInitialTimerFinished;
        m_MatchTimer.OnTimerFinished += OnMatchTimerFinished;
        m_InitialTimerText.gameObject.SetActive(true);
        m_InitialTimer.StartTimer(GameConfig.Instance.InitialCountdown);
        m_InitialTimerText.SetText(GameConfig.Instance.InitialCountdown.ToString());
        m_MatchTimerText.SetText(GameConfig.Instance.MatchDuration.ToString());
    }

    private void OnDisable()
    {
        m_InitialTimer.OnTimerFinished -= OnInitialTimerFinished;
        m_MatchTimer.OnTimerFinished -= OnMatchTimerFinished;
    }

    private void Update()
    {
        if (m_InitialTimer.IsTimerActive)
            m_InitialTimerText.SetText("Starting game in " + m_InitialTimer.RemainingTime);
        if (m_MatchTimer.IsTimerActive)
            m_MatchTimerText.SetText(m_MatchTimer.RemainingTime);
    }

    private void OnInitialTimerFinished()
    {
        m_MatchTimer.StartTimer(GameConfig.Instance.MatchDuration);
        m_InitialTimerText.gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }

    private void OnMatchTimerFinished()
    {
        GameManager.Instance.FinishGame();
    }
}