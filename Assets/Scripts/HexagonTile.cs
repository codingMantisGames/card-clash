using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonTile : MonoBehaviour
{
    #region VARIABLES
    public bool isLeft;
    [SerializeField] private MeshRenderer hexRenderer;
    public Transform buildPoint;


    [Header("Properties")]
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color normalColor;
    private bool isBuildMode;
    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {

    }
    void Update()
    {

    }
    private void OnMouseEnter()
    {
        if (isBuildMode)
        {
            HexagonManager.activeHexagon = transform;
            HighlightHexagon(true);
        }
    }
    private void OnMouseExit()
    {
        if (isBuildMode)
        {
            HexagonManager.activeHexagon = null;
            HighlightHexagon(false);
        }
    }
    #endregion

    #region FUNCTIONS
    public void HighlightHexagon(bool flag = true)
    {
        if (flag)
            hexRenderer.material.color = selectedColor;
        else
            hexRenderer.material.color = normalColor;
    }
    public void ToggleHexagon(bool flag = true)
    {
        isBuildMode = flag;
        hexRenderer.material.color = normalColor;
        hexRenderer.gameObject.SetActive(flag);
    }
    #endregion
}
