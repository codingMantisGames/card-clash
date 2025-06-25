using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.EventSystems;

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
        LobbyNetworkManager.instance.SwitchMode += HandleOnModeSwitch;
    }
    private void OnDestroy()
    {
        LobbyNetworkManager.instance.SwitchMode -= HandleOnModeSwitch;
    }
    void Update()
    {

    }

    private void OnMouseEnter()
    {
        if (IsMouseOverUI() || Gamemanager.instance.isLeft)
            return;

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

                if (Gamemanager.instance.currentRoundStage == RoundStage.USING_CARDS)
                    CursorChanger.instance.SetDropCursor();
            }
        }
    }
    private void OnMouseExit()
    {
        if (IsMouseOverUI() || Gamemanager.instance.isLeft)
            return;

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

                if (Gamemanager.instance.currentRoundStage == RoundStage.USING_CARDS)
                    CursorChanger.instance.SetNormalCursor();
            }
        }
    }
    #endregion

    #region FUNCTIONS
    private void HandleOnModeSwitch(bool isBot)
    {
        if (isBot) this.enabled = false;
        else this.enabled = true;
    }
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
        if (IsMouseOverUI() || Gamemanager.instance.isLeft)
            return;

        if (!Gamemanager.instance.isPlayerTurn || !Gamemanager.instance.canInteract)
            return;

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
    public PlayerTower GetTower()
    {
        playerColliders = new Collider[1];
        int k = Physics.OverlapSphereNonAlloc(transform.position, 0.2f, playerColliders, playerLayer);
        if (k != 0)
        {
            if (playerColliders[0].gameObject.TryGetComponent<PlayerTower>(out PlayerTower tower))
            {
                return tower;
            }

        }

        return null;
    }
    public DropableCard GetCard()
    {
        playerColliders = new Collider[1];
        int k = Physics.OverlapSphereNonAlloc(transform.position, 0.2f, playerColliders, playerLayer);
        if (k != 0)
        {
            if (playerColliders[0].gameObject.TryGetComponent<DropableCard>(out DropableCard card))
            {
                return card;
            }

        }

        return null;
    }
    public void AttackThisTile()
    {
        Gamemanager.instance.currentItemToMove.Attack(this);
    }
    [ContextMenu("Get it")]
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
    [ContextMenu("Show Hex")]
    public void ShowHex()
    {
        ToggleHexagon(true);
    }
    private void OnDrawGizmosSelected()
    {
        if (isNoBuildZone)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(buildPoint.position, 0.3f);
        }
        if (isCardNeutral)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(buildPoint.position, 0.5f);
        }

        if (isLeft)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(buildPoint.position, 0.1f);
        }
    }
    public bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    #endregion
}
