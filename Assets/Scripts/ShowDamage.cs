using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ShowDamage : MonoBehaviour
{
    #region VARIABLES
    public static ShowDamage instance;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Transform child;
    [SerializeField] private float easeTime;
    [SerializeField] private float displayTime;
    [SerializeField] private Ease ease;
    Transform mainCam;
    #endregion

    #region UNITY FUNCTIONS
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        mainCam = Camera.main.transform;
    }
    void Update()
    {
        if (child.gameObject.activeInHierarchy)
        {
            child.transform.LookAt(child.transform.position + mainCam.transform.rotation * Vector3.forward, mainCam.transform.rotation * Vector3.up);
        }
    }
    #endregion

    #region FUNCTIONS
    [SimpleButton]
    public void Demo()
    {
        ShowDamageValue(12, transform.position);
    }
    public void ShowDamageValue(int amount, Vector3 pos)
    {
        transform.position = pos;

        Color c = label.color;
        c.a = 1;
        label.color = c;

        label.text = "-" + amount;

        child.transform.localPosition = new Vector3(0, 2.2f, 0);

        child.gameObject.SetActive(true);
        label.DOFade(0, easeTime).SetDelay(displayTime).SetEase(ease);
        child.DOLocalMoveY(4.5f, easeTime).SetEase(ease).OnComplete(() =>
        {
            child.gameObject.SetActive(false);
        });
    }
    #endregion
}
