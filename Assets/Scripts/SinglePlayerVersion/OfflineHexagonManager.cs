using UnityEngine;

public class OfflineHexagonManager : MonoBehaviour
{
    #region VARIABLES
    public static OfflineHexagonManager instance;
    public OfflineHexagon[] hexagonTiles;
    public static Transform activeHexagon;
    [HideInInspector] public bool isHexMoveOn = false;
    #endregion

    #region UNITY FUNCTIONS
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        LobbyNetworkManager.instance.SwitchMode += HandleOnModeSwitch;
    }
    private void OnDestroy()
    {
        LobbyNetworkManager.instance.SwitchMode -= HandleOnModeSwitch;
    }

    void Update()
    {

    }
    #endregion

    #region FUNCTIONS
    private void HandleOnModeSwitch(bool isBot)
    {
        if (!isBot) this.enabled = false;
        else this.enabled = true;
    }
    public void ToggleHexagon(bool flag = true)
    {
        foreach (var item in hexagonTiles)
        {
            if (!item.isUsed && item.isBot == BotGameManager.instance.isBotsTurn && !item.isNoBuildZone && item.buildPoint.childCount == 0)
                item.ToggleHexagon(flag);
        }
    }
    public void ToggleHexagonForCharacter(bool flag = true)
    {
        foreach (var item in hexagonTiles)
        {
            if (!item.isUsed && item.isBot == BotGameManager.instance.isBotsTurn && item.isNoBuildZone && item.buildPoint.childCount == 0)
                item.ToggleHexagon(flag);
        }
    }
    public void ToogleHexagonForCards(bool flag = true)
    {
        foreach (var item in hexagonTiles)
        {
            if (!item.isUsed && ((item.isBot == BotGameManager.instance.isBotsTurn) || (item.isCardNeutral)) && !item.isNoBuildZone)
                item.ToggleHexagon(flag);
        }
    }
    public void ToogleForHealCards(bool flag = true)
    {
        foreach (var item in hexagonTiles)
        {
            if (item.isUsed)
            {
                OfflinePlacableItem player = item.GetPlayer();
                if (player != null && player.isBot == BotGameManager.instance.isBotsTurn)
                {
                    item.ToggleHexagon(flag);
                }
            }
        }
    }
    public void ToogleForIceCards(bool flag = true)
    {
        foreach (var item in hexagonTiles)
        {
            if (item.isUsed)
            {
                OfflinePlacableItem player = item.GetPlayer();
                if (player != null && player.isBot != BotGameManager.instance.isBotsTurn && !player.isFreezed)
                {
                    item.ToggleHexagon(flag);
                }
            }
        }
    }
    public void SpawnBuilding(string id)
    {
        int index = System.Array.IndexOf(hexagonTiles, activeHexagon.GetComponent<HexagonTile>());
        HexagonManager.activeHexagon = null;
        //RPC_SpawnItem(id, index, Gamemanager.instance.isLeft);
    }
    public void CallOnMouseDown(int index)
    {
        if (hexagonTiles[index].isBuildMode)
            hexagonTiles[index].AttackThisTile();
    }
    public void SelectHexagon(HexagonTile tile)
    {
        if (!tile.isUsed)
            tile.ToggleHexagon(true);
    }
    public void SelectHexagonAll(HexagonTile tile)
    {
        tile.ToggleHexagon(true);
    }
    public void FreeHexSpace(int index)
    {
        try
        {
            GetHexagon(index).isUsed = false;
        }
        catch
        {
            Debug.Log("Some issue here bro!");
        }
    }
    public void HideAllHex()
    {
        foreach (var item in hexagonTiles)
        {
            item.ToggleHexagon(false);
        }
        activeHexagon = null;//this is the new change
        CursorChanger.instance.SetNormalCursor();

        isHexMoveOn = false;
    }
    public int GetIndex(HexagonTile tile)
    {
        return System.Array.IndexOf(hexagonTiles, tile);
    }
    public OfflineHexagon GetHexagon(int index)
    {
        return hexagonTiles[index];
    }
    #endregion
}