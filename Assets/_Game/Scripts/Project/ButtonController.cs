using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class ButtonController : Singleton<ButtonController>
{
    public int ClickCount => m_ClickCount;
    [SerializeField, ReadOnly] 
    private int m_ClickCount;
    [SerializeField, ReadOnly]
    private PunchScaleFeedback m_Feedback;
    [SerializeField, ReadOnly] 
    private TMP_Text m_ClickText;

    [Button]
    private void SetRefs()
    {
        m_Feedback = GetComponent<PunchScaleFeedback>();
        m_ClickText = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        GameManager.OnGameFinished += OnGameFinished;
    }

    private void OnDisable()
    {
        GameManager.OnGameFinished += OnGameFinished;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameStarted)
            return;
        if (!Input.GetMouseButtonDown(0))
            return;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.transform == transform)
            {
                OnButtonClicked();
            }
        }
    }

    #region Private Methods
    
    private void OnGameFinished()
    {
        ResetButton();
    }
    
    private void OnButtonClicked()
    {
        ++m_ClickCount;
        UpdateClickText();
        m_Feedback.Play();
    }

    private void ResetButton()
    {
        m_ClickCount = 0;
        UpdateClickText();
    }

    private void UpdateClickText()
    {
        m_ClickText.SetText(m_ClickCount.ToString());
    }

    #endregion
}