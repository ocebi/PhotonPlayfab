using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public Action OnTimerFinished;

    public bool IsTimerActive
    {
        get => m_IsTimerActive;
        protected set => m_IsTimerActive = value;
    }
    public string RemainingTime
    {
        get
        {
            if (!m_IsTimerActive)
                return "0";
            var timeSpan = TimeSpan.FromSeconds(Time.time - m_StartTime);
            return ((int)(Mathf.Max(m_TimerDuration - timeSpan.Seconds, 0))).ToString();
            // return string.Format("{0:D1}", Mathf.Max(m_TimerDuration - timeSpan.Seconds, 0));
            // return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }
    
    [SerializeField, ReadOnly]
    protected float ElapsedTime;
    [SerializeField, ReadOnly]
    private bool m_IsTimerActive;
    private float m_TimerDuration;
    private float m_StartTime;
    
    [Button]
    public virtual void StartTimer(float duration)
    {
        m_TimerDuration = duration;
        m_StartTime = Time.time;
        ElapsedTime = 0;
        m_IsTimerActive = true;
    }

    protected virtual void Update()
    {
        if (!m_IsTimerActive)
            return;
        ElapsedTime += Time.deltaTime;
        if (ElapsedTime >= m_TimerDuration)
        {
            OnTimerFinished.InvokeSafe();
            m_IsTimerActive = false;
        }
    }
}