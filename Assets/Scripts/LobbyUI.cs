using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    #region VARIABLES
    public static LobbyUI instance;
    [SerializeField] private UIPanels currentPanel;
    [SerializeField] private GameObject logoPanel;
    [SerializeField] private CanvasGroup mainOptionPanel;
    [SerializeField] private CanvasGroup customGamePanel;
    [SerializeField] private CanvasGroup messagePanel;
    [SerializeField] private TMP_Text message;
    [SerializeField] private GameObject messagePanelBackButton;
    [SerializeField] private UIPanelAnimation howToPlayPanel;
    [SerializeField, Space(20)] private RectTransform mainLogo;
    [SerializeField] private GameObject blurPanel;

    [Header("Properties")]
    [SerializeField] private float uiEaseTime;
    [SerializeField] private float uiEaseTime2;
    [SerializeField] private Ease ease;

    #endregion

    #region UNITY FUNCTIONS
    private void Awake()
    {
        instance = this;
    }
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

                mainLogo.DOScale(0.7f, uiEaseTime).SetEase(ease);
                mainLogo.DOAnchorPos3DY(165, uiEaseTime).SetEase(ease).OnComplete(() =>
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
    public void GameStart()
    {
        mainLogo.GetComponent<Image>().DOFade(0, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            mainLogo.gameObject.SetActive(false);
        });
        blurPanel.GetComponent<Image>().DOFade(0, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            blurPanel.SetActive(false);
        });
        
        messagePanel.DOFade(0, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
        {
            messagePanel.gameObject.SetActive(false);
        });
    }
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
        else if (currentPanel == UIPanels.MESSAGE_PANEL)
        {
            customGamePanel.gameObject.SetActive(true);
            messagePanel.DOFade(0, uiEaseTime2).SetEase(ease).OnComplete(() =>
            {
                customGamePanel.DOFade(1, uiEaseTime2).SetEase(ease);
                messagePanel.gameObject.SetActive(false);

                currentPanel = UIPanels.CUSTOM_GAME;
            });
        }
        else if (currentPanel == UIPanels.HOW_TO_PLAY)
        {
            mainOptionPanel.DOFade(1, uiEaseTime2).SetEase(ease).OnComplete(() =>
            {
                mainOptionPanel.gameObject.SetActive(true);
                howToPlayPanel.gameObject.SetActive(false);
                currentPanel = UIPanels.MAIN_OPTIONS;
            });
            howToPlayPanel.Close();
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
        else if (currentPanel == UIPanels.MAIN_OPTIONS)
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
        currentPanel = UIPanels.HOW_TO_PLAY;
        mainOptionPanel.DOFade(0, uiEaseTime2).SetEase(ease).OnComplete(() =>
        {
            mainOptionPanel.gameObject.SetActive(false);
            howToPlayPanel.gameObject.SetActive(true);
            howToPlayPanel.Open();
        });
    }
    #endregion
}
public enum UIPanels
{
    LOGO, MAIN_OPTIONS, CUSTOM_GAME, RANDOM_GAME_SEARCH, LOBBY, MESSAGE_PANEL, HOW_TO_PLAY
}