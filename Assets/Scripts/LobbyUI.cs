using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LobbyUI : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private UIPanels currentPanel;
    [SerializeField] private GameObject logoPanel;
    [SerializeField] private CanvasGroup mainOptionPanel;
    [SerializeField] private CanvasGroup customGamePanel;
    [SerializeField] private CanvasGroup randomRoomPanel;
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
    public void ShowRandomGamePanel()
    {
        currentPanel = UIPanels.RANDOM_GAME_SEARCH;
        randomRoomPanel.gameObject.SetActive(true);
        mainOptionPanel.DOFade(0, uiEaseTime).SetEase(ease).OnComplete(() =>
        {
            randomRoomPanel.DOFade(1, uiEaseTime).SetEase(ease);
            mainOptionPanel.gameObject.SetActive(false);
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
            });
        }
        else if (currentPanel == UIPanels.RANDOM_GAME_SEARCH)
        {
            mainOptionPanel.gameObject.SetActive(true);
            randomRoomPanel.DOFade(0, uiEaseTime2).SetEase(ease).OnComplete(() =>
            {
                mainOptionPanel.DOFade(1, uiEaseTime2).SetEase(ease);
                randomRoomPanel.gameObject.SetActive(false);
            });
        }
    }
    public void HowToPlay()
    {
        Application.OpenURL(howToPlayURL);
    }
    #endregion
}
public enum UIPanels
{
    LOGO, MAIN_OPTIONS, CUSTOM_GAME, RANDOM_GAME_SEARCH, LOBBY
}