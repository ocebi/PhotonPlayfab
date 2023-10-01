using System;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
public class PunchScaleFeedback : MonoBehaviour
{
    public Action OnFeedbackFinished;
    [SerializeField] Vector3 m_PunchVector = new Vector3(0.3f, 0.3f, 0.3f);
    [SerializeField] float m_Duration = 0.4f;
    [SerializeField] int m_Vibrato = 10;
    [SerializeField] float m_Elasticity = 10;
    [SerializeField] private int m_LoopCount = 0;

    private Tweener m_Tweener;

    [Button]
    public void Play()
    {
        if (m_Tweener != null)
        {
            m_Tweener.Kill(complete: true);
        }

        m_Tweener = transform.DOPunchScale(m_PunchVector, m_Duration, m_Vibrato, m_Elasticity).SetLoops(m_LoopCount).OnComplete((() => OnFeedbackFinished.InvokeSafe()));
    }
}