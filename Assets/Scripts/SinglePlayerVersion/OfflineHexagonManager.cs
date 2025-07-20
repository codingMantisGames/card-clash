using Fusion;
using UnityEngine;
using static Unity.Collections.Unicode;

public class OfflineHexagonManager : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private CodingMantisGames.SimpleAI.AIBrain brain;

    public static OfflineHexagonManager instance;
    public OfflineHexagon[] hexagonTiles;
    public static Transform activeHexagon;
    [HideInInspector] public bool isHexMoveOn = false;
    #endregion

    #region UNITY FUNCTIONS
    private void Awake()
    {
        instance = this;

        int c = 1;
        foreach (var hexagonTile in hexagonTiles)
        {
            hexagonTile.transform.name += "-->" + c++;
        }

      /*  foreach (var hexagonTile in hexagonTiles)
        {
           if(hexagonTile.isCardNeutral && !hexagonTile.isBot) hexagonTile.gameObject.SetActive(false);

           if(hexagonTile.isBot && !hexagonTile.isNoBuildZone) hexagonTile.gameObject.SetActive(false);
        }*/
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
        int index = System.Array.IndexOf(hexagonTiles, activeHexagon.GetComponent<OfflineHexagon>());
        OfflineHexagonManager.activeHexagon = null;
        SpawnItem(id, index, BotGameManager.instance.isBotsTurn);
    }
    public void CallOnMouseDown(int index)
    {
        if (hexagonTiles[index].isBuildMode)
            hexagonTiles[index].AttackThisTile();
    }
    public void SelectHexagon(OfflineHexagon tile)
    {
        if (!tile.isUsed)
            tile.ToggleHexagon(true);
    }
    public void SelectHexagonAll(OfflineHexagon tile)
    {
        tile.ToggleHexagon(true);
    }
    public void FreeHexSpace(int index)
    {
        try
        {
            GetHexagon(index).isUsed = false;
            GetHexagon(index).isUsedByEnemy = false;
        }
        catch
        {
            Debug.Log("Some issue here bro!");
        }
    }
    public void SpawnItem(string id, OfflineHexagon offlineHexagon)
    {
        int index = System.Array.IndexOf(hexagonTiles, offlineHexagon);
        OfflineHexagonManager.activeHexagon = null;
        SpawnItem(id, index, BotGameManager.instance.isBotsTurn);
    }
    public void SpawnItem(string id, int index, bool isBot)
    {
        GameObject gm = null;
        foreach (var item in BotGameManager.instance.cardDatas)
        {
            if (item.cardID == id)
            {
                gm = item.prefab;
            }
        }
        //Debug.LogError("Index " + index);

        OfflineHexagon tile = hexagonTiles[index];
        //NetworkObject n = Runner.Spawn(gm, tile.buildPoint.position, tile.buildPoint.localRotation);
        GameObject n = Instantiate(gm, tile.buildPoint.position, tile.buildPoint.localRotation);

        Transform t = n.transform;
        t.localScale = Vector3.one * gm.transform.localScale.z;
        tile.isUsed = true;
        tile.isUsedByEnemy = !isBot;

        if (t.TryGetComponent<OfflinePlacableItem>(out OfflinePlacableItem placeableItem))
        {
            placeableItem.SetBuilding(isBot);
            placeableItem.tileIndex = index;

            if (isBot)
                placeableItem.SetInitialRotation();

            if (BotGameManager.instance.isBotsTurn) brain.allyCharacters.Add(placeableItem);
            else brain.enemyCharacters.Add(placeableItem);
        }

        if (t.TryGetComponent<OfflineDropableCards>(out OfflineDropableCards dropableCard))
        {
            dropableCard.SetCard(isBot);
            dropableCard.tileIndex = index;
        }

        if (t.transform.tag == "Heal")
        {
            tile.GetPlayer().Heal();
        }

        if (t.transform.tag == "Ice")
            tile.GetPlayer().Freeze();
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
    public int GetIndex(OfflineHexagon tile)
    {
        return System.Array.IndexOf(hexagonTiles, tile);
    }
    public OfflineHexagon GetHexagon(int index)
    {
        return hexagonTiles[index];
    }
    #endregion
}