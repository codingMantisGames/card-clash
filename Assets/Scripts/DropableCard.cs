using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DropableCard : NetworkBehaviour
{
    #region VARIABLES
    [SerializeField] private List<MeshRenderer> meshRenderers;
    [SerializeField] private Material red;
    [SerializeField] private Material blue;

    [SerializeField, Space(20)] private Transform[] adjacentHexagons;
    [SerializeField] private Transform textHolder;
    [Networked] public bool isLeft { set; get; }
    public DropCardType dropCardType;
    [Networked] public int tileIndex { set; get; }
    #endregion

    #region UNITY FUNCTIONS
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        if (!Gamemanager.instance.isLeft && textHolder)
        {
            textHolder.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }
    void Update()
    {

    }
    #endregion

    #region FUNCTIONS
    public override void Spawned()
    {
        Gamemanager.instance.CheckPlayerPosition?.Invoke();
    }
    public void SetCard(bool flag = false)
    {
        isLeft = flag;
        RPC_SetMaterial(flag);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetMaterial(bool flag)
    {
        foreach (var item in meshRenderers)
        {
            if (flag)
                item.material = red;
            else
                item.material = blue;
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
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_EnableHex(bool flag, int i)
    {
        adjacentHexagons[i].gameObject.SetActive(flag);
    }
    public void Damage()
    {
        RPC_ShakeCamera();
        RPC_Damage();
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Damage()
    {
        Runner.Despawn(Object);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ShakeCamera()
    {
        CameraShake.instance.ShakeCamera(1, 0.5f);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        Gamemanager.instance.CheckPlayerPosition?.Invoke();
        HexagonManager.instance.FreeHexSpace(tileIndex);
    }
    #endregion
}
public enum DropCardType
{
    POWER_BOOST, RANGE_SURGE, STRIKE_FLOW
}