using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class PlayerTower : NetworkBehaviour
{
    #region VARIABLES
    private ChangeDetector _changeDetector;
    [SerializeField] private Renderer meshRenderer;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material blueMaterial;
    [SerializeField] private TMP_Text lifeLabel;
    [Header("Networked Properties")]
    [Networked] public bool isLeft { get; set; }
    [Networked] public int life { get; set; }
    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {
        if (Object.HasInputAuthority)
        {
            RPC_SetMaterial();
        }
        life = 7;
    }
    void Update()
    {
    }
    #endregion

    #region FUNCTIONS
    #endregion

    #region RPCS

    [Rpc(RpcSources.InputAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SetMaterial()
    {
        if (isLeft)
            meshRenderer.material = redMaterial;
        else
            meshRenderer.material = blueMaterial;

        if (Runner.IsServer)
        {
            Gamemanager.instance.isLeft = true;
            transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            Gamemanager.instance.isLeft = false;
        }
    }
    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }
    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(life):
                    lifeLabel.text = life.ToString();
                    break;
            }
        }
    }
    public void Damage(int damage)
    {
        RPC_Damage(damage);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_Damage(int damage)
    {
        life -= damage;

        if (life <= 0)
        {
            //game over text
            Debug.Log("game Over");
        }
    }
    #endregion
}
