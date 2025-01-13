using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class HexagonManager : NetworkBehaviour
{
    #region VARIABLES
    public static HexagonManager instance;
    private HexagonTile[] hexagonTiles;
    public static Transform activeHexagon;
    #endregion

    #region UNITY FUNCTIONS
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        hexagonTiles = GetComponentsInChildren<HexagonTile>();

        Vector3 cumulativePosition = Vector3.zero;

        foreach (HexagonTile child in hexagonTiles)
        {
            cumulativePosition += child.transform.position;
        }

        transform.position = cumulativePosition / hexagonTiles.Length;
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
    public void SpawnBuilding(string id)
    {
        int index = System.Array.IndexOf(hexagonTiles, activeHexagon.GetComponent<HexagonTile>());
        HexagonManager.activeHexagon = null;
        RPC_SpawnItem(id, index);
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
    public void RPC_SpawnItem(string id, int index)
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
            //Transform t = Instantiate(gm, tile.buildPoint).transform;
            Transform t = Runner.Spawn(gm, tile.buildPoint.position, tile.buildPoint.rotation).transform;
            t.localScale = Vector3.one * 1.6f;
            tile.isUsed = true;

            /* if (!Gamemanager.instance.isLeft)
                 t.rotation = Quaternion.Euler(0, 180, 0);*/
        }
    }
    #endregion
}
