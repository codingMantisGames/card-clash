using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using DG.Tweening;

public class PlaceableItem : NetworkBehaviour
{
    #region VARIABLES
    [SerializeField] private List<MeshRenderer> meshRenderers;
    [SerializeField] private Material redMat;
    [SerializeField] private Material blueMat;

    [SerializeField, Space(20)] private Outline outline;
    [SerializeField] private bool isMainBuilding = false;
    [SerializeField] private MovementType movementType;
    [Networked] public bool isLeft { set; get; }
    [Networked] public int tileIndex { set; get; }
    bool isSelected;
    bool isHighlighted;
    Vector3 targetPos;
    bool canMove;
    [SerializeField] private Animator animController;
    #endregion

    #region UNITY FUNCTIONS
    void Start()
    {
        targetPos = transform.position;

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

        if (Gamemanager.instance.isLeft == isLeft)
        {
            Gamemanager.instance.OnItemSelected += HideOutline;
        }
    }
    private void OnDestroy()
    {
        Gamemanager.instance.OnItemSelected -= HideOutline;
    }
    private void OnMouseDown()
    {
        if (Gamemanager.instance.isLeft == isLeft && Gamemanager.instance.currentRoundStage != RoundStage.USING_CARDS && !isHighlighted)
        {
            Gamemanager.instance.OnItemSelected?.Invoke();

            if (isLeft)
                outline.OutlineColor = Color.red;
            else
                outline.OutlineColor = Color.blue;
            outline.enabled = true;
            HexagonManager.instance.ShowMovableTiles(tileIndex, movementType);

            isHighlighted = true;

            Gamemanager.instance.currentItemToMove = this;
        }
        else if (isHighlighted)
        {
            HexagonManager.instance.HideAllHex();

            isHighlighted = false;
            outline.enabled = false;

            CursorChanger.instance.SetNormalCursor();

            Gamemanager.instance.currentItemToMove = null;
        }
    }
    public void HideOutline()
    {
        outline.enabled = false;
        isHighlighted = false;

        CursorChanger.instance.SetMoveCursor();

        Gamemanager.instance.currentItemToMove = null;
    }
    public void MoveToPosition(Vector3 pos, int index)
    {
        RPC_MoveTo(pos, index);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_MoveTo(Vector3 pos, int i)
    {
        Vector3 direction = pos - transform.position;

        Quaternion targetRot = Quaternion.LookRotation(direction);
        //targetPos = pos;
        RPC_StartAnimation("move", true, targetRot);

        DOTween.To(() => targetPos, x => targetPos = x, pos, 1f).OnComplete(() =>
        {
            canMove = false;
            RPC_StartAnimation("move", false, Quaternion.Euler(0, 90, 0));
        });
        canMove = true;
        tileIndex = i;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartAnimation(string anim, bool flag, Quaternion rot)
    {
        transform.GetChild(0).rotation = rot;
        animController.SetBool(anim, flag);
    }
    public override void Render()
    {
        if (canMove)
        {
            transform.position = targetPos;
        }
    }
    #endregion
}
public enum MovementType
{
    ADJACENT, NONE
}