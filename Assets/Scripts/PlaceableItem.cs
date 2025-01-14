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
    [SerializeField] private Transform textHolder;
    [HideInInspector] public int moveCount;
    #endregion

    #region UNITY FUNCTIONS
    IEnumerator Start()
    {
        targetPos = transform.position;

        yield return new WaitForEndOfFrame();
        if (!Gamemanager.instance.isLeft && textHolder)
        {
            textHolder.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        ResetRound();
    }
    void Update()
    {

    }
    #endregion

    #region FUNCTIONS
    public void ResetRound()
    {
        moveCount = 1;
    }
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
    private void OnEnable()
    {
        Gamemanager.instance.OnItemSelected += HideOutline;
    }
    public void SetInitialRotation()
    {
        RPC_SetRotation(Quaternion.Euler(0, 270, 0));
    }
    private void OnDestroy()
    {
        Gamemanager.instance.OnItemSelected -= HideOutline;
    }
    private void OnMouseDown()
    {
        if (moveCount == 0)
        {
            Debug.LogWarning("Cant Move!.How this as some message");
            return;
        }

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
            CursorChanger.instance.SetMoveCursor();
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
        if (outline)
            outline.enabled = false;
        isHighlighted = false;

        CursorChanger.instance.SetNormalCursor();

        Gamemanager.instance.currentItemToMove = null;
    }
    public void MoveToPosition(Vector3[] pos, int index, bool isLeft, int cIndex)
    {
        RPC_MoveTo(pos, index, isLeft, cIndex);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_MoveTo(Vector3[] pos, int i, bool isLeft, int cIndex)
    {
        moveCount--;
        /*Vector3 direction = pos - transform.position;

        Quaternion targetRot = Quaternion.LookRotation(direction);
        //targetPos = pos;
        RPC_StartAnimation("move", true, targetRot);

        DOTween.To(() => targetPos, x => targetPos = x, pos, 1f).OnComplete(() =>
        {
            canMove = false;
            RPC_StartAnimation("move", false, Quaternion.Euler(0, 90, 0));
        });
        canMove = true;
        tileIndex = i;*/
        Sequence sequence = DOTween.Sequence();

        for (int j = 1; j < pos.Length; j++)
        {
            Vector3 direction = pos[j] - transform.position;

            Quaternion targetRot = Quaternion.LookRotation(direction);

            sequence.Append(
           DOTween.To(() => targetPos, x => targetPos = x, pos[j], 1f).OnStart(() =>
            {
                ///RPC_StartAnimation("move", true);
                RPC_SetRotation(targetRot);
            }));
        }
        RPC_StartAnimation("move", true);
        sequence.OnComplete(() =>
        {
            canMove = false;
            RPC_StartAnimation("move", false);
            RPC_SetRotation(Quaternion.Euler(0, isLeft ? 90 : 270, 0));
            tileIndex = i;

            HexagonTile tile = HexagonManager.instance.GetHexagon(i);
            tile.isUsed = true;
            tile = HexagonManager.instance.GetHexagon(cIndex);
            tile.isUsed = false;
        });

        sequence.Play();
        canMove = true;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartAnimation(string anim, bool flag)
    {
        animController.SetBool(anim, flag);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetRotation(Quaternion rot)
    {
        transform.GetChild(0).localRotation = rot;
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