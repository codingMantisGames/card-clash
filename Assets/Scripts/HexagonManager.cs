using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonManager : MonoBehaviour
{
    #region VARIABLES
    private HexagonTile[] hexagonTiles;
    public static Transform activeHexagon;
    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {
        hexagonTiles = GetComponentsInChildren<HexagonTile>();
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
            if (item.isLeft == Gamemanager.instance.isLeft)
                item.ToggleHexagon(flag);
        }
    }
    public void SpawnBuilding(GameObject gm)
    {
        HexagonTile tile = activeHexagon.GetComponent<HexagonTile>();
        Transform t = Instantiate(gm, tile.buildPoint).transform;
        t.localScale = Vector3.one * 1.6f;

        if (!Gamemanager.instance.isLeft)
            t.rotation = Quaternion.Euler(0, 180, 0);
    }
    #endregion
}
