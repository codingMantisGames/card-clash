using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private UIPanels currentPanel;
    [SerializeField] private GameObject logoPanel;
    [SerializeField] private CanvasGroup mainOptionPanel;
    [SerializeField] private CanvasGroup customGamePanel;
    [SerializeField] private CanvasGroup messagePanel;
    [SerializeField] private TMP_Text message;
    [SerializeField] private GameObject messagePanelBackButton;
    [Header("Properties")]
    [SerializeField] private float uiEaseTime;
    [SerializeField] private float uiEaseTime2;
    [SerializeField] private Ease ease;

    [SerializeField, Space(20)] private string howToPlayURL;

    [SerializeField, Space(20)] private RectTransform mainLogo;
    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {
    }
    void Update()
    {
        if (currentPanel == UIPanels.LOGO)
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentPanel = UIPanels.MAIN_OPTIONS;
                logoPanel.SetActive(false);

                mainLogo.DOScale(0.5f, uiEaseTime).SetEase(ease);
                mainLogo.DOAnchorPos3DY(200, uiEaseTime).SetEase(ease).OnComplete(() =>
                {
                    mainOptionPanel.gameObject.SetActive(true);
                    mainOptionPanel.DOFade(1, uiEaseTime).SetEase(ease);
                });
            }
        }
        else if (currentPanel == UIPanels.MAIN_OPTIONS)
        {

        }
    }
    #endregion

    #region FUNCTIONS
    public void ShowCustomGame()
    {
        currentPanel = UIPanels.CUSTOM_GAME;
        customGamePanel.gameObject.SetActive(true);
        mainOptionPanel.DOFade(0, uiEaseTime2).SetEase(ease).OnComplete(() =>
        {
            customGamePanel.DOFade(1, uiEaseTime2).SetEase(ease);
            mainOptionPanel.gameObject.SetActive(false);
        });
    }
    public void Back()
    {
        if (currentPanel == UIPanels.CUSTOM_GAME)
        {
            mainOptionPanel.gameObject.SetActive(true);
            customGamePanel.DOFade(0, uiEaseTime2).SetEase(ease).OnComplete(() =>
            {
                customGamePanel.gameObject.SetActive(false);
                mainOptionPanel.DOFade(1, uiEaseTime2).SetEase(ease);

                currentPanel = UIPanels.MAIN_OPTIONS;
            });
        }
        else if(currentPanel == UIPanels.MESSAGE_PANEL)
        {
            customGamePanel.gameObject.SetActive(true);
            messagePanel.DOFade(0, uiEaseTime2).SetEase(ease).OnComplete(() =>
            {
                customGamePanel.DOFade(1, uiEaseTime2).SetEase(ease);
                messagePanel.gameObject.SetActive(false);

                currentPanel = UIPanels.CUSTOM_GAME;
            });
        }
    }
    public void ShowMessagePanel(string m)
    {
        message.text = m;

        if (currentPanel == UIPanels.CUSTOM_GAME)
        {
            messagePanel.gameObject.SetActive(true);
            customGamePanel.DOFade(0, uiEaseTime2).SetEase(ease).OnComplete(() =>
            {
                customGamePanel.gameObject.SetActive(false);
                messagePanel.DOFade(1, uiEaseTime2).SetEase(ease);

                currentPanel = UIPanels.MESSAGE_PANEL;
            });
        }
        else if(currentPanel == UIPanels.MAIN_OPTIONS)
        {
            messagePanel.gameObject.SetActive(true);
            mainOptionPanel.DOFade(0, uiEaseTime2).SetEase(ease).OnComplete(() =>
            {
                mainOptionPanel.gameObject.SetActive(false);
                messagePanel.DOFade(1, uiEaseTime2).SetEase(ease);

                currentPanel = UIPanels.MESSAGE_PANEL;
            });
        }
    }
    public void ShowBackButton()
    {
        messagePanelBackButton.SetActive(true);
    }
    public void HowToPlay()
    {
        Application.OpenURL(howToPlayURL);
    }
    #endregion
}
public enum UIPanels
{
    LOGO, MAIN_OPTIONS, CUSTOM_GAME, RANDOM_GAME_SEARCH, LOBBY,MESSAGE_PANEL
}