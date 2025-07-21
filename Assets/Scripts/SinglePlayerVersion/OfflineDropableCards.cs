using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using CodingMantisGames.SimpleAI;

public class OfflineDropableCards : MonoBehaviour
{

    #region VARIABLES
    [SerializeField] private List<MeshRenderer> meshRenderers;
    [SerializeField] private Material red;
    [SerializeField] private Material blue;

    [SerializeField, Space(20)] private Transform[] adjacentHexagons;
    [SerializeField] private Transform textHolder;
    public bool isBot;
    public DropCardType dropCardType;
    public int tileIndex;
    #endregion

    #region UNITY FUNCTIONS
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        if (!BotGameManager.instance.isBotsTurn && textHolder)
        {
            textHolder.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        BotGameManager.instance.CheckPlayerPosition?.Invoke();
    }
    void Update()
    {

    }
    #endregion

    #region FUNCTIONS
    public void SetCard(bool bot = false)
    {
        isBot = bot;
        RPC_SetMaterial(bot);
    }
    public void RPC_SetMaterial(bool isBot)
    {
        foreach (var item in meshRenderers)
        {
            if (isBot)
                item.material = blue;
            else
            {
                item.material = red;
            }
        }
    }

    public void AddItem(Transform item)
    {
        for (int i = 0; i < 6; i++)
        {
            if (Vector3.Distance(item.transform.position, adjacentHexagons[i].position) < 0.5f)
            {
                RPC_EnableHex(true, i);
            }
        }
    }
    public void HideItem(Transform item)
    {
        for (int i = 0; i < 6; i++)
        {
            if (Vector3.Distance(item.transform.position, adjacentHexagons[i].position) < 0.5f)
            {
                RPC_EnableHex(false, i);
            }
        }
    }
    public void RPC_EnableHex(bool flag, int i)
    {
        adjacentHexagons[i].gameObject.SetActive(flag);
    }
    public void Damage()
    {
        RPC_ShakeCamera();
        Destroy(gameObject);
    }
    public void RPC_ShakeCamera()
    {
        CameraShake.instance.ShakeCamera(1, 0.5f);
    }
    private void OnDestroy()
    {
        BotGameManager.instance.CheckPlayerPosition?.Invoke();
        OfflineHexagonManager.instance.FreeHexSpace(tileIndex);
    }
    #endregion
}