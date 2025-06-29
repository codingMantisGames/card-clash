using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OfflineHexagon : MonoBehaviour
{
    #region VARIABLES
    public GameObject itemPlaced;
    public bool isBot;
    public bool isCardNeutral = false;
    public bool isNoBuildZone;
    public bool isBotZone;
    [SerializeField] private MeshRenderer hexRenderer;
    public Transform buildPoint;
    public List<OfflineHexagon> adjacentTiles;

    public bool isUsed;
    public bool isUsedByEnemy;
    public bool canAttack;
    public bool isMarkedByAI;

    public float radius;
    public int index;
    private Collider[] playerColliders;
    [SerializeField] private LayerMask playerLayer;


    [Header("Properties")]
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color normalColor;
    [HideInInspector] public bool isBuildMode;

    [Header("Score")]
    [SerializeField, Range(-1, 1)] public float attack;
    [SerializeField, Range(-1, 1)] public float defent;
    [SerializeField, Range(-1, 1)] public float initialMoves;
    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {
        index = OfflineHexagonManager.instance.GetIndex(this);

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
        if (IsMouseOverUI() || !BotGameManager.instance.isBotGamePlay)
            return;

        if (isBuildMode)
        {
            if (BotGameManager.instance.currentRoundStage == RoundStage.ATTACK && isUsed && canAttack)
            {
                OfflineHexagonManager.activeHexagon = transform;
                //HighlightHexagon(true);
                CursorChanger.instance.SetAttackCursorFocued();
            }
            else if (BotGameManager.instance.currentRoundStage != RoundStage.ATTACK)
            {
                OfflineHexagonManager.activeHexagon = transform;
                HighlightHexagon(true);

                if (BotGameManager.instance.currentRoundStage == RoundStage.USING_CARDS)
                    CursorChanger.instance.SetDropCursor();
            }
        }
    }
    private void OnMouseExit()
    {
        if (IsMouseOverUI() || !BotGameManager.instance.isBotGamePlay)
            return;

        if (isBuildMode)
        {
            if (BotGameManager.instance.currentRoundStage == RoundStage.ATTACK && isUsed && canAttack)
            {
                OfflineHexagonManager.activeHexagon = null;
                //HighlightHexagon(false);
                CursorChanger.instance.SetAttackCursor();
            }
            else if (BotGameManager.instance.currentRoundStage != RoundStage.ATTACK)
            {
                OfflineHexagonManager.activeHexagon = null;
                HighlightHexagon(false);

                if (BotGameManager.instance.currentRoundStage == RoundStage.USING_CARDS)
                    CursorChanger.instance.SetNormalCursor();
            }
        }
    }
    private void OnMouseDown()
    {
        if (IsMouseOverUI() || !BotGameManager.instance.isBotGamePlay)
            return;


        if (BotGameManager.instance.isBotsTurn || !BotGameManager.instance.canInteract)
            return;

        if (BotGameManager.instance.currentRoundStage != RoundStage.USING_CARDS)
        {
            playerColliders = new Collider[1];
            int k = Physics.OverlapSphereNonAlloc(transform.position, 0.2f, playerColliders, playerLayer);
            if (k != 0)
            {
                if (playerColliders[0].gameObject.TryGetComponent<OfflinePlacableItem>(out OfflinePlacableItem item))
                {
                    item.OnMouseDownFun();
                }

            }
        }

        if (BotGameManager.instance.currentRoundStage == RoundStage.MOVE_ITEM && isBuildMode)
        {
            OfflineHexagonManager.instance.HideAllHex();
            BotGameManager.instance.MoveCuurentItem(this, OfflineHexagonManager.instance.GetIndex(this));

            BotGameManager.instance.OnItemSelected?.Invoke();
        }
        else if (BotGameManager.instance.currentRoundStage == RoundStage.ATTACK && isBuildMode)
        {
            if (isUsed && canAttack)
            {
                AttackThisTile();
            }
        }

    }
    #endregion

    #region FUNCTIONS
    [ContextMenu("Get it")]
    public void GetAllAdjacent()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, radius);

        adjacentTiles = new List<OfflineHexagon>();
        foreach (var item in colls)
        {
            if (item.TryGetComponent<OfflineHexagon>(out OfflineHexagon hex) && hex != this)
            {
                adjacentTiles.Add(hex);
            }
        }
    }
    public OfflineHexagon[] GetAllAdjacnetTile()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, radius);

        List<OfflineHexagon> adjacentTiles = new List<OfflineHexagon>();
        foreach (var item in colls)
        {
            if (item.TryGetComponent<OfflineHexagon>(out OfflineHexagon hex) && hex != this)
            {
                adjacentTiles.Add(hex);
            }
        }

        return adjacentTiles.ToArray();
    }
    public void ShowHex()
    {
        ToggleHexagon(true);
    }
    public void AttackThisTile()
    {
        BotGameManager.instance.currentItemToMove.Attack(this);
    }
    public void HighlightHexagon(bool flag = true)
    {
        if (flag)
            hexRenderer.material.color = selectedColor;
        else
            hexRenderer.material.color = normalColor;
    }
    public void PlaceItem(GameObject item)
    {
        itemPlaced = item;
        isUsed = true;
        isUsedByEnemy = !isBot;
    }
    public void ToggleHexagon(bool flag = true)
    {
        isBuildMode = flag;
        hexRenderer.material.color = normalColor;
        hexRenderer.gameObject.SetActive(flag);

        if (!flag)
            canAttack = false;
    }
    private void HandleOnModeSwitch(bool isBot)
    {
        if (!isBot) this.enabled = false;
        else this.enabled = true;
    }
    public OfflinePlayerTower GetTower()
    {
        playerColliders = new Collider[1];
        int k = Physics.OverlapSphereNonAlloc(transform.position, 0.2f, playerColliders, playerLayer);
        if (k != 0)
        {
            if (playerColliders[0].gameObject.TryGetComponent<OfflinePlayerTower>(out OfflinePlayerTower tower))
            {
                return tower;
            }
        }

        return null;
    }

    public OfflinePlacableItem GetPlayer()
    {
        playerColliders = new Collider[1];
        int k = Physics.OverlapSphereNonAlloc(transform.position, 0.2f, playerColliders, playerLayer);
        if (k != 0)
        {
            if (playerColliders[0].gameObject.TryGetComponent<OfflinePlacableItem>(out OfflinePlacableItem item))
            {
                return item;
            }

        }

        return null;
    }

    public OfflineDropableCards GetCard()
    {
        playerColliders = new Collider[1];
        int k = Physics.OverlapSphereNonAlloc(transform.position, 0.2f, playerColliders, playerLayer);
        if (k != 0)
        {
            if (playerColliders[0].gameObject.TryGetComponent<OfflineDropableCards>(out OfflineDropableCards card))
            {
                return card;
            }

        }

        return null;
    }

    [ContextMenu("get data")]
    public void GetData()
    {
        HexagonTile tile = GetComponent<HexagonTile>();
        isCardNeutral = tile.isCardNeutral;
        isBot = !tile.isLeft;
        isNoBuildZone = tile.isNoBuildZone;
        isBotZone = !tile.isPlayerZone;
    }
    public bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    #endregion
}