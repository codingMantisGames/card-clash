using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlaceableItem : NetworkBehaviour
{
    #region VARIABLES
    [SerializeField] private List<MeshRenderer> meshRenderers;
    [SerializeField] private Material redMat;
    [SerializeField] private Material blueMat;

    [SerializeField, Space(20)] private Outline outline;
    [SerializeField] private bool isMainBuilding = false;
    [Networked] public bool isLeft { set; get; }
    bool isSelected;
    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {

    }
    void Update()
    {

    }
    #endregion

    #region FUNCTIONS
    public void SetBuilding(bool flag = false)
    {
        isLeft = flag;

        foreach (var item in meshRenderers)
        {
            if (flag)
                item.material = redMat;
            else
                item.material = blueMat;
        }
    }
    private void OnMouseDown()
    {
        if (Gamemanager.instance.isLeft == isLeft && Gamemanager.instance.currentRoundStage != RoundStage.USING_CARDS)
            outline.enabled = !outline.enabled;
    }
    #endregion
}
