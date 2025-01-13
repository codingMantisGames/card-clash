using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class HexagonTile : NetworkBehaviour
{
    #region VARIABLES
    public GameObject itemPlaced;
    public bool isLeft;
    public bool isNoBuildZone;
    public bool isPlayerZone;
    [SerializeField] private MeshRenderer hexRenderer;
    public Transform buildPoint;


    [Header("Properties")]
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color normalColor;
    private bool isBuildMode;

    [Header("Networked Properties")]
    [Networked] public bool isUsed { get; set; }
    #endregion

    #region UNITY FUNCTIONS
    private void Awake()
    {
    }
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
    public void PlaceItem(GameObject item)
    {
        itemPlaced = item;
        isUsed = true;
    }
    #endregion
}
