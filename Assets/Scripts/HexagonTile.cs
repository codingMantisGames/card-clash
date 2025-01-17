using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class HexagonTile : NetworkBehaviour
{
    #region VARIABLES
    public GameObject itemPlaced;
    public bool isLeft;
    public bool isCardNeutral = false;
    public bool isNoBuildZone;
    public bool isPlayerZone;
    [SerializeField] private MeshRenderer hexRenderer;
    public Transform buildPoint;
    public List<HexagonTile> adjacentTiles;


    [Header("Properties")]
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color normalColor;
    [HideInInspector] public bool isBuildMode;

    [Header("Networked Properties")]
    [Networked] public bool isUsed { get; set; }
    public bool canAttack;

    public float radius;
    public int index;
    private Collider[] playerColliders;
    [SerializeField] private LayerMask playerLayer;
    #endregion

    #region UNITY FUNCTIONS
    private void Awake()
    {
    }
    void Start()
    {
        index = HexagonManager.instance.GetIndex(this);

        canAttack = false;
    }
    void Update()
    {

    }
    private void OnMouseEnter()
    {
        if (isBuildMode)
        {
            if (Gamemanager.instance.currentRoundStage == RoundStage.ATTACK && isUsed && canAttack)
            {
                HexagonManager.activeHexagon = transform;
                //HighlightHexagon(true);
                CursorChanger.instance.SetAttackCursorFocued();
            }
            else if (Gamemanager.instance.currentRoundStage != RoundStage.ATTACK)
            {
                HexagonManager.activeHexagon = transform;
                HighlightHexagon(true);
            }
        }
    }
    private void OnMouseExit()
    {
        if (isBuildMode)
        {
            if (Gamemanager.instance.currentRoundStage == RoundStage.ATTACK && isUsed && canAttack)
            {
                HexagonManager.activeHexagon = null;
                //HighlightHexagon(false);
                CursorChanger.instance.SetAttackCursor();
            }
            else if (Gamemanager.instance.currentRoundStage != RoundStage.ATTACK)
            {
                HexagonManager.activeHexagon = null;
                HighlightHexagon(false);
            }
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

        if (!flag)
            canAttack = false;
    }
    public void PlaceItem(GameObject item)
    {
        itemPlaced = item;
        isUsed = true;
    }
    private void OnMouseDown()
    {
        if (Gamemanager.instance.currentRoundStage != RoundStage.USING_CARDS)
        {
            playerColliders = new Collider[1];
            int k = Physics.OverlapSphereNonAlloc(transform.position, 0.2f, playerColliders, playerLayer);
            if (k != 0)
            {
                if (playerColliders[0].gameObject.TryGetComponent<PlaceableItem>(out PlaceableItem item))
                {
                    item.OnMouseDownFun();
                }

            }
        }

        if (Gamemanager.instance.currentRoundStage == RoundStage.MOVE_ITEM && isBuildMode)
        {
            HexagonManager.instance.HideAllHex();
            Gamemanager.instance.MoveCuurentItem(this, HexagonManager.instance.GetIndex(this));

            Gamemanager.instance.OnItemSelected?.Invoke();
        }
        else if (Gamemanager.instance.currentRoundStage == RoundStage.ATTACK && isBuildMode)
        {
            if (isUsed && canAttack)
            {
                AttackThisTile();
            }
        }

    }
    public PlaceableItem GetPlayer()
    {
        playerColliders = new Collider[1];
        int k = Physics.OverlapSphereNonAlloc(transform.position, 0.2f, playerColliders, playerLayer);
        if (k != 0)
        {
            if (playerColliders[0].gameObject.TryGetComponent<PlaceableItem>(out PlaceableItem item))
            {
                return item;
            }

        }

        return null;
    }
    public void AttackThisTile()
    {
        Gamemanager.instance.currentItemToMove.Attack(this);
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
    [ContextMenu("Show Hex")]
    public void ShowHex()
    {
        ToggleHexagon(true);
    }
    #endregion
}
