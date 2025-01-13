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
    public List<HexagonTile> adjacentTiles;


    [Header("Properties")]
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color normalColor;
    private bool isBuildMode;

    [Header("Networked Properties")]
    [Networked] public bool isUsed { get; set; }

    public float radius;
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
    private void OnMouseDown()
    {
        if (Gamemanager.instance.currentRoundStage == RoundStage.USING_CARDS || !isBuildMode)
            return;
        HexagonManager.instance.HideAllHex();
        Gamemanager.instance.MoveCuurentItem(buildPoint.position, HexagonManager.instance.GetIndex(this));

        Gamemanager.instance.OnItemSelected?.Invoke();
    }
    /*[ContextMenu("Get it")]
    public void GetAllAdjacent()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, radius);

        adjacentTiles = new List<HexagonTile>();
        foreach (var item in colls)
        {
            if (item.TryGetComponent<HexagonTile>(out HexagonTile hex) && hex != this)
            {
                adjacentTiles.Add(hex);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }*/
    #endregion
}
