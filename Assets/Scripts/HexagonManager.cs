using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class HexagonManager : NetworkBehaviour
{
    #region VARIABLES
    public static HexagonManager instance;
    public HexagonTile[] hexagonTiles;
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

       // Vector3 cumulativePosition = Vector3.zero;

        //foreach (HexagonTile child in hexagonTiles)
        //{
           // cumulativePosition += child.transform.position;
        //}

       // transform.position = cumulativePosition / hexagonTiles.Length;
    }
    void Update()
    {

    }
    #endregion

    #region FUNCTIONS
    public void ToggleHexagon(bool flag = true)
    {
        foreach (var item in hexagonTiles)
        {
            if (!item.isUsed && item.isLeft == Gamemanager.instance.isLeft && !item.isNoBuildZone && item.buildPoint.childCount == 0)
                item.ToggleHexagon(flag);
        }
    }
    public void ToggleHexagonForCharacter(bool flag = true)
    {
        foreach (var item in hexagonTiles)
        {
            if (!item.isUsed && item.isLeft == Gamemanager.instance.isLeft && item.isNoBuildZone && item.buildPoint.childCount == 0)
                item.ToggleHexagon(flag);
        }
    }
    public void ToogleHexagonForCards(bool flag = true)
    {
        foreach (var item in hexagonTiles)
        {
            if (!item.isUsed && ((item.isLeft == Gamemanager.instance.isLeft) || (item.isCardNeutral)) && !item.isNoBuildZone)
                item.ToggleHexagon(flag);
        }
    }
    public void ToogleForHealCards(bool flag = true)
    {
        foreach (var item in hexagonTiles)
        {
            if (item.isUsed)
            {
                PlaceableItem player = item.GetPlayer();
                if (player != null && player.isLeft == Gamemanager.instance.isLeft)
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
                PlaceableItem player = item.GetPlayer();
                if (player != null && player.isLeft != Gamemanager.instance.isLeft && !player.isFreezed)
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
        RPC_SpawnItem(id, index, Gamemanager.instance.isLeft);
    }
    public void SetRotationNow()
    {
        RPC_SetRotation();
    }

    [Rpc(RpcSources.All, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SetRotation()
    {
        if (!Gamemanager.instance.isLeft)
        {
            // HexagonManager.instance.transform.rotation = Quaternion.Euler(0, 180, 0);
            Gamemanager.instance.SetTargetCamera(180);
        }
        else
        {

        }
    }
    [Rpc(RpcSources.All, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SpawnItem(string id, int index, bool isLeft)
    {
        if (Runner.IsServer)
        {
            GameObject gm = null;
            foreach (var item in Gamemanager.instance.cardDatas)
            {
                if (item.cardID == id)
                {
                    gm = item.prefab;
                }
            }

            HexagonTile tile = hexagonTiles[index];
            NetworkObject n = Runner.Spawn(gm, tile.buildPoint.position, tile.buildPoint.localRotation);

            Transform t = n.transform;
            t.localScale = Vector3.one * gm.transform.localScale.z;
            tile.isUsed = true;

            if (t.TryGetComponent<PlaceableItem>(out PlaceableItem placeableItem))
            {
                placeableItem.SetBuilding(isLeft);
                placeableItem.tileIndex = index;

                if (!isLeft)
                    placeableItem.SetInitialRotation();
            }

            if (t.TryGetComponent<DropableCard>(out DropableCard dropableCard))
            {
                dropableCard.SetCard(isLeft);
            }

            if (t.transform.tag == "Heal")
            {
                tile.GetPlayer().Heal();
            }

            if (t.transform.tag == "Ice")
                tile.GetPlayer().Freeze();
        }
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
        GetHexagon(index).isUsed = false;
    }

    public void HideAllHex()
    {
        foreach (var item in hexagonTiles)
        {
            item.ToggleHexagon(false);
        }

        CursorChanger.instance.SetNormalCursor();

        isHexMoveOn = false;
    }
    public int GetIndex(HexagonTile tile)
    {
        return System.Array.IndexOf(hexagonTiles, tile);
    }
    public HexagonTile GetHexagon(int index)
    {
        return hexagonTiles[index];
    }
    #endregion
}
