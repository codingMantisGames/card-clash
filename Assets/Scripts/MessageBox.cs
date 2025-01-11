using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    #region VARIABLES
    public static MessageBox instance;
    [SerializeField] private UIPanelAnimation panelAnimation;
    [SerializeField] private Action buttonCallback1;
    [SerializeField] private Action buttonCallback2;
    [SerializeField] private Button buttonOne;
    [SerializeField] private Button buttonTwo;

    private TMP_Text bOne;
    private TMP_Text bTwo;

    [SerializeField] private TMP_Text m_heading;
    [SerializeField] private TMP_Text m_content;

    #endregion

    #region UNITY FUNCTIONS
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        bOne = buttonOne.GetComponentInChildren<TMP_Text>();
        bTwo = buttonTwo.GetComponentInChildren<TMP_Text>();
    }
    void Update()
    {

    }
    #endregion

    #region FUNCTIONS
    public void ShowMessage(string heading, string content, string button1 = "", string button2 = "", Action callback1 = null, Action callback2 = null)
    {
        panelAnimation.Open();

        if (callback2 != null)
            buttonCallback2 = callback2;
        if (callback2 != null)
            buttonCallback1 = callback1;

        if (string.IsNullOrEmpty(button1))
            buttonOne.gameObject.SetActive(false);
        else
            buttonOne.gameObject.SetActive(true);

        if (string.IsNullOrEmpty(button2))
            buttonTwo.gameObject.SetActive(false);
        else
            buttonTwo.gameObject.SetActive(true);

        bOne.text = button1;
        bTwo.text = button2;

        m_heading.text = heading;
        m_content.text = content;
    }
    public void ButtonOneClick()
    {
        buttonCallback1?.Invoke();

        buttonCallback1 = null;

        panelAnimation.Close();
    }
    public void ButtonTwoClick()
    {
        buttonCallback2?.Invoke();

        buttonCallback2 = null;

        panelAnimation.Close();
    }
    #endregion
}
