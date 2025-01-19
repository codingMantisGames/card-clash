using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIPanelAnimation : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private RectTransform rect;
    public CanvasGroup canvasGroup;
    [SerializeField] private Image img;
    [Header("Properties")]
    [SerializeField] private Ease openEase;
    [SerializeField] private float openTime;

    [SerializeField, Space()] private Ease fadeEase;
    [SerializeField] private float fadeTime;

    [SerializeField] Vector2 size;
    #endregion

    #region UNITY FUNCTIONS
    private void Start()
    {
        HideMenu();
    }
    #endregion

    #region FUNCTIONS
    [SimpleButton]
    public void Open()
    {
        Vector2 newSize = Vector2.one * 50;
        newSize.x = size.x;
        img.DOFade(0.9f, openTime / 2).SetEase(openEase);
        rect.DOSizeDelta(newSize, openTime).SetEase(openEase).OnComplete(() =>
        {
            rect.DOSizeDelta(size, openTime).SetEase(openEase).OnComplete(() =>
            {
                canvasGroup.DOFade(1, fadeTime).SetEase(fadeEase);
            });
        });
    }
    [SimpleButton]
    public void Close()
    {
        canvasGroup.DOFade(0, fadeTime).SetEase(fadeEase).OnComplete(() =>
        {
            Vector2 newSize = size;
            newSize.y = 50;
            rect.DOSizeDelta(newSize, openTime).SetEase(openEase).OnComplete(() =>
            {
                img.DOFade(0, openTime / 2).SetDelay(openTime / 2).SetEase(openEase);
                rect.DOSizeDelta(Vector2.one * 50, openTime).SetEase(openEase);
            });
        });
    }
    [SimpleButton]
    public void ShowMenu()
    {
        rect.sizeDelta = size;
        canvasGroup.alpha = 1;

        Color c = img.color;
        c.a = 0.9f;
        img.color = c;
    }
    [SimpleButton]
    public void HideMenu()
    {
        rect.sizeDelta = Vector2.one * 50;
        canvasGroup.alpha = 0;
        Color c = img.color;
        c.a = 0;
        img.color = c;
    }
    #endregion
}
