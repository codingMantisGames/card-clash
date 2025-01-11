using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIPanelAnimation : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private RectTransform rect;
    public CanvasGroup canvasGroup;
    [Header("Properties")]
    [SerializeField] private Ease openEase;
    [SerializeField] private float openTime;

    [SerializeField, Space()] private Ease fadeEase;
    [SerializeField] private float fadeTime;

    [SerializeField] Vector2 size;
    #endregion

    #region UNITY FUNCTIONS
    #endregion

    #region FUNCTIONS
    [SimpleButton]
    public void Open()
    {
        rect.DOSizeDelta(size, openTime).SetEase(openEase).OnComplete(() =>
        {
            canvasGroup.DOFade(1, fadeTime).SetEase(fadeEase);
        });
    }
    [SimpleButton]
    public void Close()
    {
        canvasGroup.DOFade(0, fadeTime).SetEase(fadeEase).OnComplete(() =>
        {
            rect.DOSizeDelta(Vector2.zero, openTime).SetEase(openEase);
        });
    }
    #endregion
}
